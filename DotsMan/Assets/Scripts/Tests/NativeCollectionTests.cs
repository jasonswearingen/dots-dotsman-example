using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
//using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.TestTools;

//[BurstCompile]
[Category("dots lowlevel")]
public class NativeCollectionTests
{
	// A Test behaves as an ordinary method
	[Test]
	public void NewTestScriptSimplePasses()
	{
		// Use the Assert class to test conditions
	}

	// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
	// `yield return null;` to skip a frame.
	[UnityTest]
	public IEnumerator NewTestScriptWithEnumeratorPasses()
	{
		// Use the Assert class to test conditions.
		// Use yield to skip a frame.
		yield return null;
	}


	[Test]
	[Category("ecs lowlevel")]
	public void NativeStreamBasic()
	{
		var stream = new NativeStream(1,Allocator.Persistent);

		
		var writer = stream.AsWriter();
		var reader = stream.AsReader();

		writer.BeginForEachIndex(0);
		for (var i = 0; i < 100; i++)
		{
			if (i % 2 == 0)
			{
				//unsafe{
				//	var pInt = (int*)writer.Allocate(sizeof(int));
				//	*pInt = i;
				//}
				ref int x = ref writer.Allocate<int>();
				x = i;
			}
			else
			{
				writer.Write(i);
			}
		}
		writer.EndForEachIndex();

		reader.BeginForEachIndex(0);
		for (var i = 0; i < 100; i++)
		{
			var result = reader.Read<int>();
			Assert.AreEqual(i, result);
		}
		reader.EndForEachIndex();

		stream.Dispose();
	}

	[Test]
	[Category("ecs lowlevel")]
	public void NativeQueueBasic()
	{
		// Use the Assert class to test conditions
		//Assert.IsTrue(false,"boomtest");

		{
			var queue = new NativeQueue<QueueJobDataPak>(Allocator.Persistent);
			var queueWriter = queue.AsParallelWriter();
			var jobInnerLoopCount = 1000;
			var parallelCount = 100;

			var wJob = new WriterJob() { writerJobId = 1, jobInnerLoopCount = jobInnerLoopCount, queueWriter = queueWriter };
			var rJob = new ReaderJob() { writerJobIdMin = 1, writerJobIdMax=1, jobInnerLoopCount = jobInnerLoopCount, queueReader = queue, parallelCount=parallelCount };

			var w1 = wJob.Schedule(parallelCount, 10, new JobHandle());

			var r1 = rJob.Schedule(w1);


			//w1.Complete();  //not needed, as r1 depends on w1
			r1.Complete();

			new EndJob() { queue = queue }.Run();

			queue.Dispose();
		}
	}


	

	[Test]
	[Category("ecs lowlevel")]
	public void NativeQueueParallel()
	{
		{
			var queue = new NativeQueue<QueueJobDataPak>(Allocator.Persistent);
			var queueWriter = queue.AsParallelWriter();
			var jobInnerLoopCount = 1000;
			var parallelCount = 100;
			var targetBatchSize = 10;

			var w1 = new WriterJob() { writerJobId = 1, jobInnerLoopCount = jobInnerLoopCount, queueWriter = queueWriter }.Schedule(parallelCount, targetBatchSize, new JobHandle());
			var w2 = new WriterJob() { writerJobId = 2, jobInnerLoopCount = jobInnerLoopCount, queueWriter = queueWriter }.Schedule(parallelCount, targetBatchSize, new JobHandle());

			var rJob = new ReaderJob() { writerJobIdMin = 1, writerJobIdMax = 2, jobInnerLoopCount = jobInnerLoopCount, queueReader = queue, parallelCount = parallelCount };
			var r1 = rJob.Schedule(w1);
			var r2 = rJob.Schedule(JobHandle.CombineDependencies(r1, w2));

			r1.Complete();
			r2.Complete();


			//Assert.IsTrue(queue.Count == 0,$"should be zero at end.  ended with {queue.Count}");  //BUG: will eat any asserts thrown in a job.  do next line instead.
			new EndJob() { queue = queue }.Run();


			queue.Dispose();
		}
	}



