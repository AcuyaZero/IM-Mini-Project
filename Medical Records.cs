using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IM_Mini_Project
{
    public partial class Medical_Records : Form
    {
        public Medical_Records()
        {
            InitializeComponent();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void Medical_Records_Load(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            MethodsForAll a = new MethodsForAll();
            a.FixDVG(dataGridView1);
            a.DummyDVG(dataGridView1, "Juan Delacruz", "6/15/26", "Common Cold");
            a.DummyDVG(dataGridView1, "Ian Jamir", "6/11/26", "Testecular Cancer");
            a.DummyDVG(dataGridView1, "Reijn Antillion", "6/9/26", "Lung Cancer");
        }
    }
}
