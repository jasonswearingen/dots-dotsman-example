using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public struct Spawner : IComponentData
{
	public Entity spawnPrefab, spawnObject;

}
