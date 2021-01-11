using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct PowerPill	: IComponentData
{
	public float pillTimer;
}