using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MergeSort
{
    class ThreadedMergeSort
    {
		public int[] Mnums;
		public int[] Mresult;

        ThreadedMergeSort()
        {
        }

        public ThreadedMergeSort(int[] nums)
        {
            Mnums = nums;
            MSort();
        }

		//same general merge algorithm but with arrays
        public void Merge(ref int[] nums, int l, int m, int r)
        {

            int[] temp = new int[nums.Length];
			int lom = m - 1;
			int pos = l;
			int num = r - l + 1;


            while (l<=lom && m<= r)
            {
                if (nums[l]<=nums[m])
                {
					temp[pos++] = nums[l++];
                }
                else
                {
					temp[pos++] = nums[m++];
                }
            }
			
			while (l <= lom)
			{
				temp[pos++] = nums[l++];
					
			}
			while (m <= r)
			{
				temp[pos++] = nums[m++];
				
			}
			for (int i = 0; i < num; i++)
			{
				nums[r] = temp[r];
				r--;
			}
		}

		public void MSort()
		{
			Sort(Mnums, 0, Mnums.Length - 1);
		}
        
		//same general sort algorithm but with arrays
        public void Sort(int[] nums, int l, int r)
        {
			int m;

            if (r > l)
            {
				m = (r + l) / 2;

				//create threads and assign delegates
				Thread lThread = new Thread(() => Sort(nums, l, m));
				Thread rThread = new Thread(() => Sort(nums, (m + 1), r));

				//start both threads
				lThread.Start();
				rThread.Start();

				//wait for left thread
				lThread.Join();
				//then wait for right thread
				rThread.Join();

				//then merge the two modified lists
				Merge(ref nums, l, (m + 1), r);
			}
        }
    }
}
