using Unity.Entities;
using UnityEngine;

namespace EugeneC.ECS
{
	[DisallowMultipleComponent]
	public sealed class VoxelizerAuthoring : MonoBehaviour
	{
		[SerializeField, Min(0.01f)] private float voxelSize = 0.05f;
		[SerializeField, Min(0.1f)] private float voxelLife = 0.3f;
		[SerializeField] private float colorFrequency = 0.5f;
		[SerializeField] private float colorSpeed = 0.5f;
		[SerializeField] private float groundLevel = 0.0f;
		[SerializeField] private float gravity = 0.2f;
		public class Baker : Baker<VoxelizerAuthoring>
		{
			public override void Bake(VoxelizerAuthoring authoring)
			{
				var e = GetEntity(TransformUsageFlags.None);
				AddComponent(e, new VoxelizerISingleton
				{
					VoxelSize = authoring.voxelSize,
					VoxelLife = authoring.voxelLife,
					ColorFrequency = authoring.colorFrequency,
					ColorSpeed = authoring.colorSpeed,
					GroundLevel = authoring.groundLevel,
					Gravity = authoring.gravity
				});
			}
		}
	}
	
	public struct VoxelizerISingleton : IComponentData
	{
		public float VoxelSize;
		public float VoxelLife;
		public float ColorFrequency;
		public float ColorSpeed;
		public float GroundLevel;
		public float Gravity;
	}
}