using NUnit.Framework;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;


// Credit to https://gist.github.com/sarkahn/3f949ba912e9110e1620113133e31833 and https://github.com/5argon/EcsTesting\
public class ECSTestBase
{
    protected World World { get; private set; }
    protected EntityManager EntityManager => World.EntityManager;

    protected EntityQuery CreateEntityQuery(params ComponentType[] components) => EntityManager.CreateEntityQuery(components);

    protected EntityQuery GetEntityQuery(params ComponentType[] components) =>
        CreateEntityQuery(components);

    [SetUp]
    public void SetUpBase()
    {
        DefaultWorldInitialization.DefaultLazyEditModeInitialize();
        World = World.DefaultGameObjectInjectionWorld;
        // Unity lazily generates the world time component on the first 
        // world update. This will cause a structural  change which 
        // could invalidate certain state during testing
        World.Update();

       
    }

    [TearDown]
    public void TearDown()
    {
        World.Dispose();
    }

    // Add a system to the world - repects the systems'
    // [UpdateInGroup] and [UpdateBefore/After] attribute settings
    protected T AddSystemToWorld<T>() where T : SystemBase
    {
        DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(
            World,
            typeof(T));
        return World.GetExistingSystem<T>();
    }

    public void MakeEntity()
    {
        EntityManager em = World.DefaultGameObjectInjectionWorld.EntityManager;
        var ent = em.CreateEntity();
        em.AddComponentData(ent, new TimeComponent());
    }
}

[TestFixture]
[Category("dots lowlevel")]
public class EcsTests : ECSTestBase
{
    [UnityEngine.TestTools.UnityTest]
    public System.Collections.IEnumerator EcsBasic()
    {
        AddSystemToWorld<TimeSystem>();
        MakeEntity();
        World.Update();
        Debug.LogWarning("finish setup");

        while (true)
        {
            Debug.LogWarning("loop");
            World.Update();
            yield return null;
        }
    }
}


public struct TimeComponent : IComponentData
{
	public float dt;
	public double et;
	public long frame;
}

[AlwaysUpdateSystem]
public class TimeSystem : SystemBase
{
	private long frame;

	protected override void OnCreate()
	{
		base.OnCreate();
		frame = 0;
	}

	protected override void OnUpdate()
	{
		Debug.LogWarning("TimeSystem.OnUpdate()");
		var dt = Time.DeltaTime;
		var et = Time.ElapsedTime;
		var frame = this.frame++;


		Entities.ForEach((ref TimeComponent time) =>
		{
			time.dt = dt;
			time.et = et;
			time.frame = frame;
		}).ScheduleParallel();

	}
}

