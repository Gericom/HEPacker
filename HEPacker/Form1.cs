using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HEPacker
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK
				&& folderBrowserDialog1.SelectedPath.Length > 0)
			{
				string[] he = System.IO.Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.he*");
				string[] a = System.IO.Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.(a)");
				string[] b = System.IO.Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.(b)");
				if (he.Length + a.Length + b.Length >= 4) textBox1.Text = folderBrowserDialog1.SelectedPath;
				else MessageBox.Show("Folder does not contain the right files!");
				if (textBox2.Text.Length > 0 && textBox1.Text.Length > 0) button3.Enabled = true;
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK
				&& saveFileDialog1.FileName.Length > 0)
			{
				textBox2.Text = saveFileDialog1.FileName;
				if (textBox2.Text.Length > 0 && textBox1.Text.Length > 0) button3.Enabled = true;
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			Packer.Pack(textBox1.Text, textBox2.Text);
		}
	}
}
