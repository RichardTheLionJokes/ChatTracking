using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatTracking.Controllers;
using System.Threading;

namespace ChatTracking
{
    public partial class Form1 : Form
    {
        ushort viewedMsg;
		
        public Form1()
        {
            InitializeComponent();

            foreach (string user in MainController.GetUsers())
            {
                comboBox1.Items.Add(user);
                comboBox2.Items.Add(user);
            }
        }

        private void buttonTrack_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            foreach (string msg in MainController.GetMessages(comboBox1.Text, comboBox2.Text, (ushort)numericUpDown1.Value, 0))
            {
                richTextBox1.AppendText(msg + "\n\n");
            }
            viewedMsg = (ushort)numericUpDown1.Value;
        }

        private void buttonTrackMore_Click(object sender, EventArgs e)
        {
            foreach (string msg in MainController.GetMessages(comboBox1.Text, comboBox2.Text, (ushort)numericUpDown1.Value, viewedMsg))
            {
                richTextBox1.AppendText(msg + "\n\n");
            }
            viewedMsg += (ushort)numericUpDown1.Value;
        }
    }
}
