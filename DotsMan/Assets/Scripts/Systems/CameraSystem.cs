//using UnityEditor;
//using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
//using UnityEngine;

//[AlwaysUpdateSystem]//keep running the OnUpdate() even if there are no entities to work on
public class CameraSystem : SystemBase
{
	protected override void OnUpdate()
	{

		var playerQuery = GetEntityQuery(typeof(Player), typeof(Movable), typeof(Translation));

		if (playerQuery.CalculateEntityCount() == 0)
		{
			return;
		}
		var playerTrans = GetComponent<Translation>(playerQuery.GetSingletonEntity());


		var camQuery = GetEntityQuery(typeof(CameraTag), typeof(Follow));
		if (camQuery.CalculateEntityCount() == 0)
		{
			return;
		}
		var camEnt = camQuery.GetSingletonEntity();
		var camFollow = GetComponent<Follow>(camEnt);


		var minDist = float.MaxValue;

		Entities
					.WithAll<CameraPoint>()
					.ForEach((Entity e, in Translation trns) => {

						var curDist = math.distance(trns.Value, playerTrans.Value);
						//minDist = curDist < minDist ? curDist : minDist;
						if (curDist < minDist)
						{
							minDist = curDist;
							camFollow.target = e;
						}

					}).Run();

		SetComponent(camEnt, camFollow);
		
	}
}