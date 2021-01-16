using Assets.Scripts.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections.LowLevel.Unsafe;

//EXAMPLE: important!  different ways to modify entities in parallel.

unsafe public class CollectionSystem : SystemBase
{
	//private NativeReference<int> _pointsToAdd;
	//private int* p_pointsToAdd;
	//unsafe protected override void OnCreate()
	//{
	//	//_pointsToAdd = new Unity.Collections.NativeArray<int>(1, Allocator.Persistent);
	//	//p_pointsToAdd = (int*)Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafePtr(this._pointsToAdd);
	//	_pointsToAdd = new NativeReference<int>(Allocator.Persistent);
	//	p_pointsToAdd = (int*)_pointsToAdd.GetUnsafePtr();
	//	base.OnCreate();
	//}

	//protected override void OnDestroy()
	//{
	//	_pointsToAdd.Dispose(this.Dependency);
	//	base.OnDestroy();
	//}

	unsafe protected override void OnUpdate()
	{
		//System.GC.Collect();
		//itterate every player that has a triggerbuffer
		//look at entities in triggerbuffer, and if any have a collectable, mark the collectable to be killed

		////single threaded
		//{
		//	var ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();
		//	var pointsToAdd = 0;

		//	Entities
		//		.WithAll<Player>()
		//		.ForEach((Entity playerEntity, DynamicBuffer<TriggerBuffer> tBuffer, in Player player) =>
		//		{
		//			for (var i = 0; i < tBuffer.Length; i++)
		//			{
		//				var _tb = tBuffer[i];

		//				if (HasComponent<Collectable>(_tb.entity) && !HasComponent<Kill>(_tb.entity))
		//				{
		//					ecb.AddComponent(_tb.entity, new Kill() { timer = 0 });
		//				}

		//				if (HasComponent<Collectable>(_tb.entity) && !HasComponent<Kill>(_tb.entity))
		//				{
		//					ecb.AddComponent(_tb.entity, new Kill() { timer = 0 });
		//					var collectable = GetComponent<Collectable>(_tb.entity);
		//					pointsToAdd += collectable.points;
		//					//System.Threading.Interlocked.Add(ref pointsToAdd, collectable.points);
		//					//GameManager.instance.AddPoints(collectable.points);
		//				}

		//				if (HasComponent<PowerPill>(_tb.entity) && !HasComponent<Kill>(_tb.entity))
		//				{
		//					ecb.AddComponent(_tb.entity, new Kill() { timer = 0 });
		//					var pill = GetComponent<PowerPill>(_tb.entity);
		//					ecb.AddComponent(playerEntity, pill);
		//				}
		//			}
		//		}).WithoutBurst().Run();

		//	GameManager.instance.AddPoints(pointsToAdd);
		//}

		////parallel using class field for accumulation
		//{
		//	var ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		//	var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();
		//	var p_pointsToAdd = this.p_pointsToAdd;  

		//	Entities
		//			.WithAll<Player>()
		//			.ForEach((int entityInQueryIndex, Entity playerEntity, DynamicBuffer<TriggerBuffer> triggerBuffer, in Player player) =>
		//		{
		//			for (var i = 0; i < triggerBuffer.Length; i++)
		//			{
		//				var _tb = triggerBuffer[i];

		//				if (HasComponent<Collectable>(_tb.entity) && !HasComponent<Kill>(_tb.entity))
		//				{
		//					ecb.AddComponent(entityInQueryIndex, _tb.entity, new Kill() { timer = 0 });
		//					var collectable = GetComponent<Collectable>(_tb.entity);
		//					System.Threading.Interlocked.Add(ref *p_pointsToAdd, collectable.points);
		//				}

		//				if (HasComponent<PowerPill>(_tb.entity) && !HasComponent<Kill>(_tb.entity))
		//				{
		//					ecb.AddComponent(entityInQueryIndex, _tb.entity, new Kill() { timer = 0 });
		//					var pill = GetComponent<PowerPill>(_tb.entity);
		//					ecb.AddComponent(entityInQueryIndex, playerEntity, pill);
		//				}
		//			}
		//		})
		//			.WithNativeDisableUnsafePtrRestriction(p_pointsToAdd)
		//			.ScheduleParallel();


		//	ecbSystem.AddJobHandleForProducer(this.Dependency);

		//	if (_pointsToAdd.Value != 0)
		//	{
		//		var pointsToAdd = System.Threading.Interlocked.Exchange(ref *p_pointsToAdd, 0);
		//		GameManager.instance.AddPoints(pointsToAdd);
		//	}
		//}


		//UNSAFE parallel using only job temp variable
		//SAFE nativeQueue parallel version
		{
			var ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
			var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();
			//SAFE nativeQueue
			var EXAMPLE_SAFE_queue = new NativeQueue<int>(Allocator.TempJob);
			var EXAMPLE_SAFE_queue_writer = EXAMPLE_SAFE_queue.AsParallelWriter();
			//UNSAFE
			var pointsToAdd =new NativeReference<int>(Allocator.TempJob);
			var p_pointsToAdd =(int*) pointsToAdd.GetUnsafePtr();
			//SAFE nativeArray
			var EXAMPLE_SAFE_array = new NativeArray<int>(_query.CalculateEntityCount(), Allocator.TempJob, NativeArrayOptions.UninitializedMemory);



			Entities
				.WithStoreEntityQueryInField(ref _query) 
					.WithAll<Player>()
					.ForEach((int nativeThreadIndex, int entityInQueryIndex, Entity playerEntity, DynamicBuffer<TriggerBuffer> triggerBuffer, in Player player) =>
					{
						var pointsToAdd = 0;
						for (var i = 0; i < triggerBuffer.Length; i++)
						{
							var _tb = triggerBuffer[i];

							if (HasComponent<Collectable>(_tb.entity) && !HasComponent<Kill>(_tb.entity))
							{
								ecb.AddComponent(entityInQueryIndex, _tb.entity, new Kill() { timer = 0 });
								var collectable = GetComponent<Collectable>(_tb.entity);
								//UNSAFE
								System.Threading.Interlocked.Add(ref *p_pointsToAdd, collectable.points);
								//SAFE nativeQueue
								EXAMPLE_SAFE_queue_writer.Enqueue(collectable.points);
								//SAFE nativeArray (part 1/2)
								pointsToAdd += collectable.points;
								
							}

							if (HasComponent<PowerPill>(_tb.entity) && !HasComponent<Kill>(_tb.entity))
							{
								ecb.AddComponent(entityInQueryIndex, _tb.entity, new Kill() { timer = 0 });
								var pill = GetComponent<PowerPill>(_tb.entity);
								ecb.AddComponent(entityInQueryIndex, playerEntity, pill);
							}
						}
						//SAFE nativeArray (part 2/2)
						EXAMPLE_SAFE_array[entityInQueryIndex] = pointsToAdd;
					})
					.WithNativeDisableUnsafePtrRestriction(p_pointsToAdd)
					.ScheduleParallel();


			ecbSystem.AddJobHandleForProducer(this.Dependency);

			
			Job.WithReadOnly(pointsToAdd).WithCode(() => {
				//UNSAFE
				GameManager.instance.AddPoints(pointsToAdd.Value);
				//SAFE nativeQueue
				while (EXAMPLE_SAFE_queue.TryDequeue(out var pts))
				{
					GameManager.instance.AddPoints(pts);
				}
				//SAFE nativeArray				
				foreach (var pointsToAdd in EXAMPLE_SAFE_array)
				{
					GameManager.instance.AddPoints(pointsToAdd);
				}
			})
			//.WithDisposeOnCompletion(pointsToAdd) //doesn't work
			.WithoutBurst()
			.Run();

			pointsToAdd.Dispose(this.Dependency); //pointsToAdd.Dispose() would also work, as internally it finds this.Dependency
			EXAMPLE_SAFE_queue.Dispose(this.Dependency);
			EXAMPLE_SAFE_array.Dispose(this.Dependency);
		}
	}
	/// <summary>
	/// this "works" due to burst codegen.  see https://forum.unity.com/threads/withstoreentityqueryinfield-how-can-query-be-used-before-invocation.865681/
	/// </summary>
	private EntityQuery _query = new EntityQuery();
	
}