	public struct EndJob : IJob
	{
		public NativeQueue<QueueJobDataPak> queue;

		public void Execute()
		{
			Assert.Zero(queue.Count, "should be zero at end.");
		}
	}

	struct WriterJob : IJobParallelFor
	{
		[Unity.Collections.LowLevel.Unsafe.NativeDisableContainerSafetyRestriction]
		public NativeQueue<QueueJobDataPak>.ParallelWriter queueWriter;
		public int jobInnerLoopCount;
		public int writerJobId;
		public void Execute(int index)
		{
			for (var i = 0; i < jobInnerLoopCount; i++)
			{
				queueWriter.Enqueue(new QueueJobDataPak(new QueueJobDataItem() { writerJobId = writerJobId, parallelIndex = index, jobInnerLoopIndex = i }));
			}
		}
	}

	struct ReaderJob : IJob
	{
		public NativeQueue<QueueJobDataPak> queueReader;
		public int jobInnerLoopCount;
		public int writerJobIdMin, writerJobIdMax;
		public int parallelCount;
		public void Execute()
		{
			//var startingCount = queueReader.Count;
			//Assert.IsTrue(startingCount % (parallelCount * jobInnerLoopCount) == 0, $"expected startingCount amount doesn't match. {startingCount}, {parallelCount}, {jobInnerLoopCount}");
			var dequeueCount = 0;
			for (var parallelIndex =0; parallelIndex < parallelCount; parallelIndex++)
			{
				for (var i = 0; i < jobInnerLoopCount; i++)
				{
					var result = queueReader.TryDequeue(out var dataPak);
					Assert.IsTrue(result);
					dequeueCount++;
					Assert.IsTrue(dataPak.val0.writerJobId>= writerJobIdMin && dataPak.val0.writerJobId<=writerJobIdMax);
					Assert.IsTrue(dataPak.CheckAllEqual());
					//Assert.AreEqual(tupple.parallelIndex, parallelIndex); //sometimes the writeJob.Execute(int index) other workers can race., index can jump around
					//Assert.AreEqual(tupple.jobInnerLoopIndex, i); //sometimes the other workers can race, i jumps around
				}
			}
			//var endingCount = queueReader.Count;
			//Assert.AreEqual(startingCount, endingCount, dequeueCount);
			//Assert.IsTrue(endingCount % (parallelCount * jobInnerLoopCount) == 0,$"expected ending amount doesn't match. {endingCount}, {parallelCount}, {jobInnerLoopCount}");		
			
		}
	}

	public struct QueueJobDataPak
	{
		public QueueJobDataItem val0;
		public QueueJobDataItem val1;
		public QueueJobDataItem val2;
		public QueueJobDataItem val3;
		public QueueJobDataItem val4;
		public QueueJobDataItem val5;
		public QueueJobDataItem val6;
		public QueueJobDataItem val7;
		public QueueJobDataItem val8;
		public QueueJobDataItem val9;

		public bool CheckAllEqual()
		{
			return val0.Equals(ref val1) &&
				val1.Equals(ref val2) &&
				val2.Equals(ref val3) &&
				val3.Equals(ref val4) &&
				val4.Equals(ref val5) &&
				val5.Equals(ref val6) &&
				val6.Equals(ref val7) &&
				val7.Equals(ref val8) &&
				val8.Equals(ref val9);
		}

		public QueueJobDataPak(QueueJobDataItem input)
		{
			val0 = input;
			val1 = input;
			val2 = input;
			val3 = input;
			val4 = input;
			val5 = input;
			val6 = input;
			val7 = input;
			val8 = input;
			val9 = input;
		}
	}
	public struct QueueJobDataItem
	{
		public int writerJobId, parallelIndex, jobInnerLoopIndex;
		public bool Equals(ref QueueJobDataItem other)
		{
			return writerJobId == other.writerJobId && parallelIndex == other.parallelIndex && jobInnerLoopIndex == other.jobInnerLoopIndex;
		}
	}





}
