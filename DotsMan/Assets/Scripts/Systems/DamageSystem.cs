using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Physics.Systems;
using Assets.Scripts.Components;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(EndFramePhysicsSystem))] //seems not needed anymore
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class DamageSystem : SystemBase
{



	[Unity.Collections.LowLevel.Unsafe.NativeSetThreadIndex]
	public int nativeThreadIndex;
	[BurstCompile]
	protected override void OnUpdate()
	{
		var dt = Time.DeltaTime;

		Entities.ForEach((DynamicBuffer<CollisionBufferData> col, ref Health hp) =>
		{
			for (var i = 0; i < col.Length; i++)
			{
				if (hp.invincibleTimer <= 0 && HasComponent<Damage>(col[i].entity))
				{
					hp.value -= GetComponent<Damage>(col[i].entity).value;
					hp.invincibleTimer = 5; //5 sec invuln after damage
				}
			}
		})
			.ScheduleParallel();

		//method 1 to manipulate entity components at runtime
		{
			Entities
				.WithNone<Kill>()
				.ForEach((Entity e, ref Health hp) =>
			{
				hp.invincibleTimer -= dt;
				if (hp.value <= 0)
				{
					EntityManager.AddComponentData(e, new Kill() { timer = hp.killTimer });
				}

			}).WithStructuralChanges().Run();
		}

		//method 2 to manipulate entities at runtime (use a ECB)
		{
			var ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
			var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();


			//var sfxPlay = new NativeQueue<FixedString64>(Allocator.TempJob);			
			var onKillQueue = new NativeQueue<ECTuple<OnKill>>(Allocator.TempJob);
			//var tst = new NativeList<ECTuple<OnKill>>(Allocator.TempJob);

			var eventQueueWriter = this.eventQueueWriter;


			Entities
				.ForEach((Entity e, int entityInQueryIndex, ref Kill kill, in Translation trns, in Rotation rot) =>
			{
				kill.timer -= dt;
				if (kill.timer <= 0)
				{

					if (HasComponent<OnKill>(e))
					{
						var onKill = GetComponent<OnKill>(e);
						//sfxPlay.Enqueue(onKill.sfxName);
						onKillQueue.Enqueue(new ECTuple<OnKill>() { e = e, component = onKill });
						eventQueueWriter.Enqueue(new ECTuple<OnKill>() { e = e, component = onKill });
						if (HasComponent<Translation>(onKill.spawnPrefab)) //non-burst: EntityManager.Exists(onKill.spawnPrefab)
						{
							var spawned = ecb.Instantiate(entityInQueryIndex, onKill.spawnPrefab);
							ecb.AddComponent(entityInQueryIndex, spawned, trns);
							ecb.AddComponent(entityInQueryIndex, spawned, rot);
						}
					}

					ecb.DestroyEntity(entityInQueryIndex, e); //https://docs.unity3d.com/Packages/com.unity.entities@0.16/api/Unity.Entities.EntityCommandBuffer.ParallelWriter.html?q=EntityCommandBuffer#Unity_Entities_EntityCommandBuffer_ParallelWriter_DestroyEntity_System_Int32_Unity_Entities_Entity_

				}
			}).Schedule();
			ecbSystem.AddJobHandleForProducer(this.Dependency); //note that the ecbSystem needs to wait for this DamageSystem to complete

			this.Dependency.Complete();
			while(eventQueue.TryDequeue(out var item)){
				UnityEngine.Debug.LogWarning("Event!");
	}


			var audioQueue = AudioSystem.messageIn;

			//Job
			//	//.WithReadOnly(onKillQueue)
			//	.WithCode(() =>
			//{



			//	//while (sfxPlay.TryDequeue(out var fstr_sfx))
			//	//{
			//	//	AudioManager.instance.PlaySfxRequest(fstr_sfx.ConvertToString());
			//	//}
			//	//var ecb = ecbSystem.CreateCommandBuffer();
			//	while (onKillQueue.TryDequeue(out var tuple))
			//	{
			//		var audioMessage = new AudioSystem.AudioMessage() { audioFile = tuple.component.sfxName, type = SystemMessageType.Audio_Sfx };
			//		audioQueue.Enqueue(audioMessage);

			//		//AudioManager.instance.PlaySfxRequest(tuple.component.sfxName.ConvertToString());
			//		//int i = 0;
			//		//i++;
			//		//GameManager.instance.AddPoints(tuple.component.pointValue);
			//	}
			//})
			//	//.WithoutBurst().Run();

			//	.Schedule();

			var job = new SendKillMessageJob() {onKillQueue=onKillQueue, threadQueue = MessageSystem.threadQueue };
			var sendKillHandle = job.Schedule(this.Dependency);
			sendKillHandle.Complete();
			//onKillQueue.com
			//sfxPlay.Dispose();
			onKillQueue.Dispose(sendKillHandle);

		}


	}


	public event System.EventHandler OnKill;
	public struct OnKillEvent { }
	private NativeQueue<ECTuple<OnKill>> eventQueue;
	private NativeQueue<ECTuple<OnKill>>.ParallelWriter eventQueueWriter;

	protected override void OnCreate()
	{
		eventQueue = new NativeQueue<ECTuple<OnKill>>(Allocator.Persistent);
		eventQueueWriter = eventQueue.AsParallelWriter();
	}
	protected override void OnDestroy()
	{
		eventQueue.Dispose();
	}


	private struct SendKillMessageJob : IJob
	{
		[Unity.Collections.LowLevel.Unsafe.NativeSetThreadIndex]
		public int nativeThreadIndex;
		[ReadOnly]
		public NativeArray<UnsafeList<EventMsg>> threadQueue;
		//[ReadOnly]
		public NativeQueue<ECTuple<OnKill>> onKillQueue;

		public unsafe void Execute()
		{
			var p_threadQueue = (UnsafeList<EventMsg>*)threadQueue.GetUnsafeReadOnlyPtr();

			while (onKillQueue.TryDequeue(out var tuple))
			{
				//var audioMessage = new AudioSystem.AudioMessage() { audioFile = tuple.component.sfxName, type = SystemMessageType.Audio_Sfx };
				//audioQueue.Enqueue(audioMessage);

				p_threadQueue[nativeThreadIndex].Add(new EventMsg() { type = EventMsgType.Kill, target = tuple.e });
				//AudioManager.instance.PlaySfxRequest(tuple.component.sfxName.ConvertToString());
				//int i = 0;
				//i++;
				//GameManager.instance.AddPoints(tuple.component.pointValue);
			}

		}
	}

}



