using System.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using System;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Jobs;


//public static class Consts
//{
//	public static float3 North = new float3(0, 0, 1);
//	public static float3 South = new float3(0, 0, -1);
//	public static float3 East = new float3(0, 0, 1);
//	public static float3 West = new float3(0, 0, -1);

//	public static FixedList128<float3> dirs = new FixedList128<float3>() { North, South, East, West };
//}
[UpdateAfter(typeof(EndFramePhysicsSystem))] //always at constant point
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//[UpdateInGroup(typeof(GameLogicFixedSystemGroup))]
//[BurstCompatible]	
public class EnemySystem : SystemBase
{
	private Unity.Mathematics.Random rand = new Unity.Mathematics.Random(1234);
	//public NativeArray<Unity.Mathematics.Random> RandomArray { get; private set; }

	//protected override void OnCreate()
	//{
	//	var randomArray = new Unity.Mathematics.Random[Unity.Jobs.LowLevel.Unsafe.JobsUtility.MaxJobThreadCount];
	//	var seed = new System.Random();
	//	for (int i = 0; i < Unity.Jobs.LowLevel.Unsafe.JobsUtility.MaxJobThreadCount; ++i)
	//		randomArray[i] = new Unity.Mathematics.Random((uint)seed.Next());

	//	RandomArray = new NativeArray<Unity.Mathematics.Random>(randomArray, Allocator.Persistent);

	//	Console.WriteLine("hi");
	//}
	//protected override void OnDestroy()
	//{
	//	RandomArray.Dispose();
	//}


	protected override void OnUpdate()
	{
		var raycaster = new MovementRayCast() { pw = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld };
		//var pw = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld;
		this.rand.NextInt();
		var rand = this.rand;
		var dt = Time.DeltaTime;
		var et = Time.ElapsedTime;


		////var dirs = new Unity.Collections.FixedList32<float3>() {new float3(0,0,-1) };
		//NativeArray<Unity.Mathematics.Random> randTest = new NativeArray<Unity.Mathematics.Random>();
		//randTest[0] = new Unity.Mathematics.Random();
		//var sptr = Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetAtomicSafetyHandle(randTest);
		//var pptr =(Unity.Mathematics.Random *) Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(randTest);
		//Console.WriteLine(pptr[0].NextInt());
		//Unity.Mathematics.Random* pRand = stackalloc Unity.Mathematics.Random[10];
		//pRand[0] = new Unity.Mathematics.Random(1);

		//var randArray = RandomArray;
		//var pRandArray = (Unity.Mathematics.Random*)Unity.Collections.LowLevel.Unsafe.NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(randArray);
		//pRandArray[0].state

		//Debug.Log("hi");

		Entities.ForEach((int nativeThreadIndex, ref Movable mov, ref Enemy enemy, in Translation trans) =>
		{
			Debug.Log($"in enemy forEach!");
			//Debug.Log($"in enemy forEach! {et - enemy.lastMoveChoiceTime}");

			//var pRandTest = pRand[0].NextInt();

			//Unity.Mathematics.math.round()


			//var wasInit = enemy.isInit;
			//if (!wasInit)
			//{
			//	enemy.init();
			//}
			//if (!wasInit || math.distance(trans.Value, enemy.lastCell) > 0.9f)
			if ((math.distance(trans.Value, enemy.lastCell) > 0.9f) || (et - enemy.lastMoveChoiceTime) > 3f)// || trans.Value.Equals(enemy.lastPos))//|| mov.direction.Equals(float3.zero))
			{

				Debug.Log("in enemy move section!");

				enemy.lastCell = math.round(trans.Value);
				enemy.lastMoveChoiceTime = et;






				//raycast here
				var validDir = new NativeList<float3>(Allocator.Temp);
				//var validDir = new FixedList128<float3>();

				if (!raycaster.CheckRay(trans.Value, new float3(0, 0, -1), ref mov.direction))
				{
					validDir.Add(new float3(0, 0, -1));
				}
				else
				{
					Debug.Log("hit z-1");
				}
				if (!raycaster.CheckRay(trans.Value, new float3(0, 0, 1), ref mov.direction))
				{
					validDir.Add(new float3(0, 0, 1));
				}
				else
				{
					Debug.Log("hit z 1");
				}
				if (!raycaster.CheckRay(trans.Value, new float3(-1, 0, 0), ref mov.direction))
				{
					validDir.Add(new float3(-1, 0, 0));
				}
				else
				{
					Debug.Log("hit x-1");
				}
				if (!raycaster.CheckRay(trans.Value, new float3(1, 0, 0), ref mov.direction))
				{
					validDir.Add(new float3(1, 0, 0));
				}
				else
				{
					Debug.Log("hit x 1");
				}
				//if (raycaster.CheckRay(pos: trans.Value, targetDir: new float3(0, 0, -1), currentDir: ref mov.direction))
				//{
				//	validDir.Add(new float3(0, 0, -1));
				//}
				//if (raycaster.CheckRay(pos: trans.Value, targetDir: new float3(0, 0, 1), currentDir: ref mov.direction))
				//{
				//	validDir.Add(new float3(0, 0, 1));
				//}
				//if (raycaster.CheckRay(pos: trans.Value, targetDir: new float3(-1, 0, 0), currentDir: ref mov.direction))
				//{
				//	validDir.Add(new float3(-1, 0, 0));
				//}
				//if (raycaster.CheckRay(pos: trans.Value, targetDir: new float3(1, 0, 0), currentDir: ref mov.direction))
				//{
				//	validDir.Add(new float3(1, 0, 0));
				//}


				if (validDir.Length == 0)
				{
					Debug.Log("Empty");
				}
				else
				{

					mov.direction = validDir[rand.NextInt(validDir.Length)];
					Debug.Log($"FULL mov {mov.direction}");

				}

				validDir.Dispose();


				//if (raycaster.CheckRay(pos: trans.Value, targetDir: new float3(0, 0, -1), currentDir: ref mov.direction))
				//{
				//	validDir.Add(new float3(0, 0, -1));
				//}
				//if (raycaster.CheckRay(pos: trans.Value, targetDir: new float3(0, 0, 1), currentDir: ref mov.direction))
				//{
				//	validDir.Add(new float3(0, 0, 1));
				//}
				//if (raycaster.CheckRay(pos: trans.Value, targetDir: new float3(-1, 0, 0), currentDir: ref mov.direction))
				//{
				//	validDir.Add(new float3(-1, 0, 0));
				//}
				//if (raycaster.CheckRay(pos: trans.Value, targetDir: new float3(1, 0, 0), currentDir: ref mov.direction))
				//{
				//	validDir.Add(new float3(1, 0, 0));
				//}


				//raycaster.CheckRayDist(trans.Value, new float3(0, 0, -1), mov.direction);
				//raycaster.CheckRay(trans.Value, new float3(0, 0, -1), ref mov.direction);




			}



			enemy.lastPos = trans.Value;

		}).ScheduleParallel();

		this.Dependency.Complete(); //ensure completes this frame
									//Job.WithCode(() => {
									//	this.rand = rand;
									//}).Schedule();



	}

