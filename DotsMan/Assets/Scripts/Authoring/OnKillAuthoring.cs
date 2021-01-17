using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class OnKillAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public string sfxName;
    public int pointValue;
    public GameObject spawnPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new OnKill() { 
            sfxName = new Unity.Collections.FixedString64(this.sfxName), 
            pointValue = this.pointValue,
            spawnPrefab = conversionSystem.GetPrimaryEntity(this.spawnPrefab),
        });
    }

	public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
	{
        referencedPrefabs.Add(this.spawnPrefab);
	}
}
