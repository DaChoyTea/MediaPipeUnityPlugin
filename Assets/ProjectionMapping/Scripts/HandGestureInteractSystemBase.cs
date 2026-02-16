using Unity.Entities;

namespace ProjectionMapping
{
	public partial class HandGestureInteractSystemBase : SystemBase
	{
		protected override void OnCreate()
		{
			RequireForUpdate<HandSettingISingleton>();
		}

		protected override void OnUpdate()
		{
			var settings = SystemAPI.GetSingleton<HandSettingISingleton>();
		}
	}
}