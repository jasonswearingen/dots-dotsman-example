using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Health : IComponentData
{
	public float value, invincibleTimer, killTimer;

}