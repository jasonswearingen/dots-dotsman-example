using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
class CameraAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	public AudioListener audioListener;
	public Camera cam;

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		dstManager.AddComponentData(entity, new CameraTag() { });
		conversionSystem.AddHybridComponent(audioListener);
		conversionSystem.AddHybridComponent(cam);
	}
}
