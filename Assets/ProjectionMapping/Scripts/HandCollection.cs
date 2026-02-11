using Unity.Mathematics;

namespace ProjectionMapping
{
	//DO NOT TOUCH
	public enum ETrackingTarget : byte
	{
		LWrist2Thumb,
		LWrist2Index,
		LWrist2Middle,
		LWrist2Ring,
		LWrist2Pinky,
		LThumb2Index,
		LIndex2Middle,
		LMiddle2Ring,
		LRing2Pinky,
		RWrist2Thumb,
		RWrist2Index,
		RWrist2Middle,
		RWrist2Ring,
		RWrist2Pinky,
		RThumb2Index,
		RIndex2Middle,
		RMiddle2Ring,
		RRing2Pinky,
	}
	
    public static class HandCollection
    {
	    public static (float, float3) GetValue(this HandTrackingISingleton singleton, ETrackingTarget target)
	    {
		    return target switch
		    {
			    ETrackingTarget.LWrist2Thumb => (singleton.LWrist2Thumb.Current, singleton.LWrist2Thumb.Position),
			    ETrackingTarget.LWrist2Index => (singleton.LWrist2Index.Current, singleton.LWrist2Index.Position),
			    ETrackingTarget.LWrist2Middle => (singleton.LWrist2Middle.Current, singleton.LWrist2Middle.Position),
			    ETrackingTarget.LWrist2Ring => (singleton.LWrist2Ring.Current, singleton.LWrist2Ring.Position),
			    ETrackingTarget.LWrist2Pinky => (singleton.LWrist2Pinky.Current, singleton.LWrist2Pinky.Position),
			    ETrackingTarget.LThumb2Index => (singleton.LThumb2Index.Current, singleton.LThumb2Index.Position),
			    ETrackingTarget.LIndex2Middle => (singleton.LIndex2Middle.Current, singleton.LIndex2Middle.Position),
			    ETrackingTarget.LMiddle2Ring => (singleton.LMiddle2Ring.Current, singleton.LMiddle2Ring.Position),
			    ETrackingTarget.LRing2Pinky => (singleton.LRing2Pinky.Current, singleton.LRing2Pinky.Position),
			    ETrackingTarget.RWrist2Thumb => (singleton.RWrist2Thumb.Current, singleton.RWrist2Thumb.Position),
			    ETrackingTarget.RWrist2Index => (singleton.RWrist2Index.Current, singleton.RWrist2Index.Position),
			    ETrackingTarget.RWrist2Middle => (singleton.RWrist2Middle.Current, singleton.RWrist2Middle.Position),
			    ETrackingTarget.RWrist2Ring => (singleton.RWrist2Ring.Current, singleton.RWrist2Ring.Position),
			    ETrackingTarget.RWrist2Pinky => (singleton.RWrist2Pinky.Current, singleton.RWrist2Pinky.Position),
			    ETrackingTarget.RThumb2Index => (singleton.RThumb2Index.Current, singleton.RThumb2Index.Position),
			    ETrackingTarget.RIndex2Middle => (singleton.RIndex2Middle.Current, singleton.RIndex2Middle.Position),
			    ETrackingTarget.RMiddle2Ring => (singleton.RMiddle2Ring.Current, singleton.RMiddle2Ring.Position),
			    ETrackingTarget.RRing2Pinky => (singleton.RRing2Pinky.Current, singleton.RRing2Pinky.Position),
			    _ => (-1f, float3.zero)
		    };
	    }

	    public static float GetPrevious(this HandTrackingISingleton singleton, ETrackingTarget target)
	    {
		    return target switch
		    {
			    ETrackingTarget.LWrist2Thumb => singleton.LWrist2Thumb.Previous,
			    ETrackingTarget.LWrist2Index => singleton.LWrist2Index.Previous,
			    ETrackingTarget.LWrist2Middle => singleton.LWrist2Middle.Previous,
			    ETrackingTarget.LWrist2Ring => singleton.LWrist2Ring.Previous,
			    ETrackingTarget.LWrist2Pinky => singleton.LWrist2Pinky.Previous,
			    ETrackingTarget.LThumb2Index => singleton.LThumb2Index.Previous,
			    ETrackingTarget.LIndex2Middle => singleton.LIndex2Middle.Previous,
			    ETrackingTarget.LMiddle2Ring => singleton.LMiddle2Ring.Previous,
			    ETrackingTarget.LRing2Pinky => singleton.LRing2Pinky.Previous,
			    ETrackingTarget.RWrist2Thumb => singleton.RWrist2Thumb.Previous,
			    ETrackingTarget.RWrist2Index => singleton.RWrist2Index.Previous,
			    ETrackingTarget.RWrist2Middle => singleton.RWrist2Middle.Previous,
			    ETrackingTarget.RWrist2Ring => singleton.RWrist2Ring.Previous,
			    ETrackingTarget.RWrist2Pinky => singleton.RWrist2Pinky.Previous,
			    ETrackingTarget.RThumb2Index => singleton.RThumb2Index.Previous,
			    ETrackingTarget.RIndex2Middle => singleton.RIndex2Middle.Previous,
			    ETrackingTarget.RMiddle2Ring => singleton.RMiddle2Ring.Previous,
			    ETrackingTarget.RRing2Pinky => singleton.RRing2Pinky.Previous,
			    _ => -1f
		    };
	    }
    }
}
