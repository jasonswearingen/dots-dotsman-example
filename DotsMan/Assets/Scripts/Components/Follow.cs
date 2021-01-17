using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Follow : IComponentData
{
	public Entity target;
	public float distance, speedMove, speedRotation;
	public float3 offset;
	public bool freezeXPos, freezeYPos, freezeZPos, freezeRot;

}