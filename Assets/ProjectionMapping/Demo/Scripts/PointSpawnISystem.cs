using EugeneC.ECS;
using Unity.Burst;
using Unity.Entities;

namespace ProjectionMapping
{
	[BurstCompile]
	[UpdateInGroup(typeof(Eu_InitializationSystemGroup))]
    public partial struct PointSpawnISystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
        
        }
    }
}
