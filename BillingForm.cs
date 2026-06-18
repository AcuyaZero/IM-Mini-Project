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
    public partial class BillingForm : Form
    {
        public BillingForm()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void BillingForm_Load(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            MethodsForAll a = new MethodsForAll();
            a.FixDVG(dataGridView1);
            a.DummyDVG(dataGridView1, "DOC 1", "N", "Cardiology");
            a.DummyDVG(dataGridView1, "DOC 2", "L", "Doctor");
            a.DummyDVG(dataGridView1, "DOC 3", "0", "Doctor");
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