	//protected override void OnUpdate2()
	//{
	//	var raycaster = new MovementRayCast() { pw = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld };
	//	this.rand.NextInt();
	//	var rand = this.rand;
	//	Entities.ForEach((int nativeThreadIndex, ref Movable mov, ref Enemy enemy, in Translation trans) =>
	//	{
	//		if ((math.distance(trans.Value, enemy.lastCell) > 0.9f))
	//		{
	//			enemy.lastCell = math.round(trans.Value);
	//			//raycast here
	//			var validDir = new NativeList<float3>(Allocator.Temp);

	//			if (!raycaster.CheckRay(trans.Value, new float3(0, 0, -1), ref mov.direction))
	//			{
	//				validDir.Add(new float3(0, 0, -1));
	//			}
	//			else
	//			{
	//				Debug.Log("hit z-1");
	//			}
	//			if (!raycaster.CheckRay(trans.Value, new float3(0, 0, 1), ref mov.direction))
	//			{
	//				validDir.Add(new float3(0, 0, 1));
	//			}
	//			else
	//			{
	//				Debug.Log("hit z 1");
	//			}
	//			if (!raycaster.CheckRay(trans.Value, new float3(-1, 0, 0), ref mov.direction))
	//			{
	//				validDir.Add(new float3(-1, 0, 0));
	//			}
	//			else
	//			{
	//				Debug.Log("hit x-1");
	//			}
	//			if (!raycaster.CheckRay(trans.Value, new float3(1, 0, 0), ref mov.direction))
	//			{
	//				validDir.Add(new float3(1, 0, 0));
	//			}
	//			else
	//			{
	//				Debug.Log("hit x 11");
	//			}
	//			if (validDir.Length == 0)
	//			{
	//				Debug.Log("Empty");
	//			}
	//			else
	//			{

	//				Debug.Log("FULL");
	//				mov.direction = validDir[rand.NextInt(validDir.Length)];

	//			}
	//			validDir.Dispose();
	//		}
	//		enemy.lastPos = trans.Value;

	//	}).ScheduleParallel();
	//}



	private struct MovementRayCast
	{



