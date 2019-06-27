// Brandon Townsend
// CS321
// HW4
// Shenanigans. All Shenanigans...

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSTtreeAbs
{

	public abstract class BinTree<T>
	{
		public abstract void Insert(T val); // Insert new item of type T
		public abstract bool Contains(T val); // Returns true if item is in tree
		public abstract void InOrder(); // Print elements in tree inorder traversal
		public abstract void PreOrder(); //pre order print
		public abstract void PostOrder(); // post order print
	}
	public class BSTtree<T> : BinTree<T> where T : IComparable<T>
	{

		// ~~~~~~~~~~~~Useful Functions For Testing Operators~~~~~~~~~~~~
		/*public int Tester(T val1, T val2)
		{
			return val1.CompareTo(val2);
		}
		public bool Tester2(T val1, T val2)
		{
			BSTnode b1 = new BSTnode(val1,0);
			BSTnode b2 = new BSTnode(val2, 0);
			return b1>b2;
		}*/

		public class BSTnode : IComparable<BSTnode>
		{
			public T value;
			public int depth;
			public BSTnode left;
			public BSTnode right;
			public bool leafL;
			public bool leafR;

			BSTnode(T val)
				{
					value = val;
					depth = 0;
					left = null;
					right = null;
					leafL = true;
					leafR = true;
			}

			public BSTnode(T val, int d)
			{
					value = val;
					depth = d;
					left = null;
					right = null;
					leafL = true;
					leafR = true;
			}

			//operator overloads ------------------------------------------------------
			public int CompareTo(BSTnode other)
			{
				if (other == null) return 1;

				return this.value.CompareTo(other.value);
			}

			public static bool operator >(BSTnode op1, BSTnode op2)
			{
				return op1.CompareTo(op2) == 1;
			}
			public static bool operator <(BSTnode op1, BSTnode op2)
			{
				return op1.CompareTo(op2) == -1;
			}
			public static bool operator >=(BSTnode op1, BSTnode op2)
			{
				return op1.CompareTo(op2) >= 0;
			}
			public static bool operator <=(BSTnode op1, BSTnode op2)
			{
				return op1.CompareTo(op2) <= 0;
			}
			public static bool operator ==(BSTnode op1, BSTnode op2)
			{

				if (Object.ReferenceEquals(op1, op2))
				{
					return true;
				}
				else { return false; }
			}
			public static bool operator !=(BSTnode op1, BSTnode op2)
			{
				return op1.CompareTo(op2) != 1;
			}

			public override bool Equals(Object obj)
			{
				BSTnode compare = obj as BSTnode;

				//Check for null and compare run-time types.

				if ((compare == null) || !this.GetType().Equals(compare.GetType()))
				{
					return false;
				}
				else
				{
					BSTnode x = (BSTnode)compare;
					return this == x;
				}
			}
			//------------------------------------------------------
			//General Functions
				public T GiveValue()
				{
					return value;
				}

				public int GiveDepth()
				{
					return depth;
				}

				public bool GiveLeafR()
				{
					return leafR;
				}
				public bool GiveLeafL()
				{
					return leafL;
				}
		}

			public BSTnode head;
			public int size;
			public int depthTracker;
			public int height;

			public BSTtree(T[] valList)
			{

				T x = default(T);
				
				head = new BSTnode(x,0);

				//adding everything to the BST
				for (int i = 0; i < valList.Length; i++)
				{
					Insert(valList[i]);
				}
			}

			public int Count()
			{
				return size;
			}

			public void depth() { Console.WriteLine("Depth: {0}", height); }

			public int MinLevels()
			{
				int numNodes = Count();
				int lvls = 0;
				int counter = 0;

				if (numNodes == 0) { return 0; }

				// counter is incremented every iteration by the number of nodes in the next
				// level of a full BST, which is twice the number of nodes that were in the last level.
				// Until this value is greater than the number of nodes in the actual BST, lvls is 
				// incremented by 1 every iteration.
				for (int i = 1; counter < numNodes; i *= 2)
				{
					counter += i;
					lvls++;
				}

				return lvls;
			}

			BSTnode AddW(BSTnode currNode, T NewN)
			{
				BSTnode temp = new BSTnode(NewN, depthTracker);

				// once we reach a new spot for a leaf...
				
				//inserting left
				if (currNode > temp)
				{
					depthTracker++; //we gotta go deeper

					if (currNode.GiveLeafL() == true) //checks to see if it is a leaf to the left
				{
						currNode.left = new BSTnode(NewN, depthTracker);
						currNode.leafL = false;
						size++;
						if (height < depthTracker) { height = depthTracker; } // updates height of tree
					}
					else
					{
						AddW(currNode.left, NewN);
					}
				}

				//same as above, but to the right
				else if (currNode < temp)
				{
					depthTracker++; // down the rabbit hole we go

					if (currNode.GiveLeafR() == true)//checks to see if it is a leaf to the right
					{
						currNode.right = new BSTnode(NewN, depthTracker);
						currNode.leafR = false;
						size++;
						if (height < depthTracker) { height = depthTracker; } // updates height of tree
					}
					else
					{
						AddW(currNode.right, NewN);
					}
				}
				// if it runs into a duplicate
				else
				{
					Console.WriteLine("Double detected: {0}\n", NewN);
				}

				return currNode;
			}

			//wrapper function
			public override void Insert(T val)
			{
				depthTracker = 0; // starting at head/root, so starting depth is 0
				if (size == 0) { head = new BSTnode(val, 0); size++; }
				else { head = AddW(head, val); } //starts out with tree head

			}
			//Tree InOrder() wrapper
			public override void InOrder()
			{
				Console.WriteLine("InOrder...");
				bstTraversW(head);
				Console.WriteLine("\n");
			}
			// Traversing the tree
			void bstTraversW(BSTnode thing)
			{
				if (thing == null)
				{
					return;
				}

				bstTraversW(thing.left);
				Console.Write("{0} ", thing.value);
				bstTraversW(thing.right);
			}

		public override bool Contains(T val)
		{
			bool found = ContainsW(head, val);

			if (found)
			{
				Console.WriteLine("Your value was found!");
			}
			else
			{
				Console.WriteLine("It wasn't there!");
			}
			

			return found;
		}

		private bool ContainsW(BSTnode thing, T val)
		{

			BSTnode findit = new BSTnode(val, 0);

			if (thing == null) // if it hits an end
			{
				return false;
			}
			else if (thing > findit)
			{
				return ContainsW(thing.left, val); // look left
			}

			else if (thing < findit)
			{
				return ContainsW(thing.right, val); // look right
			}
			else { return true; } // otherwise, it's been found
		}

		//PostOrder() wrapper
		public override void PostOrder()
		{
			Console.WriteLine("Ordering by post... (PostOrder)");
			PostOrderW(head);
			Console.WriteLine("\n");
		}
		// Traversing the tree
		void PostOrderW(BSTnode thing)
		{
			if (thing == null)
			{
				return;
			}

			PostOrderW(thing.left);
			PostOrderW(thing.right);
			Console.Write("{0} ", thing.value);
		}

		//PreOrder() wrapper
		public override void PreOrder()
		{
			Console.WriteLine("Deciding what to order... (PreOrder)");
			PreOrderW(head);
			Console.WriteLine("\n");
		}
		void PreOrderW(BSTnode thing)
		{
			if (thing == null)
			{
				return;
			}

			Console.Write("{0} ", thing.value);
			PreOrderW(thing.right);
			PreOrderW(thing.left);
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Please enter a string of numbers! \n");
			string readnums = Console.ReadLine();
			Console.Write("\n");
			string[] input = readnums.Split(' ');
			int[] TArray = new int[input.Length]; 


			//takes the split input and turns the individual strings into ints
			for (int i = 0; i < input.Length; i++)
			{
				TArray[i] = Int32.Parse(input[i]);
			}

			BSTtree<int> Newbst = new BSTtree<int>(TArray);

			//BST Stats printed after intertions are complete.
			Newbst.InOrder();
			Newbst.PreOrder();
			Newbst.PostOrder();
			Console.WriteLine("Number of nodes: {0}", Newbst.Count());
			Console.WriteLine("BST minimum # of levels: {0}", Newbst.MinLevels());
			Newbst.depth();

			Newbst.Contains(3);

			Console.WriteLine("  ^______________^");
			Console.WriteLine(" %                &");
			Console.WriteLine("%  ||          ||   &");
			Console.WriteLine("%  ||          ||   &");
			Console.WriteLine("%      ,     ,       &");
			Console.WriteLine(" %     [_____]      &");
			Console.WriteLine("  % |)  /|         &");
			Console.WriteLine("    | `/ |       &");
			Console.WriteLine("   % &   /       &");
			return;
		}
	}

}

	

