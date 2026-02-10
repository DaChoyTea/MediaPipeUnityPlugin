using System;
using EugeneC.ECS;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace ProjectionMapping
{
	[DisallowMultipleComponent]
    public sealed class PointSpawnAuthoring : MonoBehaviour
    {
	    [SerializeField] private BoxAuthoring prefab;
	    [SerializeField] private LayerMask layerMask;
	    [SerializeField, Min(0.1f)] private float frequency = 100;

	    private uint _id;

	    private void OnValidate()
	    {
		    _id = (uint)this.gameObject.GetInstanceID();
	    }

	    public class Baker : Baker<PointSpawnAuthoring>
	    {
		    public override void Bake(PointSpawnAuthoring authoring)
		    {
			    var e = GetEntity(TransformUsageFlags.Dynamic);
			    AddComponent(e, new PointSpawnISingleton
			    {
				    Prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
				    LayerMask = authoring.layerMask.value,
				    Frequency = authoring.frequency,
				    Random = new Random((uint) System.DateTime.Now.Ticks + authoring._id)
			    });
		    }
	    }
    }
    
    public struct PointSpawnISingleton : IComponentData
    {
	    public Entity Prefab;
	    public int LayerMask;
	    public float Frequency;
	    public Random Random;
    }
}
