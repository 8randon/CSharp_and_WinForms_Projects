//Brandon Townsend

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Spreadsheet
{
	public partial class Form1 : Form
	{
		CptS321.Spreadsheet Sheet;
		CptS321.Cell selectedCell;

		public Form1()
		{
			//setting datagrid properties

			InitializeComponent();
			dataGridView1.ColumnCount = 26;
			dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

			dataGridView1.RowCount = 50;
			dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;

			// default header style for now

			//generate column names
			for (int i = 0; i < dataGridView1.ColumnCount; i++)
			{
				dataGridView1.Columns[i].Name = Convert.ToChar(65 + i).ToString();
			}

			//generate row headers
			for (int i = 1; i <= dataGridView1.RowCount; i++)
			{
				dataGridView1.Rows[i - 1].HeaderCell.Value = (i).ToString();
			}

			Sheet = new CptS321.Spreadsheet(dataGridView1.RowCount, dataGridView1.ColumnCount);

			//if CellPropertyChanged event is fired, UpdateCell is called
			//"Subscribing" to this Spreadsheet's CellPropertyChanged event
			Sheet.CellPropertyChanged += UpdateCell;
			textBox1.LostFocus += textBoxFocusLost;
			textBox1.GotFocus += textBoxFocusGot;
			selectedCell = Sheet.GetCell(0, 0);
		}

		public void UpdateCell(object sender, PropertyChangedEventArgs e)
		{
			CptS321.Cell target = (CptS321.Cell)sender;

			//updates GUI cell with value of corresponding Spreadsheet cell
			dataGridView1.Rows[target.RowIndex].Cells[target.ColumnIndex].Value = target.Value;
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			//lord knows i tried to get this to work so everything is in the constructor for now
		}

		private void button1_Click(object sender, EventArgs e)
		{
			// demo demo who wants a demo
			Sheet.Demo();
		}

		//When the user clicks away from the text box, either set it to the new cell's text or "" if it is empty
		private void textBoxFocusLost(object sender, EventArgs e)
		{
			TextBox active = (TextBox)sender;

			if (selectedCell != null)
			{
				selectedCell.Text = active.Text;
			}
			else
			{
				textBox1.Text = "";
			}
		}

		//When user clicks on the text box, update the selected cell. Might be redundant.
		private void textBoxFocusGot(object sender, EventArgs e)
		{
			TextBox active = (TextBox)sender;

			Int32 selectedCellCount =
				dataGridView1.GetCellCount(DataGridViewElementStates.Selected);

			if (selectedCellCount == 1 && dataGridView1.CurrentCell.Value != null)
			{
				selectedCell = Sheet.GetCell(dataGridView1.SelectedCells[0].RowIndex, dataGridView1.SelectedCells[0].ColumnIndex);
			}
		}

		//When user clicks on a cell, selectedcell is updated and text box text is updated to match selected cell text
		private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (dataGridView1.CurrentCell.Value != null)
			{
				selectedCell = Sheet.GetCell(dataGridView1.SelectedCells[0].RowIndex, dataGridView1.SelectedCells[0].ColumnIndex);
			}
			else
			{
				textBox1.Text = "";
			}

			if (selectedCell != null)
			{
				textBox1.Text = selectedCell.Text;
			}
		}

		//When the user enters a cell, update text box text and selectedcell
		private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			textBox1.Text = "";
			if (dataGridView1.CurrentCell.Value != null)
			{
				selectedCell = Sheet.GetCell(dataGridView1.SelectedCells[0].RowIndex, dataGridView1.SelectedCells[0].ColumnIndex);
			}
			else
			{
				selectedCell = null;
				textBox1.Text = "";
			}

			if (selectedCell != null && dataGridView1.CurrentCell.Value != null)
			{
				textBox1.Text = Sheet.GetCell(dataGridView1.SelectedCells[0].RowIndex, dataGridView1.SelectedCells[0].ColumnIndex).Text;
			}
		}

		//When enter is pressed, current cell's text is set to the text currently in the text box.
		private void textBox1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				Sheet.GetCell(dataGridView1.CurrentCell.RowIndex, dataGridView1.CurrentCell.ColumnIndex).Text = textBox1.Text;

			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Sheet.Serialize();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Sheet.Serialize();
		}

		private void loadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Sheet.Deserialize();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string Title = "Literally the Best Excel Clone";
			const string Version = "5.0";
			string Contact = "Brandon Townsend brandon.townsend@wsu.edu";
			string Copyright = "2018";
			string WaL = "Attribution 4.0 International";
			MessageBox.Show(Title + "\n" + "Version: " + Version + "\n" + "Contact: " + Contact + "\n" + "Copyright: " + Copyright + "\n" + "This product has no warranty.\n" + "Copyright: " + WaL, "About");
		}
	}
}
