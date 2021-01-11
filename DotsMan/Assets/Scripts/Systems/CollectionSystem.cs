using Assets.Scripts.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class CollectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
		// Assign values to local variables captured in your job here, so that it has
		// everything it needs to do its work when it runs later.
		// For example,
		//     float deltaTime = Time.DeltaTime;

		// This declares a new kind of job, which is a unit of work to do.
		// The job is declared as an Entities.ForEach with the target components as parameters,
		// meaning it will process all entities in the world that have both
		// Translation and Rotation components. Change it to process the component
		// types you want.


		//itterate every player that has a triggerbuffer
		//look at entities in triggerbuffer, and if any have a collectable, mark the collectable to be killed

		//      {
		//          var ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();

		//          Entities
		//              .WithAll<Player>()
		//              .ForEach((DynamicBuffer<TriggerBuffer> tBuffer, in Player player) =>
		//              {
		//                  for (var i = 0; i < tBuffer.Length; i++)
		//                  {
		//                      var _tb = tBuffer[i];

		//                      if (HasComponent<Collectable>(_tb.entity) && !HasComponent<Kill>(_tb.entity))
		//                      {
		//                          ecb.AddComponent(_tb.entity, new Kill() { timer = 0 });
		//                      }
		//                  }
		//              }).Run();

		//}

		{
			var ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
			var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

			Entities
				.WithAll<Player>()
				.ForEach((int entityInQueryIndex, Entity playerEntity, DynamicBuffer<TriggerBuffer> tBuffer, in Player player) =>
			{
				for (var i = 0; i < tBuffer.Length; i++)
				{
					var _tb = tBuffer[i];

					if (HasComponent<Collectable>(_tb.entity) && !HasComponent<Kill>(_tb.entity))
					{
						ecb.AddComponent(entityInQueryIndex, _tb.entity, new Kill() { timer = 0 });
					}

					if (HasComponent<PowerPill>(_tb.entity) && !HasComponent<Kill>(_tb.entity))
					{
						ecb.AddComponent(entityInQueryIndex, _tb.entity, new Kill() { timer = 0 });
						var pill = GetComponent<PowerPill>(_tb.entity);
						ecb.AddComponent(entityInQueryIndex, playerEntity, pill);
						//ecb.AddComponent(entityInQueryIndex, playerEntity, new PowerPill() { pillTimer = pill.pillTimer });
					}
				}
			}).ScheduleParallel();

			ecbSystem.AddJobHandleForProducer(this.Dependency);
		}

		//Entities.ForEach((ref Translation translation, in Rotation rotation) => {
		//    // Implement the work to perform for each entity here.
		//    // You should only access data that is local or that is a
		//    // field on this job. Note that the 'rotation' parameter is
		//    // marked as 'in', which means it cannot be modified,
		//    // but allows this job to run in parallel with other jobs
		//    // that want to read Rotation component data.
		//    // For example,
		//    //     translation.Value += math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;
		//}).Schedule();
	}
}
