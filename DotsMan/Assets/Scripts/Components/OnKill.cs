using UnityEngine;
using Unity.Entities;
using Unity.Collections;

//[GenerateAuthoringComponent]
public struct OnKill : IComponentData
{
	public FixedString64 sfxName;
	public Entity spawnPrefab;
	public int pointValue;
}