public class ExampleSystem : SystemBase
{

	[Unity.Collections.LowLevel.Unsafe.NativeSetThreadIndex]
	public int nativeThreadIndex;

	private struct ExampleJob2 : IJob
	{

		[Unity.Collections.LowLevel.Unsafe.NativeSetThreadIndex]
		public int nativeThreadIndex;

		
		public void Execute()
		{
			//UnityEngine.Debug.Log($"ExampleSystem: ExampleJob2 (IJob) threadIndex={nativeThreadIndex}");  //1 to 16.  never zero.
			UnityEngine.Debug.Log($"ExampleSystem: ExampleJob2 (IJob) ");  //1 to 16.  never zero.
		}

	
		
	}

	protected override void OnUpdate()
	{
		var nativeThreadIndex = this.nativeThreadIndex;
		Job.WithCode(() =>
		{
			//UnityEngine.Debug.Log($"ExampleSystem: Job.WithCode() threadIndex={nativeThreadIndex}");  //always zero
			UnityEngine.Debug.Log($"ExampleSystem: Job.WithCode() threadIndex={nativeThreadIndex}");  //always zero
		})
			.Schedule();

		var example2Job = new ExampleJob2();
		example2Job.Schedule().Complete();	
	}

	
}

public struct ECTuple<TComponent>
{
	public Entity e;
	public TComponent component;
}
