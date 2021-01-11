//using UnityEditor;
//using UnityEngine;
using Unity.Entities;
using UnityEngine;

[AlwaysUpdateSystem]//keep running the OnUpdate() even if there are no entities to work on
public class GameStateSystem : SystemBase
{
	protected override void OnUpdate()
	{
		//GetEntityQuery(typeof(Pellet));
		var pelletQuery = GetEntityQuery(ComponentType.ReadOnly<Pellet>());
		Debug.Log(pelletQuery.CalculateEntityCount());
	}
}