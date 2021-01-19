//using UnityEditor;
//using UnityEngine;
using Unity.Entities;
using UnityEngine;

[AlwaysUpdateSystem]//keep running the OnUpdate() even if there are no entities to work on
public class GameStateSystem : SystemBase
{
	protected override void OnUpdate()
	{


		//////EXAMPLE:  can use Entities.ForEach() to access Monobehaviours too!  Just need to add a "ConvertToEntity" component with option "Convert and Inject"
		////Entities.ForEach((GameManager gameManager) => {
		////	Debug.Log(gameManager.scoreTextUI.text);
		////}).WithoutBurst().Run();

		////GetEntityQuery(typeof(Pellet));
		//var pelletQuery = GetEntityQuery(ComponentType.ReadOnly<Pellet>());
		////	Debug.Log(pelletQuery.CalculateEntityCount());
		//if (pelletQuery.CalculateEntityCount() <= 0)
		//{
		//	GameManager.instance.Win();

		//}
		//var playerQuery = GetEntityQuery(ComponentType.ReadOnly<Player>());
		//if (playerQuery.CalculateEntityCount() <= 0)
		//{
		//	GameManager.instance.Loose();
		//}

	}
}