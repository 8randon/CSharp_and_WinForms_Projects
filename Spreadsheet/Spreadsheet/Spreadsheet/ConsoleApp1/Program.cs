using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	class Program
	{
		static void Main(string[] args)
		{
			ExpTree.ExpTree test = new ExpTree.ExpTree("(A3+5)*(E3/3)");
			Console.WriteLine(test.Shunt("(A3+5)*(E3/3)"));
		}
	}
}
