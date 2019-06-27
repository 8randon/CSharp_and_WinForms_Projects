using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CPTS321_HW_2_Brandon_Townsend
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
		private void Form1_Load(object sender, EventArgs e)
		{
			Random rand = new Random();
			System.Collections.Generic.List<int> list = new List<int>();
			Dictionary<int, int> numbers = new Dictionary<int, int>();
			StringBuilder output = new StringBuilder("1. Number of distinct integers: ");

			int listLength = 10000;
			int valBound = 20000;

			int numUnique = 0;

			int increm = 0;

			// 1.

			for (int i = 0; i < listLength; i++)
			{
				// Add the desired number of random integers to your list
				list.Add(rand.Next(valBound));

				// Chec to see if it is already in the dictionary
				if (!numbers.ContainsKey(list[i]))
				{
					// If not, go ahead and add it
					numbers.Add(list[i], list[i]);
				}
			}

			// The total number of dictionary entries will give the number of distinct integers in the list
			output.Append(numbers.Count + "\r\n Time complexity: O(n). The for loop will always iterate n times, where n is the desired " +
				"list length, giving O(n) time. The Add() methods are both O(1) operations. Searching the dictionary is also an O(1) operation." +
				"When combined:  O(n) + O(1) + O(1) + O(1) ~= O(n).\r\n");

			// 2.
			// O(n^2)

			for (int j = 0; j < valBound; j++)
			{
				// For each possible value of a list item...
				for (int x = 0; x < listLength; x++)
				{
					//... check to see if there are any items with that value in the list.
					if (j == list[x])
					{
						// If so, increase the number of distinct integers by one and move on to the next
						// possible value.
						numUnique++;
						break;
					}
				}
			}

			output.Append("\r\n2. O(1) Storage: Number of distinct integers: " + numUnique + "\r\n");


			// 3.

			list.Sort();

			numUnique = 0;

			// This algorithm essentially counts the total number of sequences in the sorted list.
			// The total number of sequences is the number of distinct integers in the sorted list.
			for (int s = 0; s < listLength-1; s++)
			{
				increm = 0;

				// Once a new sequence is found, it counts the number of copies / the length of the sequence...
				while ((s+1+increm < listLength) && list[s] == list[s + 1 + increm])
				{
					// ( ^ This conditional checks against the next item)

					// Length of the sequence - 1 ( <-- the -1 accounts for loop-driven incrementation)
					increm++;
				}

				// Increment the number of distinct integers
				numUnique++;

				//... and jumps past them all to the next sequence
				s += increm;
			}

			// Checking the last item vs the previous item
			if (list[listLength - 1] != list[listLength - 2])
			{
				// Increment the number of distinct integers if the last value is unique
				numUnique++;
			}

			output.Append("\r\n3. Sorted, O(n): Number of distinct integers: " + numUnique + "\r\n");

			textBox1.Text = output.ToString();
		}
	}
}
