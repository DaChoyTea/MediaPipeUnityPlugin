using EugeneC.ECS;
using Mediapipe.Unity;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ProjectionMapping
{
	public struct HandData
	{
		public float Current;
		public float Previous;
		public float3 Position;
	}
	
	// Maybe there's a better way to do this, but keep it as it is for now
	public struct HandTrackingISingleton : IComponentData
	{
		public HandData LWrist2Thumb;
		public HandData LWrist2Index;
		public HandData LWrist2Middle;
		public HandData LWrist2Ring;
		public HandData LWrist2Pinky;
		public HandData LThumb2Index;
		public HandData LIndex2Middle;
		public HandData LMiddle2Ring;
		public HandData LRing2Pinky;
		
		public HandData RWrist2Thumb;
		public HandData RWrist2Index;
		public HandData RWrist2Middle;
		public HandData RWrist2Ring;
		public HandData RWrist2Pinky;
		public HandData RThumb2Index;
		public HandData RIndex2Middle;
		public HandData RMiddle2Ring;
		public HandData RRing2Pinky;
	}
	
	[BurstCompile]
	[UpdateInGroup(typeof(Eu_EffectSystemGroup), OrderFirst = true)]
    public partial struct HandPointISystem : ISystem
    {
	    private const int Wrist = 0;
	    private const int Thumb = 4;
	    private const int Index = 8;
	    private const int Middle = 12;
	    private const int Ring = 16;
	    private const int Pinky = 20;

	    public void OnCreate(ref SystemState state)
	    {
		    state.RequireForUpdate<HandTrackingISingleton>();
	    }

	    [BurstCompile]
	    public void OnUpdate(ref SystemState state)
	    {
		    var tracking = SystemAPI.GetSingleton<HandTrackingISingleton>();
		    
		    var leftPos = new NativeArray<float3>(21, Allocator.Temp);
		    var rightPos = new NativeArray<float3>(21, Allocator.Temp);
		    var leftId = new NativeArray<byte>(21, Allocator.Temp);
		    var rightId = new NativeArray<byte>(21, Allocator.Temp);

		    foreach (var (point, lt, entity) 
		             in SystemAPI.Query<RefRO<HandPointIData>, RefRO<LocalTransform>>().WithEntityAccess())
		    {
			    if (point.ValueRO.EHand == EHand.None) continue;
			    if (!point.ValueRO.IsTracked) continue;
			    
			    var pos = lt.ValueRO.Position;
			    var id = point.ValueRO.ID;
			    if (id >= 21) continue;

			    switch (point.ValueRO.EHand)
			    {
				    case EHand.Left:
					    leftPos[id] = pos;
					    leftId[id] = point.ValueRO.ID;
					    break;
				    case EHand.Right:
					    rightPos[id] = pos;
					    rightId[id] = point.ValueRO.ID;
					    break;
			    }
		    }
		    
		    tracking.LWrist2Thumb.Previous = tracking.LWrist2Thumb.Current;
		    tracking.LWrist2Index.Previous = tracking.LWrist2Index.Current;
		    tracking.LWrist2Middle.Previous = tracking.LWrist2Middle.Current;
		    tracking.LWrist2Ring.Previous = tracking.LWrist2Ring.Current;
		    tracking.LWrist2Pinky.Previous = tracking.LWrist2Pinky.Current;
		    
		    tracking.LThumb2Index.Previous = tracking.LThumb2Index.Current;
		    tracking.LIndex2Middle.Previous = tracking.LIndex2Middle.Current;
		    tracking.LMiddle2Ring.Previous = tracking.LMiddle2Ring.Current;
		    tracking.LRing2Pinky.Previous = tracking.LRing2Pinky.Current;
		    
		    tracking.RWrist2Thumb.Previous = tracking.RWrist2Thumb.Current;
		    tracking.RWrist2Index.Previous = tracking.RWrist2Index.Current;
		    tracking.RWrist2Middle.Previous = tracking.RWrist2Middle.Current;
		    tracking.RWrist2Ring.Previous = tracking.RWrist2Ring.Current;
		    tracking.RWrist2Pinky.Previous = tracking.RWrist2Pinky.Current;
		    
		    tracking.RThumb2Index.Previous = tracking.RThumb2Index.Current;
		    tracking.RIndex2Middle.Previous = tracking.RIndex2Middle.Current;
		    tracking.RMiddle2Ring.Previous = tracking.RMiddle2Ring.Current;
		    tracking.RRing2Pinky.Previous = tracking.RRing2Pinky.Current;
		    
		    (tracking.LWrist2Thumb.Current, tracking.LWrist2Thumb.Position) 
			    = DistanceBetween(leftPos, leftId, Wrist, Thumb);
		    (tracking.LWrist2Index.Current, tracking.LWrist2Index.Position)
			    = DistanceBetween(leftPos, leftId, Wrist, Index);
		    (tracking.LWrist2Middle.Current, tracking.LWrist2Middle.Position)
			    = DistanceBetween(leftPos, leftId, Wrist, Middle);
		    (tracking.LWrist2Ring.Current, tracking.LWrist2Ring.Position)
			    = DistanceBetween(leftPos, leftId, Wrist, Ring);
		    (tracking.LWrist2Pinky.Current, tracking.LWrist2Pinky.Position)
			    = DistanceBetween(leftPos, leftId, Wrist, Pinky);
		    
		    (tracking.LThumb2Index.Current, tracking.LThumb2Index.Position)
			    = DistanceBetween(leftPos, leftId, Thumb, Index);
		    (tracking.LIndex2Middle.Current, tracking.LIndex2Middle.Position)
			    = DistanceBetween(leftPos, leftId, Index, Middle);
		    (tracking.LMiddle2Ring.Current, tracking.LMiddle2Ring.Position)
			    = DistanceBetween(leftPos, leftId, Middle, Ring);
		    (tracking.LRing2Pinky.Current, tracking.LRing2Pinky.Position)
			    = DistanceBetween(leftPos, leftId, Ring, Pinky);
		    
		    (tracking.RWrist2Thumb.Current, tracking.RWrist2Thumb.Position)
			    = DistanceBetween(rightPos, rightId, Wrist, Thumb);
		    (tracking.RWrist2Index.Current, tracking.RWrist2Index.Position)
			    = DistanceBetween(rightPos, rightId, Wrist, Index);
		    (tracking.RWrist2Middle.Current, tracking.RWrist2Middle.Position)
			    = DistanceBetween(rightPos, rightId, Wrist, Middle);
		    (tracking.RWrist2Ring.Current, tracking.RWrist2Ring.Position)
			    = DistanceBetween(rightPos, rightId, Wrist, Ring);
		    (tracking.RWrist2Pinky.Current, tracking.RWrist2Pinky.Position)
			    = DistanceBetween(rightPos, rightId, Wrist, Pinky);
		    
		    (tracking.RThumb2Index.Current, tracking.RThumb2Index.Position)
			    = DistanceBetween(rightPos, rightId, Thumb, Index);
		    (tracking.RIndex2Middle.Current, tracking.RIndex2Middle.Position)
			    = DistanceBetween(rightPos, rightId, Index, Middle);
		    (tracking.RMiddle2Ring.Current, tracking.RMiddle2Ring.Position)
			    = DistanceBetween(rightPos, rightId, Middle, Ring);
		    (tracking.RRing2Pinky.Current, tracking.RRing2Pinky.Position)
			    = DistanceBetween(rightPos, rightId, Ring, Pinky);
		    
		    SystemAPI.SetSingleton(tracking);

		    leftPos.Dispose();
		    rightPos.Dispose();
		    leftId.Dispose();
		    rightId.Dispose();
	    }

	    private (float, float3) DistanceBetween(NativeArray<float3> pos, NativeArray<byte> id, int id1, int id2)
	    {
		    if (id[id1] != id1 || id[id2] != id2) return (-1f, float3.zero);
		    return (math.distance(pos[id1], pos[id2]), math.lerp(pos[id1], pos[id2], 0.5f));
	    }
    }
}
