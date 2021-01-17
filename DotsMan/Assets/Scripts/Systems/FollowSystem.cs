using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Physics.Systems;
using Assets.Scripts.Components;

public class FollowSystem : SystemBase
{
	protected override void OnUpdate()
	{
		//need (read) for followData, (get the entity target and details on how far to follow)
		//need (write) for  trans, rot

		var dt = Time.DeltaTime;

		Entities
			.WithAll<Translation, Rotation>()
			.ForEach((Entity e, in Follow follow) =>
			{

				//if(TryGetComponent(follow.target, out Translation targetPos, out Rotation targetRot))
				//{
				//	//set distance behind target:  unit vector = math.mul(quar,pos)  				
				//	var newTargetPos = targetPos.Value + math.mul(targetRot.Value, targetPos.Value) * -follow.distance;
				//	newTargetPos += follow.offset;
				//	newTargetPos = math.lerp(trans.Value, newTargetPos, dt * follow.speedMove);
				//	trans.Value = newTargetPos;
				//}

				if (HasComponent<Translation>(follow.target) && HasComponent<Rotation>(follow.target))
				{

					var currentPos = GetComponent<Translation>(e).Value;
					var currentRot = GetComponent<Rotation>(e).Value;
					var targetPos = GetComponent<Translation>(follow.target).Value;
					var targetRot = GetComponent<Rotation>(follow.target).Value;

					//set distance behind target:  unit vector = math.mul(quar,pos)  				
					var newTargetPos = targetPos + math.mul(targetRot, targetPos) * -follow.distance;
					newTargetPos += follow.offset;
					newTargetPos = math.lerp(currentPos, newTargetPos, dt * follow.speedMove);

					var newTargetRot = math.lerp(currentRot.value, targetRot.value, dt * follow.speedRotation);

					newTargetPos.x = follow.freezeXPos ? currentPos.x : newTargetPos.x;
					newTargetPos.y = follow.freezeYPos ? currentPos.y : newTargetPos.y;
					newTargetPos.z = follow.freezeZPos ? currentPos.z : newTargetPos.z;
					newTargetRot = follow.freezeRot ? currentRot.value : newTargetRot;

					//trans.Value = newTargetPos;
					SetComponent(e, new Translation() { Value = newTargetPos });
					SetComponent(e, new Rotation() { Value = newTargetRot });


					////set distance behind target:  unit vector = math.mul(quar,pos)  				
					//var newTargetPos = targetPos.Value + math.mul(targetRot.Value, targetPos.Value) * -follow.distance;
					//newTargetPos += follow.offset;
					//newTargetPos = math.lerp(trans.Value, newTargetPos, dt * follow.speedMove);
					////trans.Value = newTargetPos;
					//SetComponent(e, new Translation() { Value = newTargetPos });
				}

			})
			//.WithStructuralChanges().Run();
			.Schedule();
	}
}

//public static class Helpers
//{
//	public bool TryGetComponent<TComponent>(this SystemBaseEx system, ref Entity ent, out TComponent component) where TComponent : struct, IComponentData
//	{
//		if (!system.HasCom2<TComponent>(ref ent))
//		{
//			component = default(TComponent);
//			return false;
//		}
//		component = system.GetComponent<TComponent>(ent);
//		return true;
//	}
//}


//public abstract class SystemBaseEx : SystemBase
//{
//	//public bool HasCom2<TComponent>(ref Entity ent) where TComponent : struct, IComponentData
//	//{
//	//	return HasComponent<TComponent>(ent);
//	//}

//	public bool TryGetComponent<T1>(Entity ent, out T1 component) where T1 : struct, IComponentData
//	{
//		if (!HasComponent<T1>(ent))
//		{
//			component = default(T1);
//			return false;
//		}
//		component = GetComponent<T1>(ent);
//		return true;
//	}
//	public bool TryGetComponent<T1, T2>(Entity ent, out T1 c1, out T2 c2) where T1 : struct, IComponentData where T2 : struct, IComponentData
//	{
//		if (!HasComponent<T1>(ent) || !HasComponent<T2>(ent))
//		{
//			c1 = default(T1);
//			c2 = default(T2);
//			return false;
//		}
//		c1 = GetComponent<T1>(ent);
//		c2 = GetComponent<T2>(ent);
//		return true;
//	}
//}