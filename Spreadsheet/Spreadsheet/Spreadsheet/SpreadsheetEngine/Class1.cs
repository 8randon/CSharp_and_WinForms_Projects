// Brandon Townsend

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using System.Xml;

// Expression Tree
namespace ExpTree
{
	public class ExpTree
	{
		public string Expression { get; set; }

		[Serializable]
		public class Node
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
				//evautes subnodes in tree
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

		public class Var : Node // Variable node subclass
		{
			public double Value { get; set; }
			public string Name { get; set; }

			public Var(string n)
			{
				Name = n;
				Value = 0.00;
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
		Dictionary<string, double> vars = new Dictionary<string, double>();
		List<Var> regi = new List<Var>();

		public List<Var> GetRegi()
		{
			return regi;
		}

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
				head = Build(Shunt(this.Expression));
			}

			Console.WriteLine("Current expression evaluation: " + Expression + " = " + head.eval() + "\n");
		}

		public double Evald()
		{
			if (head == null)
			{
				head = Build(Shunt(this.Expression));
			}

			return head.eval();
		}

		//converts to RPN w/ shunting yard alg.
		public string Shunt(string exp)
		{
			Stack<char> yard = new Stack<char>();
			Queue<char> outp = new Queue<char>();
			string result = "";
			int count = 0;

			for (int i = 0; i < exp.Length; i++)
			{
				//tests for variable
				if (Regex.Match(exp[i].ToString(), @"([A-Za-z])").Success && i+1 < exp.Length && Regex.Match(exp[i+1].ToString(), @"(^\d*$)").Success)
				{
					outp.Enqueue(exp[i]);
					while (i + 1 < exp.Length && Regex.Match(exp[i + 1].ToString(), @"(^\d*$)").Success)
					{
						i++;
						outp.Enqueue(exp[i]);
					}
					
					outp.Enqueue(' ');
				}
				//testing for double
				else if (Regex.Match(exp[i].ToString(), @"(^\d+\.?\d*$)").Success)
				{
					while (i < exp.Length && Regex.Match(exp[i].ToString(), @"(^\d+\.?\d*$)").Success)
					{
						outp.Enqueue(exp[i]);
						i++;
					}

					i--;

					outp.Enqueue(' ');
				}
				else if (exp[i] != '(' && exp[i] != ')')//Catches supported arithmetic operators
				{
					switch (exp[i])//precedence handling in yard
					{
						case '+':
						case '-':
							while (yard.Count > 0 && (yard.Peek() == '*' || yard.Peek() == '/' || yard.Peek() == '+' || yard.Peek() == '-'))
							{
								outp.Enqueue(yard.Pop());
								outp.Enqueue(' ');
							}
							break;
						case '*':
						case '/':
							while (yard.Count > 0 && (yard.Peek() == '*' || yard.Peek() == '/'))
							{
								outp.Enqueue(yard.Pop());
								outp.Enqueue(' ');
							}
							break;
					}
					yard.Push(exp[i]);

				}
				else if (exp[i] == '(')
				{
					yard.Push(exp[i]);
				}
				//keep popping until the other '(' is hit
				else if (exp[i] == ')')
				{
					while (yard.Count() > 0 && yard.Peek() != '(')
					{
						outp.Enqueue(yard.Pop());
						outp.Enqueue(' ');
					}

					//pop of the ')'
					if (yard.Count() != 0)
					{
						yard.Pop();
					}
					else
					{
						//error handling later? Not likely. Not the job of the expression tree.
					}
				}
			}

			//clearing out the rest of the yard
			if (yard.Count() != 0)
			{
				while (yard.Count() > 0)
				{
					outp.Enqueue(yard.Pop());
					outp.Enqueue(' ');
				}
			}

			count = outp.Count;

			for (int j = 0; j < count-1; j++)
			{
				result += outp.Dequeue().ToString();
			}

			Console.WriteLine(result);

			return result;
		}

