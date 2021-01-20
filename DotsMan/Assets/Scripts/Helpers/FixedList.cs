//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Unity.Collections;

//public unsafe struct FixedTest<TItem> where TItem : unmanaged
//{
//	public FixedList128<int> ptrList;

//	public FixedTest(Allocator allocator)
//	{
//		ptrList = new FixedList128<int>();
//		for (int i = 0; i < ptrList.Capacity; i++)
//		{
//			var queue = new NativeList<TItem>(allocator);
//			var ptr = Unity.Collections.LowLevel.Unsafe.NativeListUnsafeUtility.GetUnsafePtr(queue);
//			ptrList[i] = (int)ptr;

//			//ptrList[i]= //Unity.Collections.LowLevel.Unsafe.native
//		}
//	}

//	public NativeList<TItem>* Get(int i)
//	{
//		var ptr = (void*)ptrList[i];
//		return (NativeList<TItem>*)ptr;
//	}

//}

////public unsafe struct FixedList_8<T> where T : struct, Unity.Collections.INativeDisposable
////{
////	public int length;
////	public int capacity;

////	public fixed int test[128];
////}


////internal unsafe struct Buffer
////{
////	public fixed char fixedBuffer[128];
////}

////public struct FixedArray_8<T>
////{
////	public int capacity;

////	public ref T GetIndex(int i)
////	{
////		return ref v000;
////	}

////	public T v000;
////	public T v001;
////	public T v002;
////	public T v003;
////	public T v004;
////	public T v005;
////	public T v006;
////	public T v007;
////	public T v008;
////	public T v009;
////}

//public unsafe struct FixedTest128<T> where T : unmanaged
//{
//	public fixed int ptrs[128];
//}