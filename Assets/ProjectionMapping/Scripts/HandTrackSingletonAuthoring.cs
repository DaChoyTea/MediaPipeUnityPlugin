using Unity.Entities;
using UnityEngine;

namespace ProjectionMapping
{
	[DisallowMultipleComponent]
    public sealed class HandTrackSingletonAuthoring : MonoBehaviour
    {
	    [SerializeField] private bool useGrabAny;
	    [SerializeField] private bool useGesture;
	    [SerializeField] private EHandPose gestureType;
	    
	    private void OnValidate()
	    {
		    if(useGrabAny) useGesture = false;
		    if(useGesture) useGrabAny = false;
	    }

	    public class Baker : Baker<HandTrackSingletonAuthoring>
	    {
		    public override void Bake(HandTrackSingletonAuthoring authoring)
		    {
			    var e = GetEntity(TransformUsageFlags.None);
			    AddComponent<HandTrackingISingleton>(e);
			    AddComponent<HandPoseISingleton>(e);
			    AddComponent(e, new HandSettingISingleton
			    {
				    UseGrabAny = authoring.useGrabAny,
				    UseGesture = authoring.useGesture,
				    GestureType = authoring.gestureType
			    });
		    }
	    }
    }
}