		// Builds new expression tree from current Expression
		Node Build(string exp)
		{	
			Stack<Node> elems = new Stack<Node>();

			string[] parts = exp.Split(' ');

			foreach (string part in parts)
			{
				if (part != "")
				{
					switch (part[0])
					{
						case '+':
						case '-':
						case '*':
						case '/': // if it's an operator, take the top two nodes and set them as children to a new node
							Op newnode = new Op(part[0]);
							newnode.Right = elems.Pop();
							newnode.Left = elems.Pop();
							elems.Push(newnode); //then push the new node tot he stack
							break;
						default: //testing for doubles and variables
							if (Regex.Match(part, @"(^\d+\.?\d*$)").Success)
							{
								//returns a value node
								elems.Push(new Val(part));
							}
							else
							{
								//Whatever non-operator, non-number they entered is now a variable.
								//They made this bed, now they lie in it. Not the job of the epression tree to check variable semantics.

								//It is added to the variable registry
								vars.Add(part, 0.00);
								regi.Add(new Var(part));
								//then returned.
								elems.Push(regi.Last());
							}
							break;
					}
				}	
			}
			return elems.Pop();
		}

		// Builds new expression tree from new input expression
		public void Rebuild(string newExp)
		{
			Expression = newExp;

			//Clears old variables from variable registry
			vars.Clear();
			regi.Clear();

			head = Build(Shunt(Expression));
		}

