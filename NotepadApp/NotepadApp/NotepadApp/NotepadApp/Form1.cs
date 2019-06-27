using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotepadApp
{
	//Custom Class which inherits from TextReader. For use in loading function calls in
	//'loadFibonacci...' events.
	public class FibonacciTextReader : System.IO.TextReader
	{

		private int lineLimit = 0;
		private int curRun = 0;
		private System.Numerics.BigInteger cur = 0; // using BigIintegers because values get very large, very quick
		private System.Numerics.BigInteger prev = 0;

		public FibonacciTextReader(int limit) // constructor; number of desired runs is defined
		{
			lineLimit = limit;
		}

		public override string ReadToEnd()
		{
			StringBuilder text = new StringBuilder();
			bool end = false;
			string temp = "";

			//While the end of the calculation has not been met...
			while (!end)
			{
				//read in the next "line"
				temp = ReadLine();

				//temp is null, that means there are no more "lines" to read
				if (temp != null)
				{
					//if we are not at the end yet, add the new "line" to the net text to load in,
					//along with syntax
					text.Append(curRun + ": " + temp + "\r\n");
				}
				else
				{
					//otherwise, we are at the end, so we set 'end' to 'true'
					end = true;
				}
			}

			//once we're done, we return the body of text
			return text.ToString();
		}

		//overridden ReadLine() method.
		//calculates next value in series and returns it as a string.
		//returns null when curRun >= the limit defined in the constructor
		public override string ReadLine()
		{
			if (curRun < lineLimit)
			{
				//switch statement to handle first two special cases in the series
				switch (curRun)
				{
					case 0:
						curRun++;
						return 0.ToString();
					case 1:
						curRun++;
						cur = 1;
						return 1.ToString();
					default: //for the other run numbers, calculate the next number in the sequence
						System.Numerics.BigInteger temp = prev;
						prev = cur;
						cur += temp;
						curRun++;
						return cur.ToString(); // returns the next number as a string
				}
			}
			else
			{ // returns null when the defined number of numbers have been calculated
				return null;
			}
		}
	}
	public partial class Form1 : Form
	{
		public Form1()
		{ // handling events
			InitializeComponent();
			this.saveToFileToolStripMenuItem.Click += new System.EventHandler(this.saveToFileToolStripMenuItem_Click);
			this.loadFibonacciNumbersfirst50ToolStripMenuItem.Click += new System.EventHandler(this.loadFibonacciNumbersfirst50ToolStripMenuItem_Click);
			this.loadFibonacciNumbersfirst100ToolStripMenuItem.Click += new System.EventHandler(this.loadFibonacciNumbersfirst100ToolStripMenuItem_Click);
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{

		}

		//Reads all text from file and sets it as the text in the textbox
		//Will also accept out FibonacciTextReader class
		private void LoadText(System.IO.TextReader sr)
		{
			string numbs = sr.ReadToEnd();
			textBox1.Text = numbs;
		}


		private void saveToFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//partial body code from: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.savefiledialog?redirectedfrom=MSDN&view=netframework-4.7.2
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();

			saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
			saveFileDialog1.FilterIndex = 2;
			saveFileDialog1.RestoreDirectory = true;
			// just a suggested filename:
			saveFileDialog1.FileName = "Fibonachos.txt";

			//checks to see if dialog opened correctly
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{ // if it does, write the text box text to the file
				using (System.IO.StreamWriter writer = new System.IO.StreamWriter(saveFileDialog1.FileName))
				{
					writer.WriteLine(textBox1.Text);
					writer.Close();
				}
			}
		}

		private void loadFromFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string fileName = null;

			using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
			{
				openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
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
				System.IO.StreamReader newtext = new System.IO.StreamReader(fileName);
				LoadText(newtext);
			}
		}

		//loads in first 50 numbers of Fibonacci sequence
		private void loadFibonacciNumbersfirst50ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FibonacciTextReader LoadFifty = new FibonacciTextReader(50);

			LoadText(LoadFifty);
		}

		//loads in first 100 numbers of Fibonacci sequence
		private void loadFibonacciNumbersfirst100ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FibonacciTextReader LoadHundred = new FibonacciTextReader(100);

			LoadText(LoadHundred);
		}
	}
}
