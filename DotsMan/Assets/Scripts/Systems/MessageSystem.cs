using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using System.Runtime.InteropServices;
using System;
using Unity.Collections.LowLevel.Unsafe;


public enum SystemMessageType : int
{
	Audio_Sfx,
	Audio_Music,

}
//[StructLayout(LayoutKind.Explicit, Size = 132)]
//public struct SystemMessage132
//{
//	[FieldOffset(0)]
//	public SystemMessageType type;
//	[FieldOffset(4)]
//	public FixedString128 val_str128;
//}


public enum EventMsgType : int
{
	Spawn,
	Hit,
	Kill,
	Move,
}
[StructLayout(LayoutKind.Sequential)]
public struct EventMsg
{
	public EventMsgType type;
	public double gt;
	public Entity sender;
	public Entity target;
	public FixedListInt4096 data; //TODO: huge.  would want multiple sized message queues to prevent too much wasted mem.
}

public unsafe class MessageSystem : SystemBase
{

	private struct EventMsgParseJob : IJobParallelFor
	{

		[Unity.Collections.LowLevel.Unsafe.NativeSetThreadIndex]
		public int nativeThreadIndex;
		/// <summary>
		/// don't use this reference, but it's included so we tell burst that we do read-write with it so other jobs touching it don't run at the same time.
		/// other jobs using it should use the [ReadOnly] attribute.
		/// </summary>
		public NativeArray<UnsafeList<EventMsg>> threadQueue;
		/// <summary>
		/// Thread local storge.  ptr to NativeArray<UnsafeList<EventMsg>>  
		/// </summary>
		[NativeDisableUnsafePtrRestriction]
		public UnsafeList<EventMsg>* p_threadQueue;
		[NativeDisableParallelForRestriction, ReadOnly]
		public ComponentDataFromEntity<OnKill> onKillData;
		[NativeDisableParallelForRestriction]
		public NativeQueue<AudioSystem.AudioMessage>.ParallelWriter audioIn;
		[NativeDisableParallelForRestriction, ReadOnly]
		public EntityManager em;

		public unsafe void Execute(int index)
		{
			//Debug.Log($"EventTest Thread=${nativeThreadIndex}, index={index}");
			var p_list = p_threadQueue[index].Ptr;
			var count = p_threadQueue[index].length;
			for (var i = 0; i < count; i++)
			{
				_processMsg(ref p_list[i]);
			}
			if (count != p_threadQueue[index].length)
			{
				throw new Exception("race, shouldn't happen!");
			}
			p_threadQueue[index].Clear();
		}

		private void _processMsg(ref EventMsg msg)
		{
			switch (msg.type)
			{
				case EventMsgType.Kill:
					//if (em.HasComponent<OnKill>(msg.target)) //dots bug: https://forum.unity.com/threads/in-a-ijobparallelfor-can-not-use-entitymanager.1042306/
					if (onKillData.HasComponent(msg.target))
					{
						var onKill = onKillData[msg.target];
						//var onKill = em.GetComponentData<OnKill>(msg.target);  //dots bug: https://forum.unity.com/threads/in-a-ijobparallelfor-can-not-use-entitymanager.1042306/
						audioIn.Enqueue(new AudioSystem.AudioMessage() { type = SystemMessageType.Audio_Sfx, audioFile = onKill.sfxName });                                                                                                                                     //onKill.
					}
					break;
			}
		}
	}

	//	public NativeArray<NativeQueue<EventMsg>> threadQueue;
	//public NativeQueue<EventMsg>[] threadQueue;
	public static NativeArray<UnsafeList<EventMsg>> threadQueue;

	protected override void OnCreate()
	{


		base.OnCreate();
		//threadQueue = new NativeArray<NativeQueue<EventMsg>>(Unity.Jobs.LowLevel.Unsafe.JobsUtility.MaxJobThreadCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		//threadQueue = new NativeQueue<EventMsg>[Unity.Jobs.LowLevel.Unsafe.JobsUtility.MaxJobThreadCount];
		threadQueue = new NativeArray<UnsafeList<EventMsg>>(Unity.Jobs.LowLevel.Unsafe.JobsUtility.MaxJobThreadCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		for (int i = 0; i < threadQueue.Length; i++)
		{
			//threadQueue[i] = new NativeQueue<EventMsg>(Allocator.Persistent);
			threadQueue[i] = new UnsafeList<EventMsg>(100, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			//threadQueue[i].
		}
	}
	protected override void OnDestroy()
	{
		base.OnDestroy();
		for (int i = 0; i < threadQueue.Length; i++)
		{
			threadQueue[i].Dispose();
		}
		threadQueue.Dispose();
	}
	protected override void OnUpdate()
	{

		var job = new EventMsgParseJob()
		{

			audioIn = AudioSystem.messageIn,
			em = this.EntityManager,
			threadQueue = threadQueue,
			p_threadQueue = (UnsafeList<EventMsg>*)threadQueue.GetUnsafePtr(),
			onKillData = GetComponentDataFromEntity<OnKill>(),
		};



		var handle = job
			.Schedule(threadQueue.Length, 1); //each item is a queue, so batches of 1
		handle.Complete();
	}
}