		//Lists current variables and their values
		public void ListVars()
		{
			// Build expression tree if it has not already been built
			if (head == null)
			{
				head = Build(Shunt(this.Expression));
			}

			if (vars.Count != 0) // Checks to see if there are any variables in registry
			{
				Console.WriteLine("\nRegistered variables: ");
				foreach (KeyValuePair<string, double> varable in vars)
				{
					Console.WriteLine(varable);
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
			if(head == null)
			{
				head = Build(Shunt(this.Expression));
			}

			bool found = false;

			// Searches list for all instances of input variable name...
			
			if (vars.ContainsKey(vn))
			{
				//...and sets them equal to the new value.
				found = true;
				vars[vn] = newval;
			
				foreach (Var var in regi)
				{
					if (var.Name == vn)
					{
						var.Value = newval;
						Console.WriteLine("New value set.\n");
					}
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


// Spreadsheet Engine
namespace CptS321
{
	[Serializable]
	public class Spreadsheet
	{
		//Helper class. Circumvents permission restrictions and requirements for stream precessing of objects
		[Serializable()]
		public class CellProps
		{
			[System.Xml.Serialization.XmlElement("Ri")]
			public int Ri { get; set; }
			[System.Xml.Serialization.XmlElement("Ci")]
			public int Ci { get; set; }
			[System.Xml.Serialization.XmlElement("Value")]
			public string Value { get; set; }
			[System.Xml.Serialization.XmlElement("Text")]
			public string Text { get; set; }

			CellProps()
			{
				Ri = 0;
				Ci = 0;
				Value = "";
				Text = "";
			}
			public CellProps(int ri, int ci, string value, string text)
			{
				Ri = ri;
				Ci = ci;
				Value = value;
				Text = text;
			}
		}

		//custom derived class for Spreadsheet
		[Serializable()]
		private class SCell : Cell
		{
			//XmlSerializer xs;
			SCell()
			{
				RowIndex = 26;
				ColumnIndex = 26;
				//xs = new XmlSerializer(typeof(List<SCell>));
			}

			public SCell(int RowInd, int ColumnInd)
			{
				RowIndex = RowInd;
				ColumnIndex = ColumnInd;
			}

			public ExpTree.ExpTree Eqn = new ExpTree.ExpTree("");

			public void Serialize(FileStream data)
			{
				//xs.Serialize(data, this);
			}

			//Defines response behavior of listening cells to when a referenced cell's value changes.
			//Checks to see if referenced cell is valid.
			public void OnAlertCellValueChange(object sender, PropertyChangedEventArgs e)
			{
				SCell target = (SCell)sender;

				//If it is, then test to see if the value is a double. If so, then update the eqn variables
				if (Double.TryParse(target.Value, out double num))
				{
					Eqn.SetVar(((char)(target.ColumnIndex + 65)).ToString() + (target.RowIndex + 1).ToString(), num);
					this.Value = Eqn.Evald().ToString();
				}
				else //otherwise, it is invalid
				{
					this.Value = "#REFF!";
				}
			}
		}

		SCell[,] SpreadS;
		XmlSerializer xs;
		List<int[]> ChangedCells;
		List<CellProps> cl;


		private readonly int nRows;
		public int RowCount
		{
			get { return nRows; }
		}

		private readonly int nColumns;
		public int ColumnCount
		{
			get { return nColumns; }
		}

		public event PropertyChangedEventHandler CellPropertyChanged;

		public void Serialize()
		{
			//helper list
			cl = new List<CellProps>();

			SaveFileDialog saveFileDialog1 = new SaveFileDialog();

			saveFileDialog1.Filter = "Xml files (*.Xml)|*.Xml|All files (*.*)|*.*";
			saveFileDialog1.FilterIndex = 2;
			saveFileDialog1.RestoreDirectory = true;
			// just a suggested filename:
			saveFileDialog1.FileName = "Spreadsheet.Xml";

			//write each set of cell data from helper list
			foreach (int[] location in ChangedCells)
			{
				CellProps CellProps = new CellProps(location[0], location[1], SpreadS[location[0], location[1]].Value, SpreadS[location[0], location[1]].Text);
				cl.Add(CellProps);
			}
			//checks to see if dialog opened correctly
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{ // if it does, write the text box text to the file
				using (System.IO.StreamWriter writer = new System.IO.StreamWriter(saveFileDialog1.FileName))
				{
					xs.Serialize(writer, cl);
					writer.Close();
				}
			}
		}
		public void Deserialize()
		{
			string fileName = null;

			using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
			{
				openFileDialog1.Filter = "Xml files (*.Xml)|*.Xml|All files (*.*)|*.*";
				openFileDialog1.FilterIndex = 2;
				openFileDialog1.RestoreDirectory = true;

				//checks to see if dialog opened correctly
				if (openFileDialog1.ShowDialog() == DialogResult.OK)
				{
					fileName = openFileDialog1.FileName;
				}
			}

			//checks to see if filename is valid
			if (fileName != null)
			{ // if it is, go ahead and load in the file text

				//helper list
				cl = new List<CellProps>();

				//read in cell data from xml file and put into helper list
				using (XmlReader reader = XmlReader.Create(fileName))
				{
					cl = (List<CellProps>)xs.Deserialize(reader);
				}

				//reset all the cells
				for (int i = 0; i < nRows; i++)
				{
					for (int j = 0; j < nColumns; j++)
					{
						SpreadS[i, j].Text = i.ToString() + " " + j.ToString();
						SpreadS[i, j].Value = "";
					}
				}

				//edit cells specified by helper list
				foreach (CellProps cell in cl)
				{
					SpreadS[cell.Ri, cell.Ci].RowIndex = cell.Ri;
					SpreadS[cell.Ri, cell.Ci].ColumnIndex = cell.Ci;
					SpreadS[cell.Ri, cell.Ci].Text = cell.Text;
					//Spreadsheet cell value will automatically update
				}
			}

		}

		//Defining Response behaviour to CellPropertyChanged event.
		//If the cell Text changes, then the cell is recalculated.
		private void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
		{

			SCell target = (SCell)sender;
			int[] newcell = { target.RowIndex, target.ColumnIndex };
			bool flag = false;

			foreach (int[] location in ChangedCells)
			{
				flag = location.SequenceEqual(newcell);
				if (flag == true)
				{
					break;
				}
			}

			if (flag == false)
			{
				ChangedCells.Add(newcell);
			}
			
			

			string temp = "";

			//if the target text has been set before and starts with '='...
			if (e.PropertyName == "Text" && target.Text != null && target.Text != "" && target.Text[0] == '=')
			{
				//just grab the rest of the string...
				for (int i = 1; i < target.Text.Length; i++)
				{
					temp += target.Text[i].ToString();
				}
				//... and set the target cell's value to it

				string[] parts = target.Eqn.Shunt(temp).Split(' ');
				bool equation = true;

				//Checking to see if it is a valid equation. looks for non-value, non-variable, and non-operator parts
				foreach (string part in parts)
				{
					if (equation == true &&  (Regex.Match(part, @"(^[+,\-,/,*]$)").Success || Regex.Match(part, @"(^\d+\.?\d*$)").Success || (Regex.Match(part, @"(^[A-Z{1}][\d+])").Success) && !Regex.Match(part, @"(\W)").Success))
					{
						equation = true;
					}
					else
					{
						equation = false;
					}
				}

				if (equation == true)
				{
					//prepare target:

					//Removing all subscriptions. Subscriptions to referenced cells are (re)added later.
					foreach (ExpTree.ExpTree.Var variable in target.Eqn.GetRegi())
					{
						int row = Int32.Parse(variable.Name[1].ToString()) - 1;
						int col = variable.Name[0] - 65;

						//unsubscribing to referenced cell's event:
						SpreadS[row, col].AlertValChange -= target.OnAlertCellValueChange;
					}

					//rebuilding target cell's equation
					target.Eqn.Rebuild(temp);

					//~*~* MaTh *~*~

					//target.Eqn.Expression = temp;
					target.Value = target.Eqn.Evald().ToString();

					//setting variables in target cell to values in respective cells, if there are any
					foreach (ExpTree.ExpTree.Var variable in target.Eqn.GetRegi())
					{
						int row = Int32.Parse(variable.Name.Substring(1))-1;
						int col = variable.Name[0] - 65;

						//subscribing to referenced cell's event for when just the value changes
						SpreadS[row, col].AlertValChange += target.OnAlertCellValueChange;

						//Checking to see if referenced cell is valid for an equation input
						if (SpreadS[row, col].Value == null || !IsEqn(SpreadS[row, col].Eqn.Expression) || !Double.TryParse(SpreadS[row, col].Value, out double num))
						{
							target.Value = "#REFF!";
						}
						else
						{
							target.Eqn.SetVar(variable.Name, Double.Parse(SpreadS[row, col].Value));
							target.Value = target.Eqn.Evald().ToString();
						}						
					}
				}
				else
				{
					target.Value = target.Text;
				}
			}
			//...otherwise just set the target cell's value to it
			else if(e.PropertyName == "Text")
			{
				if (Double.TryParse(target.Text, out double num))
				{
					target.Eqn.Expression = num.ToString();
				}
				target.Value=target.Text;
			}

			// finally, fire off CellPropertyChanged event
			CellPropertyChanged?.Invoke(target, e);
		}

		// Tests to see if input string is an equation or a double
		private bool IsEqn(string exp)
		{
			string temp = "";
			bool equation = true;

			//if the target text has been set before and starts with '='...
			if (exp != null && exp != "" && exp[0] == '=')
			{
				//just grab the rest of the string...
				for (int i = 1; i < exp.Length; i++)
				{
					temp += exp[i].ToString();
				}
				//... and set the target cell's value to it

				string[] parts = temp.Split(' ');

				//testing to see if it is a valid equation. looks for non-value, non-variable, and non-operator parts
				foreach (string part in parts)
				{
					if (equation == true && (Regex.Match(part, @"(^[+,\-,/,*]$)").Success || Regex.Match(part, @"(^\d+\.?\d*$)").Success || Regex.Match(part, @"(^[A-Z{1}][\d{1}]$)").Success))
					{
						equation = true;
					}
					else
					{
						equation = false;
					}
				}
			}
			else if (Double.TryParse(exp, out double num))
			{
				equation = true;
			}

			return equation;
		}

		Spreadsheet()
		{
			SpreadS = new SCell[26, 50];
			xs = new XmlSerializer(typeof(List<CellProps>));
			ChangedCells = new List<int[]>();
		}

		public Spreadsheet(int Rows, int Columns)
		{
			SpreadS = new SCell[Rows, Columns];
			nRows = Rows;
			nColumns = Columns;
			xs = new XmlSerializer(typeof(List<CellProps>));
			ChangedCells = new List<int[]>();

			//initialize and populate cells
			for (int i = 0; i < Rows; i++)
			{
				for (int j = 0; j < Columns; j++)
				{
					SpreadS[i, j] = new SCell(i, j) { Text = i.ToString() + " " + j.ToString() };

					//if PropertyChanged event is fired for this cell, call OnCellPropertyChanged is called
					//"Subscribing" to this cell's PropertyChanged and AlertValChange events
					SpreadS[i, j].PropertyChanged += this.OnCellPropertyChanged;
					SpreadS[i, j].AlertValChange += this.OnCellPropertyChanged;
				}
			}
		}

		//get that cell!
		public Cell GetCell(int row, int column)
		{
			if (row > nRows || column > nColumns)
			{
				return null;
			}
			else
			{
				return SpreadS[row, column];
			}
		}

		//demo function
		public void Demo()
		{
			int[,] randoms = new int[50,2];
			int tempi = 0;
			int tempj = 0;

			Random rand = new Random();

			for (int i = 0; i < 50; i++)
			{
				//generate random numbers
				tempi = rand.Next(RowCount);
				tempj = rand.Next(ColumnCount);

				//check list to see if these coordinates have already been generated
				for (int j = 0; j < i; j++)
				{
					//if they have, generate more and start the check progress over
					if (randoms[j, 0] == tempi && randoms[j,1] == tempj)
					{
						tempi = rand.Next(RowCount);
						tempj = rand.Next(ColumnCount);
						j = 0;
					}
				}

				//put the coordinates into the list
				randoms[i, 0] = tempi;
				randoms[i, 1] = tempj;

				//set text of the cell at the coordinates
				SpreadS[tempi, tempj].Text = "Hullabaloo";				
			}

			//demonstrating the '=' handling works
			for (int i = 0; i < RowCount; i++)
			{
				SpreadS[i, 1].Text = "This is cell B" + i.ToString();
				SpreadS[i, 0].Text = "=" + SpreadS[i, 1].Text;
			}
		}
	}

	[Serializable()]
	public abstract class Cell : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public event PropertyChangedEventHandler AlertValChange;


		public int RowIndex { get; internal set; }
		public int ColumnIndex { get; internal set; }
	
		protected string text { get; set; }
		//text property; handling new text being set
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				//only change the value if it's new
				if (!Equals(value, text))
				{
					text = value;
					//trigger event protocol!
					OnPropertyChanged("Text");
				}
			}
		}

		protected void OnPropertyChanged(string texT)
		{
			//using delegate stuff apparently? cool but need to research more...
			//fire off that event!
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(texT));
		}

		protected string value;
		public string Value
		{
			get
			{
				return value;
			}
			set
			{
				//only change the value if it's new
				if (!Equals(value, this.value))

				{
					this.value = value;
					//trigger event protocol!
					OnAlertValueChange("Value");
				}
			}
		}

		//For when the cell's value changes. Picked up by subscribed cells.
		protected void OnAlertValueChange(string vaL)
		{
			AlertValChange?.Invoke(this, new PropertyChangedEventArgs(vaL));
		}
	}
}
