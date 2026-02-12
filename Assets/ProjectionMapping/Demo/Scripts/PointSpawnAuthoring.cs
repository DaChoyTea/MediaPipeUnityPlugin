using EugeneC.ECS;
using Unity.Entities;
using UnityEngine;

namespace ProjectionMapping
{
	[DisallowMultipleComponent]
    public sealed class PointSpawnAuthoring : MonoBehaviour
    {
	    [SerializeField] private BoxAuthoring prefab;
	    [SerializeField, Min(0.01f)] private float scale = 0.05f;
	    [SerializeField, Range(0.1f, 200f)] private float frequency = 100;
	    
	    public class Baker : Baker<PointSpawnAuthoring>
	    {
		    public override void Bake(PointSpawnAuthoring authoring)
		    {
			    DependsOn(authoring.prefab);
			    
			    var e = GetEntity(TransformUsageFlags.Dynamic);
			    AddComponent(e, new PointSpawnISingleton
			    {
				    Prefab = GetEntity(authoring.prefab.gameObject, TransformUsageFlags.Dynamic),
				    Scale = authoring.scale,
				    Frequency = authoring.frequency
			    });
		    }
	    }
    }
    
    public struct PointSpawnISingleton : IComponentData
    {
	    public Entity Prefab;
	    public float Scale;
	    public float Frequency;
    }
}
