using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;


namespace MergeSort
{
	class Program
	{

		static void Main(string[] args)
		{
			List<int> unsorted = new List<int>();
			List<int> sorted;

			Stopwatch stopWatch = new Stopwatch();



			Random random = new Random();

			Console.WriteLine("Original array elements:");
			for (int i = 0; i < 10; i++)
			{
				unsorted.Add(random.Next(0, 100));
				Console.Write(unsorted[i] + " ");
			}
			Console.WriteLine();

			stopWatch.Start();

			MergeSort mergesort = new MergeSort(unsorted);

			TimeSpan ts = stopWatch.Elapsed;

			string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
			ts.Hours, ts.Minutes, ts.Seconds,
			ts.Milliseconds / 10);
			Console.WriteLine("\nRunTime: " + elapsedTime + "\n");



			sorted = mergesort.Mresult;

			Console.WriteLine("Sorted array elements: ");
			foreach (int x in sorted)
			{
				Console.Write(x + " ");
			}

			//for (int i = 0; i-1 < sorted.Count; i++)
			//{
			//	if (sorted[i] > sorted[i+1])
			//	{
			//		Console.Write("\nNOT SORTED");
			//		break;
			//	}
			//}
			Console.Write("\n");
		}

		class MergeSort
		{
			List<int> Mnums = new List<int>();
			public List<int> Mresult = new List<int>();
			//Thread mergeThread;

			MergeSort()
			{
			}

			public MergeSort(List<int> nums)
			{
				Mnums = nums;
				MSort();
				//mergeThread = new Thread(() => MSort());
				//mergeThread.Start();
				//mergeThread.IsBackground = true;
			}


			public List<int> merge(List<int> l, List<int> r)
			{
				//Console.Write("Working Thread: " + Thread.CurrentThread.ManagedThreadId + "\n");

				List<int> result = new List<int>();

				//Object thisLock = new Object();

				//lock (thisLock)
				//{
				//Console.Write("\nLocked\n");
				while (l.Count > 0 || r.Count > 0)
				{
					if (l.Count > 0 && r.Count > 0)
					{
						if (l.First() <= r.First())
						{

							result.Add(l.First());
							l.Remove(l.First());

						}
						else
						{

							result.Add(r.First());
							r.Remove(r.First());

						}
					}
					else if (l.Count > 0)
					{

						result.Add(l.First());
						l.Remove(l.First());

					}
					else if (r.Count > 0)
					{

						result.Add(r.First());
						r.Remove(r.First());

					}
				}
				//Console.Write("\nUnlocked\n");
				//}
				//foreach (int num in result)
				//{
				//	//Console.Write(num + " ");
				//}
				//lock (thisLock)
				//{
				//	if (result.Count > 1)
				//	{
				//		for(int i = 0; i< result.Count-1;i++)
				//		{

				//			if (result[i] > result[i + 1])
				//			{
				//			}
				//		}
				//	}
				//}
				return result;
			}

			public void MSort()
			{
				Mresult = Sort(Mnums);
			}

			//public List<int> Sort(Thread worker, List<int> unsorted)
			public List<int> Sort(List<int> unsorted)
			{
				//Object thisLock = new Object();
				//List<int> done = new List<int>();

				//lock (thisLock)
				//{
				if (unsorted.Count <= 1)
				{
					Console.Write(unsorted.First() + "\n");
					//Console.Write("\nThread " + Thread.CurrentThread.ManagedThreadId + " terminating\n");
					return unsorted;
				}
				//}

				List<int> l = new List<int>();
				List<int> r = new List<int>();


				//lock (thisLock)
				//{

				//int m = ;

				//Console.Write("\nLocked\n");
				for (int i = 0; i < unsorted.Count / 2; i++)
				{
					//Console.Write(unsorted.Count+"\n");
					l.Add(unsorted[i]);
				}
				for (int i = unsorted.Count / 2; i < unsorted.Count; i++)
				{
					r.Add(unsorted[i]);
				}
				//Console.Write("\nUnlocked\n");
				//}


				//Thread lThread = new Thread(() => Sort(l));
				//Thread rThread = new Thread(() => Sort(r));
				l = Sort(l);
				r = Sort(r);

				//lThread.Start();		
				//rThread.Start();

				//lThread.Join();
				//lock (thisLock)
				//{
				//	if (l.Count > 1 && l[0] > l[1])
				//	{
				//		//////
				//	}
				//}
				//rThread.Join();



				return merge(l, r);
			}

		}
	}
}