		[ReadOnly]
		public PhysicsWorld pw;

		public bool CheckRay(float3 pos, float3 targetDir, ref float3 currentDir)
		{
			if (targetDir.Equals(-currentDir))
			{
				//don't let enemy do an abrupt 180
				return true;
			}
			var ray = new RaycastInput()
			{
				Start = pos,
				End = pos + (targetDir * 0.9f),
				Filter = new CollisionFilter()
				{
					GroupIndex = 0,
					BelongsTo = 1u << 1, //bitmasks, so using bitshifts
					CollidesWith = 1u << 2
				}
			};
			var hit = pw.CastRay(ray, out Unity.Physics.RaycastHit closestHit);

			return hit;// ? math.length((closestHit.Position - pos)) : null;

		}

		public float? CheckRayDist(float3 pos, float3 targetDir, float3 currentDir)
		{
			var ray = new RaycastInput()
			{
				Start = pos,
				End = pos + (targetDir * 100f),
				Filter = new CollisionFilter()
				{
					GroupIndex = 0,
					BelongsTo = 1u << 1, //bitmasks, so using bitshifts
					CollidesWith = 1u << 2
				}
			};
			var hit = pw.CastRay(ray, out Unity.Physics.RaycastHit closestHit);

			float? toReturn = hit ? math.length((closestHit.Position - pos)) : (null as float?);
			return toReturn;
		}
	}
}



//[UpdateBefore(typeof(Unity.Physics.Systems.StepPhysicsWorld))]


//var startPhysics = World.GetOrCreateSystem<Unity.Physics.Systems.>();
//var endPhysics = World.GetOrCreateSystem<EndFramePhysicsSystem>();


//bpw.AddInputDependency(this.Dependency);


//this.CompleteDependency();


//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using Unity.Physics;
//using Unity.Physics.Systems;
//using Unity.Jobs;


//[UpdateAfter(typeof(EndFramePhysicsSystem))]
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//public class TestRaycastSystem : SystemBase
//{
//	private JobHandle _lastHandle = new JobHandle();
//	protected override void OnUpdate()
//	{

//		_lastHandle.Complete();

//		var bpw = World.GetOrCreateSystem<BuildPhysicsWorld>();
//		var pw = bpw.PhysicsWorld;

//		var handle = JobHandle.CombineDependencies(Dependency, bpw.GetOutputDependency());
//		this.Dependency = Entities
//			.WithNativeDisableParallelForRestriction(pw)
//			.WithReadOnly(pw)
//			.ForEach((int nativeThreadIndex, ref Movable mov, ref Enemy enemy, in Translation trans) =>
//		{
//			var targetDir = new float3(0, 0, -1);
//			var ray = new RaycastInput()
//			{
//				Start = trans.Value,
//				End = trans.Value + (targetDir * 0.9f),
//				Filter = new CollisionFilter()
//				{
//					GroupIndex = 0,
//					BelongsTo = 1u << 1, //bitmasks, so using bitshifts
//					CollidesWith = 1u << 2
//				}
//			};

//			var hit = pw.CastRay(ray, out Unity.Physics.RaycastHit closestHit);

//			if (hit)
//			{
//				mov.direction = mov.direction; //dummy write				
//			}
//			enemy.lastPos = enemy.lastPos; //dummy write
//		})
//			.ScheduleParallel(handle);
//		bpw.AddInputDependency(this.Dependency);
//		this._lastHandle = this.Dependency;
//	}
//}






[ExecuteAlways]
[UpdateAfter(typeof(BeginSimulationEntityCommandBufferSystem))]
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup),OrderLast =true)]
public class GameLogicFixedSystemGroup : FixedStepSimulationSystemGroup
{
	//protected override void OnCreate()
	//{
	//	base.OnCreate();
	//	//this.
	//	//World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>().
	//}
	public float Timestep
	{
		get
		{
			return (base.FixedRateManager != null) ? base.FixedRateManager.Timestep : 0f;
		}
		set
		{
			if (base.FixedRateManager != null)
			{
				base.FixedRateManager.Timestep = value;
			}
		}
	}

	[Obsolete("MaximumDeltaTime is now specified at the World level as World.MaximumDeltaTime (RemovedAfter 2020-12-26)")]
	public float MaximumDeltaTime
	{
		get
		{
			return base.World.MaximumDeltaTime;
		}
		set
		{
			base.World.MaximumDeltaTime = value;
		}
	}

	public GameLogicFixedSystemGroup()
	{
		float fixedDeltaTime = 0.0166666675f * 2.0f;
		base.FixedRateManager = new FixedRateUtils.FixedRateCatchUpManager(fixedDeltaTime);
	}
}