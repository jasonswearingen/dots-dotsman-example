using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using System.Runtime.InteropServices;
using System;

public class PlayerSystem : SystemBase
{
	protected override void OnUpdate()
	{
		var x = Input.GetAxis("Horizontal");
		var y = Input.GetAxis("Vertical");
		Entities
			.WithAll<Player>() //filter by player
			.ForEach((ref Movable mov //, ref Translation translation, ref Rotation rot, in Player player
			) =>
			{
				//Debug.Log("in player forEach");
				mov.direction = new float3(x, 0, y);
			})
			.Schedule(); //.Run() runs on main thread.   .Schedule() schedules

		var dt = Time.DeltaTime;

		var ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

		var audioQueue = AudioSystem.messageIn;
		var powerupMusicFile = new FixedString64("powerup");
		var gameMusicFile = new FixedString64("game");

		Entities
			.WithNativeDisableContainerSafetyRestriction(audioQueue)
			.WithAll<Player>()
			.ForEach((Entity playerEntity, int entityInQueryIndex, ref Health hp, ref PowerPill pill, ref Damage dmg) =>
			{
				pill.pillTimer -= dt;
				hp.invincibleTimer = pill.pillTimer;
				dmg.value = 100;

				audioQueue.Enqueue(new AudioSystem.AudioMessage() { type = SystemMessageType.Audio_Music, audioFile = powerupMusicFile });
				if (pill.pillTimer <= 0)
				{
					audioQueue.Enqueue(new AudioSystem.AudioMessage() { type = SystemMessageType.Audio_Music, audioFile = gameMusicFile });
					ecb.RemoveComponent<PowerPill>(entityInQueryIndex, playerEntity);
					dmg.value = 0;
				}
			})
			.ScheduleParallel();
		ecbSystem.AddJobHandleForProducer(this.Dependency);
	}
}
