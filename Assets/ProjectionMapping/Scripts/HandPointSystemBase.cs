using EugeneC.ECS;
using Unity.Entities;

namespace ProjectionMapping
{
	public struct HandTrackingISingleton : IComponentData
	{
		
	}
	
	[UpdateInGroup(typeof(Eu_EffectSystemGroup), OrderLast = true)]
    public partial class HandPointSystemBase : SystemBase
    {
	    protected override void OnCreate()
	    {
		    RequireForUpdate<HandTrackingISingleton>();
	    }

	    protected override void OnUpdate()
	    {
		    //var handTracking = SystemAPI.GetSingleton<HandTrackingISingleton>();
	    }
    }
}
