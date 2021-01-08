using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Physics.Systems;
using Assets.Scripts.Components;


[UpdateAfter(typeof(EndFramePhysicsSystem))] //seems not needed anymore
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class CollisionSystem : SystemBase
{
	/// <summary>
	/// job that reads collision events and updates the collisionBuffer (component on each entity) so the collision pairs are readable by ECS
	/// </summary>
	private struct CollisionSystemJob : ICollisionEventsJob
	{
		//public ComponentDataFromEntity<Player> players;
		/// <summary>
		/// all entities that include a CollisionData buffer
		/// </summary>
		public BufferFromEntity<CollisionBufferData> collisions;
		public void Execute(CollisionEvent collisionEvent)
		{
			//players[collisionEvent.EntityA]

			

			//add a collision pair to our collisionBuffer so it can be acted on by the CollisionSystem.OnUpdate()
			if (collisions.HasComponent(collisionEvent.EntityA))
			{
				collisions[collisionEvent.EntityA].Add(new CollisionBufferData()
				{
					entity = collisionEvent.EntityB
				});
			}
			if (collisions.HasComponent(collisionEvent.EntityB))
			{
				collisions[collisionEvent.EntityB].Add(new CollisionBufferData()
				{
					entity = collisionEvent.EntityA
				});
			}
		}
	}
	private struct TriggerSystemJob : ITriggerEventsJob
	{
		//public ComponentDataFromEntity<Player> players;
		public BufferFromEntity<TriggerBuffer> triggers;
		public void Execute(TriggerEvent triggerEvent)
		{
			//players[collisionEvent.EntityA]

			//add a collision pair to our collisionBuffer so it can be acted on by the CollisionSystem.OnUpdate()
			if (triggers.HasComponent(triggerEvent.EntityA))
			{
				triggers[triggerEvent.EntityA].Add(new TriggerBuffer()
				{
					entity = triggerEvent.EntityB
				});
			}
			if (triggers.HasComponent(triggerEvent.EntityB))
			{
				triggers[triggerEvent.EntityB].Add(new TriggerBuffer()
				{
					entity = triggerEvent.EntityA
				});
			}
		}
	}



	//protected override void OnCreate()
	//{
	//   }
	//protected override void OnDestroy()
	//{

	//}


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

		var pw = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld;
		var sim = World.GetOrCreateSystem<StepPhysicsWorld>().Simulation;


		/////////////////////// collisions
		Entities.ForEach((DynamicBuffer<CollisionBufferData> buffer) =>  //get all entities that have a collisionData buffer and clear it
		{
			buffer.Clear();
		}).ScheduleParallel();


		var colJobHandle = new CollisionSystemJob()  //create a new custom collision job (see struct defined above)
		{
			collisions = GetBufferFromEntity<CollisionBufferData>()
		}
			.Schedule(sim, ref pw, this.Dependency);

		//Job.WithCode(() =>
		//{
		//          //ensure colJob finishes
		//          colJobHandle.Complete();
		//      }).Run();


		////////////////////// triggers
		Entities.ForEach((DynamicBuffer<TriggerBuffer> buffer) =>
		{
			buffer.Clear();
		}).ScheduleParallel();


		var trigJobHandle = new TriggerSystemJob()
		{
			triggers = GetBufferFromEntity<TriggerBuffer>()
		}
			.Schedule(sim, ref pw, this.Dependency);

		Job.WithCode(() =>
		{
			//    //ensure colJob finishes
			colJobHandle.Complete();
			//ensure trigJob finishes
			trigJobHandle.Complete();
		}).Run();
	}
}
