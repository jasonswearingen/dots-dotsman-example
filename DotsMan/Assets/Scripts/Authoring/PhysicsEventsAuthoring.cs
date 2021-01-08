using Assets.Scripts.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class PhysicsEventsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float hp;

    
    

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //dstManager.AddComponent(entity,)
        dstManager.AddBuffer<CollisionBufferData>(entity);
        dstManager.AddBuffer<TriggerBuffer>(entity);

    }
}
