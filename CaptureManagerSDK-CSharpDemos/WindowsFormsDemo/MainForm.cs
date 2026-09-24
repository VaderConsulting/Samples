using System;
using System.Windows.Forms;

namespace WindowsFormsDemo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            (new WebViewer()).ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            (new Recording()).ShowDialog();
        }
    }
}
