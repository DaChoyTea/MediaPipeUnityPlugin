using Mediapipe.Unity;
using Unity.Entities;

namespace ProjectionMapping
{
    public struct HandPointIData : IComponentData
    {
	    public byte ID;
	    public Hand Hand;
	    public bool IsTracked;
    }
    
    public struct PointSpawnIData : IComponentData
    {
	    public float CurrentTime;
    }
}
