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
    public partial class MedicalRecordsMainForm : Form
    {
        private Form activeForm;
        private Form1 Patient = new Form1();
        private Appointments  Appointments = new Appointments();
        private DoctorForm Doctor = new DoctorForm();
        private BillingForm Billing = new BillingForm();
        private Medical_Records MedicalRecords = new Medical_Records();
        public MedicalRecordsMainForm()
        {
            InitializeComponent();
        }

        private void OpenForm(Form childForm)
        {
            if (activeForm != null)
                activeForm.Hide();

            activeForm = childForm;

            childForm.TopLevel = false;
            childForm.Dock = DockStyle.Fill;
            childForm.TopLevel = false;
            panel2.Controls.Clear();
            panel2.Controls.Add(childForm);

            childForm.BringToFront();
            childForm.Show();

        }

        private void MedicalRecordsMainForm_Load(object sender, EventArgs e)
        {
            
            OpenForm(Patient);
        }

        private void btnPatient_Click(object sender, EventArgs e)
        {
            OpenForm(Patient);
        }

        private void btnAppointments_Click(object sender, EventArgs e)
        {
            OpenForm(Appointments);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenForm(Doctor);
        }

        private void btnBilling_Click(object sender, EventArgs e)
        {
            OpenForm(Billing);
        }

        private void btnMedicalRecords_Click(object sender, EventArgs e)
        {
            OpenForm(MedicalRecords);
        }
    }
}
