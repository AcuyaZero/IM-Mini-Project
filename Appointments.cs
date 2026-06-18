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
    public partial class Appointments : Form
    {
        public Appointments()
        {
            InitializeComponent();
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Appointments_Load(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            MethodsForAll a = new MethodsForAll();
            a.FixDVG(dataGridView1);
            a.DummyDVG(dataGridView1, "APM 1", "Patient", "Doctor");
            a.DummyDVG(dataGridView1, "APM 2", "Patient", "Doctor");
            a.DummyDVG(dataGridView1, "APM 3", "Patient", "Doctor");
        }

        private void Appointments_Shown(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
