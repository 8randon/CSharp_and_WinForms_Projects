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
            List<int> unsortedList = new List<int>();
			//number of threads
			int numbers = 2000;
			int[] unsortedThreadedList = new int[numbers];

			Stopwatch stopWatch1 = new Stopwatch();
			Stopwatch stopWatch2 = new Stopwatch();



			Random random = new Random();


			//generate random number list to sort
			Console.WriteLine("Original array elements:");
			for (int i = 0; i < numbers; i++)
			{
                int newVal = random.Next(0, numbers*2);
				unsorted.Add(newVal);
                unsortedList.Add(newVal);
				unsortedThreadedList[i] = newVal;

				//Uncomment to see unsorted numbers
						// Console.Write(unsorted[i] + " ");
			}

			Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

			stopWatch1.Start();
			MergeSort mergesort = new MergeSort(unsorted);
			TimeSpan ts = stopWatch1.Elapsed;

			string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
											ts.Hours, ts.Minutes, ts.Seconds,
											ts.Milliseconds / 10);
			Console.WriteLine("\nRunTime For Unthreaded: " + elapsedTime + "\n");

			sorted = mergesort.Mresult;

			Console.WriteLine("Sorted array elements: ");

			//Uncomment loop to see sorted numbers
			foreach (int x in sorted)
			{
				Console.Write(x + " ");
			}

			Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

			stopWatch2.Start();
            ThreadedMergeSort threadedMergesort = new ThreadedMergeSort(unsortedThreadedList);
            TimeSpan tsThreaded = stopWatch2.Elapsed;

            int[] threadedSorted = new int[unsortedThreadedList.Length];
            threadedSorted = threadedMergesort.Mnums;

			

			string ThreadedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
											tsThreaded.Hours, tsThreaded.Minutes, tsThreaded.Seconds,
											tsThreaded.Milliseconds / 10);

			Console.WriteLine("\nRunTime For threaded: " + ThreadedTime + "\n");

			Console.WriteLine("Threaded based Sorted array elements: ");

			//Uncomment loop to see sorted numbers
			for (int i = 0; i < threadedSorted.Length; i++)
			{
				Console.Write(threadedSorted[i] + " ");
			}

			Console.Write("\n\nApplication has finished, press any key to continue \n\n");
            Console.ReadKey();
		}

		class MergeSort
		{
			List<int> Mnums = new List<int>();
			public List<int> Mresult = new List<int>();

			MergeSort()
			{
			}

			public MergeSort(List<int> nums)
			{
				Mnums = nums;
				MSort();
			}


			public List<int> merge(List<int> l, List<int> r)
			{
				List<int> result = new List<int>();

				//While counters are still in list range
				while (l.Count > 0 || r.Count > 0)
				{
					//if both counters are above zero, continue to advance with both
					if (l.Count > 0 && r.Count > 0)
					{
						//sorting lists for merging
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
					else if (l.Count > 0) //if the left list was longer
					{

						result.Add(l.First());
						l.Remove(l.First());

					}
					else if (r.Count > 0) //if the right list was longer
					{

						result.Add(r.First());
						r.Remove(r.First());

					}
				}
				
				return result;
			}

			public void MSort()
			{
				Mresult = Sort(Mnums);
			}

			public List<int> Sort(List<int> unsorted)
			{
				//if the list is down to one element
				if (unsorted.Count <= 1)
				{
					return unsorted;
				}

				List<int> l = new List<int>();
				List<int> r = new List<int>();

				//create left list
				for (int i = 0; i < unsorted.Count / 2; i++)
				{
					l.Add(unsorted[i]);
				}
				//create right list
				for (int i = unsorted.Count / 2; i < unsorted.Count; i++)
				{
					r.Add(unsorted[i]);
				}

				//sort both
				l = Sort(l);
				r = Sort(r);

				//merge them
				return merge(l, r);
			}

		}
	}
}
