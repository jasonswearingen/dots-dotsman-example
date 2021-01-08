//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.Entities;
//using Unity.Rendering;

///// <summary>
///// following great tutorial on creating pure dots object: https://www.youtube.com/watch?v=H-goqMxN0Bc
///// </summary>
//public class Spawner : MonoBehaviour
//{

//	[SerializeField] private Mesh unitMesh;
//	[SerializeField] private Material unitMaterial;



//	// Start is called before the first frame update
//	void Start()
//	{
//		MakeEntity();
//	}


//	private void MakeEntity()
//	{
//		var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;


//		EntityArchetype archetype = entityManager.CreateArchetype(
//			typeof(Unity.Transforms.Translation),
//			typeof(Unity.Transforms.Rotation),
//			typeof(Unity.Rendering.RenderMesh),
//			typeof(Unity.Rendering.RenderBounds),
//			typeof(Unity.Transforms.LocalToWorld)
//			);

//		var entity = entityManager.CreateEntity(archetype);
//#if UNITY_EDITOR
//		entityManager.SetName(entity, "ecs-pure-apple");
//#endif
//		entityManager.AddComponentData(entity, new Unity.Transforms.Translation() { Value = new Unity.Mathematics.float3(2, 0, 4) });

//		//entityManager.se


//		entityManager.AddSharedComponentData(entity, new RenderMesh() {
//			mesh=unitMesh,
//			material=unitMaterial,
//		});

//	}
//}
