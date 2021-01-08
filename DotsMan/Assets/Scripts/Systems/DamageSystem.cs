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
public class DamageSystem : SystemBase
{




	//protected override void OnCreate()
	//{
	//   }
	//protected override void OnDestroy()
	//{

	//}


	protected override void OnUpdate()
	{
		var dt = Time.DeltaTime;

		Entities.ForEach((DynamicBuffer<CollisionBufferData> col, ref Health hp) => {
			for (var i = 0; i < col.Length; i++)
			{
				if (hp.invincibleTimer <=0 &&  HasComponent<Damage>(col[i].entity))
				{
					hp.value -= GetComponent<Damage>(col[i].entity).value;
					hp.invincibleTimer = 5; //5 sec invuln after damage
				}
			}
		})
			.ScheduleParallel();

		Entities
			.WithNone<Kill>()
			.ForEach((Entity e, ref Health hp) =>
		{
			hp.invincibleTimer -= dt;
			if (hp.value <= 0)
			{
				EntityManager.AddComponentData(e, new Kill() {timer=hp.killTimer });
			}

		}).WithStructuralChanges().Run(); //method 1 to manipulate entity components at runtime


		//method 2 to manipulate entities at runtime (use a ECB)
		{
			var ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
			var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();
			Entities.ForEach((Entity e, int entityInQueryIndex, ref Kill kill) =>
			{
				kill.timer -= dt;
				if (kill.timer <= 0)
				{
					ecb.DestroyEntity(entityInQueryIndex, e); //https://docs.unity3d.com/Packages/com.unity.entities@0.16/api/Unity.Entities.EntityCommandBuffer.ParallelWriter.html?q=EntityCommandBuffer#Unity_Entities_EntityCommandBuffer_ParallelWriter_DestroyEntity_System_Int32_Unity_Entities_Entity_
			}
			}).ScheduleParallel();
			ecbSystem.AddJobHandleForProducer(this.Dependency); //note that the ecbSystem needs to wait for this DamageSystem to complete
		}
	}
}
