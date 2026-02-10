using Unity.Entities;
using UnityEngine;

namespace ProjectionMapping
{
	[DisallowMultipleComponent]
    public sealed class HandTrackSingletonAuthoring : MonoBehaviour
    {
	    public class Baker : Baker<HandTrackSingletonAuthoring>
	    {
		    public override void Bake(HandTrackSingletonAuthoring singletonAuthoring)
		    {
			    var e = GetEntity(TransformUsageFlags.None);
			    AddComponent<HandTrackingISingleton>(e);
		    }
	    }
    }
}
