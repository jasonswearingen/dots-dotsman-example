//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Assets.Tests.EditMode
//{
//	class PhysicsTests
//	{
//	}
//}
//[ExecuteAlways]
//[UpdateAfter(typeof(EndFramePhysicsSystem))]
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//public class TestRaycastSystem : SystemBase
//{
//    protected override void OnUpdate()
//    {
//        Debug.LogWarning("In Update TestRaycastSystem");

//        var bpw = World.GetOrCreateSystem<BuildPhysicsWorld>();
//        var pw = bpw.PhysicsWorld;
//        var handle = JobHandle.CombineDependencies(Dependency, bpw.GetOutputDependency());
//        var rayJob = new RayJob() { world = pw.CollisionWorld };
//        var jobHandle = rayJob.Schedule(handle);
//        this.Dependency = jobHandle;
//        bpw.AddInputDependency(this.Dependency);
//    }

//    struct RayJob : IJob
//    {
//        [ReadOnly]
//        //public PhysicsWorld world;
//        public CollisionWorld world;
//        public void Execute()
//        {
//            var targetDir = new float3(0, 0, -1);
//            var ray = new RaycastInput()
//            {
//                Start = float3.zero,
//                End = targetDir,
//            };
//            var hit = world.CastRay(ray, out Unity.Physics.RaycastHit closestHit);
//        }
//    }
//}


