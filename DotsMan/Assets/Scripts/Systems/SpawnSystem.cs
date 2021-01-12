using System.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.Scripts.Systems
{
	public class SpawnSystem : SystemBase
	{
		protected override void OnUpdate()
		{



			////single threaded
			//{
			//	Entities.ForEach((int entityInQueryIndex, ref Spawner spawner, in Translation trans, in Rotation rot) =>
			//	{
			//		if (!EntityManager.Exists(spawner.spawnObject))
			//		{
			//			//doesn't exist, so spawn a copy of the prefab, using transform details from the spawner
			//			spawner.spawnObject = EntityManager.Instantiate(spawner.spawnPrefab);
			//			EntityManager.SetComponentData(spawner.spawnObject, trans);
			//			EntityManager.SetComponentData(spawner.spawnObject, rot);
			//		}
			//	})
			//	.WithStructuralChanges().Run();
			//}

			//parallel
			{
				var ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
				var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();
				Entities
				.ForEach((Entity spawnerEnt, int entityInQueryIndex, ref Spawner spawner, in Translation trans, in Rotation rot) =>
				{
					if (!HasComponent<Translation>(spawner.spawnObject))  //works in a Parallel ForEach(), makes sure that Entity and Translation component exist
					{
						//doesn't exist, so spawn a copy of the prefab, using transform details from the spawner
						spawner.spawnObject = ecb.Instantiate(entityInQueryIndex, spawner.spawnPrefab); //creates a TEMPORARY entity reference.   needs to be fixed up (see below)
						ecb.SetComponent(entityInQueryIndex, spawner.spawnObject, trans);
						ecb.SetComponent(entityInQueryIndex, spawner.spawnObject, rot);

						ecb.SetComponent(entityInQueryIndex, spawnerEnt, spawner); //need re-copy the spawner component back onto itself, to fix up the spawner.spawnObject reference.
					}
				})
				.ScheduleParallel();
				ecbSystem.AddJobHandleForProducer(this.Dependency);
			}
		}

	}
}