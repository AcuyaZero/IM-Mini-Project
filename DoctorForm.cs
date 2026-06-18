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
    public partial class DoctorForm : Form
    {
        public DoctorForm()
        {
            InitializeComponent();
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void DoctorForm_Load(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            MethodsForAll a = new MethodsForAll();
            a.FixDVG(dataGridView1);
            a.DummyDVG(dataGridView1, "DOC 1", "N", "Cardiology");
            a.DummyDVG(dataGridView1, "DOC 2", "L", "Doctor");
            a.DummyDVG(dataGridView1, "DOC 3", "0", "Doctor");
        }
    }
}
