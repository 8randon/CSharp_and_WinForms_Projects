using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ExpTree
{
	public class ExpTree
	{
		public string Expression { get; set; }

		class Node
		{
			virtual public double eval() { return 0.0; }
		}

		//~~~~~~~~~~~~ Node Subclasses ~~~~~~~~~~~

		class Op : Node  // Operator node sublclass
		{
			char value = '~';
			public Node Left { get; set; }
			public Node Right { get; set; }


			Op() { }

			public Op(char init)
			{
				value = init;
			}

			public override double eval()
			{
				switch (value)
				{
					case '+':
						return this.Left.eval() + this.Right.eval();
					case '-':
						return this.Left.eval() - this.Right.eval();
					case '*':
						return this.Left.eval() * this.Right.eval();
					case '/':
						return this.Left.eval() / this.Right.eval();
					default:
						return 0.0;
				}
			}
		}

		class Val : Node // Value node sublclass
		{
			double value = 0;

			Val() { }

			public Val(string init)
			{
				Double.TryParse(init, out value);
			}

			public override double eval()
			{
				return value;
			}
		}

		class Var : Node // Variable node subclass
		{
			public double Value { get; set; }
			public string Name { get; set; }

			public Var(string n)
			{
				Name = n;
				Value = 1.00;
			}

			public override double eval()
			{
				// ask for variable value
				return Value;
			}
		}

		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

		private Node head;

		// variable registry; keeps track of all variables in current expression
		List<Var> vars = new List<Var>();

		ExpTree()
		{
			Expression = "1+1";
		}

		public ExpTree(string exp)
		{
			Expression = exp;
		}

		// Evaluates current expression
		public void Eval()
		{
			// Build expression tree if it has not already been built
			if (head == null)
			{
				head = Build(this.Expression);
			}

			Console.WriteLine("Current expression evaluation: " + Expression + " = " + head.eval() + "\n");
		}

		// Builds new expression tree from current Expression
		Node Build(string exp)
		{

			for (int i = exp.Length - 1; i >= 0; i--)
			{
				//Catches supported operators
				switch (exp[i])
				{
					case '+':
					case '-':
					case '*':
					case '/':
						Op newnode = new Op(exp[i]);

						//Recurses through string. Since we found an operator, we know that
						//there are numbers on either side.
						newnode.Left = Build(exp.Substring(0, i));
						newnode.Right = Build(exp.Substring(i + 1));
						return newnode;
				}
			}

			// Tests if input is some sort of number
			if (Regex.Match(exp, @"(^\d+\.?\d*$)").Success)
			{
				//returns a value node
				return new Val(exp);
			}
			else
			{
				//Whatever non-operator, non-number they entered is now a variable.
				//They made this bed, now they lie in it.

				//It is added to the variable registry
				vars.Add(new Var(exp));

				//then returned.
				return vars.Last();
			}
		}

		// Builds new expression tree from new input expression
		public void Rebuild(string newExp)
		{
			Expression = newExp;

			//Clears old variables from variable registry
			vars.Clear();

			head = Build(Expression);
		}

		//Lists current variables and their values
		public void ListVars()
		{
			// Build expression tree if it has not already been built
			if (head == null)
			{
				head = Build(this.Expression);
			}

			if (vars.Count != 0) // Checks to see if there are any variables in registry
			{
				Console.WriteLine("\nRegistered variables: ");
				foreach (Var varable in vars)
				{
					Console.WriteLine(varable.Name + " = " + varable.Value + "\n");
				}
			}
			else
			{
				Console.WriteLine("No variables are currently registered.\n");
			}
		}

		//Set value of all instances of a variable
		public void SetVar(string vn, double newval)
		{
			

			bool found = false;

			// Searches list for all instances of input variable name...
			foreach(Var variable in vars)
			{
				if (Equals(variable.Name, vn))
				{
					//...and sets them equal to the new value.
					found = true;
					variable.Value = newval;
					Console.WriteLine("New value set.\n");
				}
			}

			// If the input variable is not in the registry...
			if (found == false)
			{
				// Then let em know
				Console.WriteLine("No variable named " + vn + " is found in expression.\n");
			}
		}
	}
}
