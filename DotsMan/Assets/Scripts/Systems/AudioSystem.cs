using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using System.Runtime.InteropServices;


[BurstCompile]
public class AudioSystem : SystemBase
{

	[StructLayout(LayoutKind.Sequential, Size = 132)]
	public struct AudioMessage
	{
		public SystemMessageType type;
		public FixedString64 audioFile;
	}


	private NativeQueue<AudioMessage> _messageQueue;
	public static NativeQueue<AudioMessage>.ParallelWriter messageIn;

	protected override void OnCreate()
	{
		base.OnCreate();

		_messageQueue = new NativeQueue<AudioMessage>(Allocator.Persistent);
		messageIn = _messageQueue.AsParallelWriter();
	}
	protected override void OnDestroy()
	{
		base.OnDestroy();
		_messageQueue.Dispose();
	}

	/** cache known audio files to avoid GC allocations */
	private System.Collections.Generic.Dictionary<FixedString64, string> _strLookup = new System.Collections.Generic.Dictionary<FixedString64, string>();
	/// <summary>
	/// cache strings to avoid gc pressure
	/// </summary>
	/// <param name="fstr"></param>
	/// <returns></returns>
	private string _getString(ref FixedString64 fstr)
	{
		string toReturn;
		if (_strLookup.TryGetValue(fstr, out toReturn))
		{
			return toReturn;
		}
		toReturn = fstr.ToString();
		_strLookup.Add(fstr, toReturn);
		return toReturn;
	}


	protected override void OnUpdate()
	{

		var _messageQueue = this._messageQueue;
		Job
		//.WithReadOnly(messageQueueParallel) //guess this is not needed
		.WithCode(() =>
		{
			while (_messageQueue.TryDequeue(out var message))
			{
				var audioFileStr = _getString(ref message.audioFile);
				switch (message.type)
				{
					case SystemMessageType.Audio_Sfx:
						AudioManager.instance.PlaySfxRequest(audioFileStr);
						break;
					case SystemMessageType.Audio_Music:
						AudioManager.instance.PlayMusicRequest(audioFileStr);
						break;
					default:
						throw new System.Exception("invalid message type send to audioSystem");
				}
			}
		})
		.WithoutBurst()
		.Run();
	}
}