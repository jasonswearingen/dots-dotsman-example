using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Components
{/// <summary>
/// for storing "array of components" per entity
/// </summary>

	[GenerateAuthoringComponent] //let attach to entities in editor
	public struct TriggerBuffer : IBufferElementData
	{
		public Entity entity;

	}
}