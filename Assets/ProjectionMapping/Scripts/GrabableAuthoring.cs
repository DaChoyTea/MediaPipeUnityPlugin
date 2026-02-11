using System;
using EugeneC.ECS;
using EugeneC.Utilities;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using static Unity.Physics.Math;
using UnityEngine;

namespace ProjectionMapping
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public sealed class GrabableAuthoring : MonoBehaviour
	{
		private Rigidbody _rb;

		private void OnValidate()
		{
			_rb = GetComponent<Rigidbody>();
			_rb.useGravity = false;
		}

		private class GrabableAuthoringBaker : Baker<GrabableAuthoring>
		{
			public override void Bake(GrabableAuthoring authoring)
			{
				var e = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent<HandGrabbableIData>(e);
			}
		}
	}

	[UpdateInGroup(typeof(Eu_PostTransformSystemGroup))]
	public partial class HandPinchInputSystemBase : SystemBase
	{
		public readonly JobHandle[] PickJobHandles = new JobHandle[Enum.GetValues(typeof(ETrackingTarget)).Length];

		// Job writes here; The main thread reads after completing the handle.
		public readonly NativeReference<GrabbableData>[] GrabRefs =
			new NativeReference<GrabbableData>[Enum.GetValues(typeof(ETrackingTarget)).Length];

		private const float CastMagnitude = 1000f;

		protected override void OnCreate()
		{
			RequireForUpdate<ColliderCastISingleton>();
			RequireForUpdate<PhysicsWorldSingleton>();
			RequireForUpdate<HandTrackingISingleton>();

			for (var i = 0; i < GrabRefs.Length; i++)
				GrabRefs[i] = new NativeReference<GrabbableData>(Allocator.Persistent);
		}

		protected override void OnUpdate()
		{
			if (CameraController.Instance is null) return;

			var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
			var cast = SystemAPI.GetSingleton<ColliderCastISingleton>();
			var tracking = SystemAPI.GetSingleton<HandTrackingISingleton>();
			var dir = (float3)CameraController.Instance.transform.forward;

			foreach (ETrackingTarget t in Enum.GetValues(typeof(ETrackingTarget)))
			{
				var (value, pos) = tracking.GetValue(t);
				var idx = (int)t;

				switch (value)
				{
					case -1f:
						continue;

					case < 1f when tracking.GetPrevious(t) > 1:
					{
						// Ensure any previous pick for this hand finished before rescheduling into the same NativeReference.
						PickJobHandles[idx].Complete();

						Dependency = new GrabIJob
						{
							CollisionWorld = physicsWorld.CollisionWorld,
							IgnoreStatic = cast.IgnoreStatic,
							IgnoreTriggers = cast.IgnoreTriggers,

							GrabRef = GrabRefs[idx],
							Origin = pos,
							RayInput = new RaycastInput()
							{
								Start = pos,
								End = pos + dir * CastMagnitude,
								Filter = CollisionFilter.Default
							}
						}.Schedule(Dependency);

						PickJobHandles[idx] = Dependency;
						break;
					}

					case > 1f when tracking.GetPrevious(t) < 1:
					{
						PickJobHandles[idx].Complete();
						GrabRefs[idx].Value = new GrabbableData { Valid = false };
						PickJobHandles[idx] = default;
						break;
					}
				}
			}
		}

		protected override void OnDestroy()
		{
			for (var i = 0; i < GrabRefs.Length; i++)
			{
				if (GrabRefs[i].IsCreated)
					GrabRefs[i].Dispose();
			}
		}
	}

	[UpdateInGroup(typeof(Eu_PreTransformSystemGroup))]
	public partial class HandPinchGrabFollowSystemBase : SystemBase
	{
		private HandPinchInputSystemBase _grabSystemBase;

		protected override void OnCreate()
		{
			_grabSystemBase = World.GetOrCreateSystemManaged<HandPinchInputSystemBase>();
			RequireForUpdate<ColliderCastISingleton>();
			RequireForUpdate<HandTrackingISingleton>();
		}

		protected override void OnUpdate()
		{
			// Complete all scheduled pick jobs first, then read their results on the main thread.
			var combined = Dependency;
			foreach (var t in _grabSystemBase.PickJobHandles)
				combined = JobHandle.CombineDependencies(combined, t);

			combined.Complete();

			for (var i = 0; i < _grabSystemBase.PickJobHandles.Length; i++)
			{
				if (_grabSystemBase.PickJobHandles[i].Equals(default(JobHandle))) continue;
				_grabSystemBase.PickJobHandles[i] = default;
			}

			var cast = SystemAPI.GetSingleton<ColliderCastISingleton>();
			var refs = _grabSystemBase.GrabRefs;

			for (var i = 0; i < refs.Length; i++)
			{
				if (!refs[i].Value.Valid) continue;
				var entity = refs[i].Value.Target;
				var destination = refs[i].Value.Origin;

				if (cast.DeleteEntityOnClick)
				{
					EntityManager.DestroyEntity(entity);
					refs[i].Value = new GrabbableData
					{
						Valid = false
					};
					continue;
				}

				if (cast.DeleteTagEntityOnClick && SystemAPI.HasComponent<DestroyIEnableableTag>(entity))
				{
					SystemAPI.SetComponentEnabled<DestroyIEnableableTag>(entity, true);
					refs[i].Value = new GrabbableData
					{
						Valid = false
					};
					continue;
				}

				if (!SystemAPI.HasComponent<PhysicsMass>(entity)) continue;
				//if (!SystemAPI.HasComponent<HandGrabbableIData>(entity)) continue;
				if (SystemAPI.HasComponent<PhysicsMassOverride>(entity)) continue;

				var mass = SystemAPI.GetComponent<PhysicsMass>(entity);
				var massOverride = SystemAPI.GetComponentLookup<PhysicsMassOverride>(true);
				var vel = SystemAPI.GetComponent<PhysicsVelocity>(entity);
				var lt = SystemAPI.GetComponent<LocalTransform>(entity);

				if (mass.HasInfiniteMass ||
				    massOverride.HasComponent(entity) && massOverride[entity].IsKinematic != 0) continue;
				var worldFromBody = new MTransform(lt.Rotation, lt.Position);

				var bodyFromMotion = new MTransform(mass.InertiaOrientation, mass.CenterOfMass);
				var worldFromMotion = Mul(worldFromBody, bodyFromMotion);

				const float gain = 0.95f;
				vel.Linear *= gain;
				vel.Angular *= gain;

				var bodyCenterNPointWorldPos = Mul(worldFromBody, refs[i].Value.PointOnBody);

				var bodyCenterNPointLocalPos = Mul(Inverse(bodyFromMotion), refs[i].Value.PointOnBody);
				float3 deltaVel;
				{
					var diff = bodyCenterNPointWorldPos - destination;
					float3 relativeVelInWorld;
					{
						var tangentVel = math.cross(vel.Angular, bodyCenterNPointLocalPos);
						var relativeVelInBody = vel.Linear + math.mul(worldFromMotion.Rotation, tangentVel);
						relativeVelInWorld = Mul(worldFromMotion, relativeVelInBody);
					}

					const float elasticity = 0.1f;
					const float damping = 0.5f;
					deltaVel = -diff * (elasticity / SystemAPI.Time.DeltaTime) - damping * relativeVelInWorld;
				}

				float3x3 effectiveMassMatrix;
				{
					float3 arm = bodyCenterNPointWorldPos - worldFromMotion.Translation;
					var skew = new float3x3(
						new float3(0.0f, arm.z, -arm.y),
						new float3(-arm.z, 0.0f, arm.x),
						new float3(arm.y, -arm.x, 0.0f)
					);

					// world space inertia = worldFromMotion * inertiaInMotionSpace * motionFromWorld
					var invInertiaWs = new float3x3(
						mass.InverseInertia.x * worldFromMotion.Rotation.c0,
						mass.InverseInertia.y * worldFromMotion.Rotation.c1,
						mass.InverseInertia.z * worldFromMotion.Rotation.c2
					);
					invInertiaWs = math.mul(invInertiaWs, math.transpose(worldFromMotion.Rotation));

					float3x3 invEffMassMatrix = math.mul(math.mul(skew, invInertiaWs), skew);
					invEffMassMatrix.c0 = new float3(mass.InverseMass, 0.0f, 0.0f) - invEffMassMatrix.c0;
					invEffMassMatrix.c1 = new float3(0.0f, mass.InverseMass, 0.0f) - invEffMassMatrix.c1;
					invEffMassMatrix.c2 = new float3(0.0f, 0.0f, mass.InverseMass) - invEffMassMatrix.c2;

					effectiveMassMatrix = math.inverse(invEffMassMatrix);
				}

				// Calculate impulse to cause the desired change in velocity
				var impulse = math.mul(effectiveMassMatrix, deltaVel);

				// Clip the impulse
				const float maxAcceleration = 250.0f;
				float maxImpulse = math.rcp(mass.InverseMass) * SystemAPI.Time.DeltaTime * maxAcceleration;
				impulse *= math.min(1.0f, math.sqrt((maxImpulse * maxImpulse) / math.lengthsq(impulse)));
				{
					vel.Linear += impulse * mass.InverseMass;

					float3 impulseLs = math.mul(math.transpose(worldFromMotion.Rotation), impulse);
					float3 angularImpulseLs = math.cross(bodyCenterNPointLocalPos, impulseLs);
					vel.Angular += angularImpulseLs * mass.InverseInertia;
				}

				SystemAPI.SetComponent(entity, vel);
			}
		}
	}
}