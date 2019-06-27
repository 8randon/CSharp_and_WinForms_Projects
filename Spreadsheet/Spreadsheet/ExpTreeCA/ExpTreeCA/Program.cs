using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpTreeCA
{
	class Program
	{
		static void Main(string[] args)
		{
			// Setting a sample equation with a variable.
			ExpTree.ExpTree demo = new ExpTree.ExpTree("1+2+A2");

			bool quit = false;
			string option;
			string val = "";
			double valint = 0;

			// Standard menu interface
			while (quit == false)
			{
				val = "";

				Console.WriteLine("Menu: (current expression: " + demo.Expression + ")\n 1: Enter expression\n2: Set variable\n3: Evaluate expression\n4: Quit\n5: List variables in current expression\n");

				option = Console.ReadLine();

				switch (option[0])
				{
					case '1': // Get new expression from user
						Console.WriteLine("Enter expression: ");
						val = Console.ReadLine();
						demo.Rebuild(val);
						Console.WriteLine("\n");
						break;

					case '2': // Get variable name and desired new value for variable
						Console.WriteLine("Enter variable name: ");
						val = Console.ReadLine();
						Console.WriteLine("\nEnter new variable value: ");

						// Variable value filtering is handled here for UX reasons.
						// By definition, SetVar() only accepts input of type double for the new value,
						// and since the value of a variable cannot be set here, SetVar() is the only 
						// way to change a variable's value.
						if (double.TryParse(Console.ReadLine(), out valint))
						{
							demo.SetVar(val, valint);
						}
						else
						{
							Console.WriteLine("Bad Input, setting " + val + "to default value of 0.00");
							demo.SetVar(val, 0.00);
						}
						break;

					case '3': // Evaluate the current expression
						demo.Eval();
						break;

					case '4':
						Console.WriteLine("Exiting...");
						quit = true;
						break;

					case '5':// List variables in current expression and their respective values
						demo.ListVars();
						break;

					default:
						Console.WriteLine("\nInvalid Input\n");
						break;

				}
			}
		}
	}
}
