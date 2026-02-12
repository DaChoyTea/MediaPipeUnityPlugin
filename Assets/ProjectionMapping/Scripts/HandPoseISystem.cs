using EugeneC.ECS;
using Unity.Burst;
using Unity.Entities;

namespace ProjectionMapping
{
	[BurstCompile]
	[UpdateInGroup(typeof(Eu_EffectSystemGroup))]
	[UpdateAfter(typeof(HandPointISystem))]
    public partial struct HandPoseISystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
        
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        
        }
    }
}
