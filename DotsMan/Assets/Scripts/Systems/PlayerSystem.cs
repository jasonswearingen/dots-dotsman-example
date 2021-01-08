using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class PlayerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        Entities
            .WithAll<Player>() //filter by player
            .ForEach((ref Movable mov //, ref Translation translation, ref Rotation rot, in Player player
            ) => {
                //Debug.Log("in player forEach");
            mov.direction = new float3(x, 0, y);
        })
            .Schedule(); //.Run() runs on main thread.   .Schedule() schedules
    }
}
