using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace IM_Mini_Project
{
    public partial class Appointments : Form
    {
        // SINGLE INSTANCE OF DATABASE
        private Database db = new Database();
        private string selectedAppointmentId = "";
        private bool isPatientTextChanging = false;
        private bool isDoctorTextChanging = false;

        public Appointments()
        {
            InitializeComponent();

            this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
            this.btnUpdate.Click += new EventHandler(this.btnUpdate_Click);
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);
            this.btnClear.Click += new EventHandler(this.btnClear_Click);
        }

        private void Appointments_Load(object sender, EventArgs e)
        {
            ButtonRound(btnAdd, 20);
            ButtonRound(btnUpdate, 20);
            ButtonRound(btnDelete, 20);
            ButtonRound(btnRefresh, 20);
            ButtonRound(btnClear, 20);

            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new object[] {
                "Select Status",
                "Scheduled",
                "Completed",
                "Cancelled",
                "No-Show"
            });
            comboBox1.SelectedIndex = 0;

            SetupDataGridView();
            LoadPatients();
            LoadDoctors();
            LoadAppointments();

            dateTimePicker1.Value = DateTime.Now.Date.AddDays(1);
            dateTimePicker2.Format = DateTimePickerFormat.Time;
            dateTimePicker2.ShowUpDown = true;
            dateTimePicker2.Value = DateTime.Now;

            SetupPlaceholders();

            this.BeginInvoke(new Action(() => ResizeDataGridView()));
            this.Resize += Appointments_Resize;
            this.ResizeEnd += Appointments_ResizeEnd;
        }

        private void ResizeDataGridView()
        {
            if (panel2 == null || dataGridView1 == null) return;

            int panelWidth = panel2.Width;
            int panelHeight = panel2.Height;

            int dgvWidth = panelWidth - 22;
            int dgvHeight = panelHeight - 60;

            if (dgvWidth > 100 && dgvHeight > 100)
            {
                dataGridView1.Width = dgvWidth;
                dataGridView1.Height = dgvHeight;
                dataGridView1.Location = new Point(11, 61);
                dataGridView1.Refresh();
            }
        }

        private void Appointments_Resize(object sender, EventArgs e)
        {
            ResizeDataGridView();
        }

        private void Appointments_ResizeEnd(object sender, EventArgs e)
        {
            ResizeDataGridView();
        }

        private void ButtonRound(Button btn, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(btn.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(btn.Width - radius, btn.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, btn.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            btn.Region = new Region(path);
        }

        private void SetupDataGridView()
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;

            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.GridColor = Color.LightGray;
            dataGridView1.ForeColor = Color.Black;

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(48, 45, 109);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.EnableHeadersVisualStyles = false;

            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 9.75F);
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView1.DefaultCellStyle.Padding = new Padding(5);
            dataGridView1.RowTemplate.Height = 35;

            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns[0].HeaderText = "Appointment ID";
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[1].HeaderText = "Patient";
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[2].HeaderText = "Doctor";
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

            dataGridView1.CellClick -= dataGridView1_CellClick;
            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        private void SetupPlaceholders()
        {
            // Search Appointment ID
            textBox1.Text = "Search Appointment ID";
            textBox1.ForeColor = Color.DarkGray;
            textBox1.Enter += (s, e) => { if (textBox1.Text == "Search Appointment ID") { textBox1.Text = ""; textBox1.ForeColor = Color.Black; } };
            textBox1.Leave += (s, e) => { if (string.IsNullOrEmpty(textBox1.Text)) { textBox1.Text = "Search Appointment ID"; textBox1.ForeColor = Color.DarkGray; } };
            textBox1.KeyPress += textBox1_KeyPress;

            // Patient Name
            textBox3.Text = "Enter patient name...";
            textBox3.ForeColor = Color.DarkGray;
            textBox3.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBox3.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBox3.Enter += (s, e) => { if (textBox3.Text == "Enter patient name...") { textBox3.Text = ""; textBox3.ForeColor = Color.Black; } };
            textBox3.Leave += (s, e) => { if (string.IsNullOrEmpty(textBox3.Text)) { textBox3.Text = "Enter patient name..."; textBox3.ForeColor = Color.DarkGray; } };
            textBox3.TextChanged += textBox3_TextChanged;

            // Doctor Name
            textBox4.Text = "Enter doctor name...";
            textBox4.ForeColor = Color.DarkGray;
            textBox4.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBox4.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBox4.Enter += (s, e) => { if (textBox4.Text == "Enter doctor name...") { textBox4.Text = ""; textBox4.ForeColor = Color.Black; } };
            textBox4.Leave += (s, e) => { if (string.IsNullOrEmpty(textBox4.Text)) { textBox4.Text = "Enter doctor name..."; textBox4.ForeColor = Color.DarkGray; } };
            textBox4.TextChanged += textBox4_TextChanged;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (isPatientTextChanging) return;

            string searchText = textBox3.Text;
            if (searchText == "Enter patient name..." || string.IsNullOrEmpty(searchText))
                return;

            DataTable dt = db.GetPatientsForAutoComplete(searchText);
            if (dt != null)
            {
                AutoCompleteStringCollection autoComplete = new AutoCompleteStringCollection();
                foreach (DataRow row in dt.Rows)
                {
                    autoComplete.Add(row["patient_name"].ToString());
                }
                textBox3.AutoCompleteCustomSource = autoComplete;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (isDoctorTextChanging) return;

            string searchText = textBox4.Text;
            if (searchText == "Enter doctor name..." || string.IsNullOrEmpty(searchText))
                return;

            DataTable dt = db.GetDoctorsForAutoComplete(searchText);
            if (dt != null)
            {
                AutoCompleteStringCollection autoComplete = new AutoCompleteStringCollection();
                foreach (DataRow row in dt.Rows)
                {
                    autoComplete.Add(row["doctor_name"].ToString());
                }
                textBox4.AutoCompleteCustomSource = autoComplete;
            }
        }

        private string GetTextBoxValue(TextBox txt)
        {
            if (txt.ForeColor == Color.DarkGray || string.IsNullOrEmpty(txt.Text) ||
                txt.Text == "Enter patient name..." || txt.Text == "Enter doctor name...")
                return "";
            return txt.Text.Trim();
        }

        private void ClearFields()
        {
            textBox1.Text = "Search Appointment ID";
            textBox1.ForeColor = Color.DarkGray;
            textBox3.Text = "Enter patient name...";
            textBox3.ForeColor = Color.DarkGray;
            textBox4.Text = "Enter doctor name...";
            textBox4.ForeColor = Color.DarkGray;
            dateTimePicker1.Value = DateTime.Now.Date.AddDays(1);
            dateTimePicker2.Value = DateTime.Now;
            comboBox1.SelectedIndex = 0;
            selectedAppointmentId = "";
        }

        private bool ValidateAppointmentFields()
        {
            string patientName = GetTextBoxValue(textBox3);
            string doctorName = GetTextBoxValue(textBox4);

            if (string.IsNullOrEmpty(patientName))
            {
                MessageBox.Show("Please select a patient.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(doctorName))
            {
                MessageBox.Show("Please select a doctor.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox4.Focus();
                return false;
            }

            if (dateTimePicker1.Value.Date < DateTime.Now.Date)
            {
                MessageBox.Show("Appointment date cannot be in the past.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dateTimePicker1.Focus();
                return false;
            }

            if (comboBox1.SelectedIndex == 0 || comboBox1.SelectedItem.ToString() == "Select Status")
            {
                MessageBox.Show("Please select a status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox1.Focus();
                return false;
            }

            return true;
        }

        // =============================================
        // LOAD DATA METHODS - USING STORED PROCEDURES
        // =============================================

        private void LoadPatients()
        {
            DataTable dt = db.GetAllPatientNames();
            if (dt != null)
            {
                textBox3.Tag = dt;

                AutoCompleteStringCollection autoComplete = new AutoCompleteStringCollection();
                foreach (DataRow row in dt.Rows)
                {
                    autoComplete.Add(row["patient_name"].ToString());
                }
                textBox3.AutoCompleteCustomSource = autoComplete;
            }
        }

        private void LoadDoctors()
        {
            DataTable dt = db.GetAllDoctorNames();
            if (dt != null)
            {
                textBox4.Tag = dt;

                AutoCompleteStringCollection autoComplete = new AutoCompleteStringCollection();
                foreach (DataRow row in dt.Rows)
                {
                    autoComplete.Add(row["doctor_name"].ToString());
                }
                textBox4.AutoCompleteCustomSource = autoComplete;
            }
        }

        private void LoadAppointments()
        {
            DataTable dt = db.GetAllAppointments();
            if (dt != null)
            {
                dataGridView1.Rows.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    dataGridView1.Rows.Add(
                        row["AppointmentID"].ToString(),
                        row["Patient"].ToString(),
                        row["Doctor"].ToString()
                    );
                }

                dataGridView1.AutoResizeColumns();
                ResizeDataGridView();
            }
        }

        private void LoadAppointmentDetails(string appointmentId)
        {
            var reader = db.GetAppointmentDetails(appointmentId);
            if (reader != null)
            {
                if (reader.Read())
                {
                    textBox3.Text = reader["patient_name"].ToString();
                    textBox3.ForeColor = Color.Black;
                    textBox4.Text = reader["doctor_name"].ToString();
                    textBox4.ForeColor = Color.Black;

                    if (reader["appointment_date"] != DBNull.Value)
                        dateTimePicker1.Value = Convert.ToDateTime(reader["appointment_date"]);

                    if (reader["appointment_time"] != DBNull.Value)
                    {
                        TimeSpan time = (TimeSpan)reader["appointment_time"];
                        dateTimePicker2.Value = DateTime.Today.Add(time);
                    }

                    if (reader["appointment_status"] != DBNull.Value)
                    {
                        string status = reader["appointment_status"].ToString();
                        int index = comboBox1.Items.IndexOf(status);
                        comboBox1.SelectedIndex = index >= 0 ? index : 0;
                    }
                }
                reader.Close();
            }
        }

        private void SearchAppointmentById(string appointmentId)
        {
            DataTable dt = db.GetAppointmentById(appointmentId);
            if (dt != null)
            {
                dataGridView1.Rows.Clear();

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        dataGridView1.Rows.Add(
                            row["AppointmentID"].ToString(),
                            row["Patient"].ToString(),
                            row["Doctor"].ToString()
                        );
                    }
                    LoadAppointmentDetails(appointmentId);
                }
                else
                {
                    MessageBox.Show("Appointment ID not found.", "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAppointments();
                }

                ResizeDataGridView();
            }
        }

        private int GetPatientIdByName(string patientName)
        {
            return db.GetPatientIdByName(patientName);
        }

        private int GetDoctorIdByName(string doctorName)
        {
            return db.GetDoctorIdByName(doctorName);
        }

        // =============================================
        // CRUD OPERATIONS - USING STORED PROCEDURES
        // =============================================

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateAppointmentFields())
                return;

            string patientName = GetTextBoxValue(textBox3);
            string doctorName = GetTextBoxValue(textBox4);

            int patientId = GetPatientIdByName(patientName);
            int doctorId = GetDoctorIdByName(doctorName);

            if (patientId == -1)
            {
                MessageBox.Show("Selected patient not found. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (doctorId == -1)
            {
                MessageBox.Show("Selected doctor not found. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int result = db.AddAppointment(
                patientId.ToString(),
                doctorId.ToString(),
                dateTimePicker1.Value.Date,
                dateTimePicker2.Value.TimeOfDay,
                comboBox1.SelectedItem.ToString()
            );

            if (result > 0)
            {
                MessageBox.Show("Appointment added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadAppointments();
                ClearFields();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedAppointmentId))
            {
                MessageBox.Show("Please select an appointment from the list to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateAppointmentFields())
                return;

            string patientName = GetTextBoxValue(textBox3);
            string doctorName = GetTextBoxValue(textBox4);

            int patientId = GetPatientIdByName(patientName);
            int doctorId = GetDoctorIdByName(doctorName);

            if (patientId == -1)
            {
                MessageBox.Show("Selected patient not found. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (doctorId == -1)
            {
                MessageBox.Show("Selected doctor not found. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int result = db.UpdateAppointment(
                selectedAppointmentId,
                patientId.ToString(),
                doctorId.ToString(),
                dateTimePicker1.Value.Date,
                dateTimePicker2.Value.TimeOfDay,
                comboBox1.SelectedItem.ToString()
            );

            if (result > 0)
            {
                MessageBox.Show("Appointment updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadAppointments();
                ClearFields();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedAppointmentId))
            {
                MessageBox.Show("Please select an appointment from the list to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this appointment?\n\n" +
                "This will also delete the associated billing record.\n\n" +
                "This action will be logged in the audit trail.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                int rowsAffected = db.DeleteAppointment(selectedAppointmentId);
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Appointment deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAppointments();
                    ClearFields();
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadPatients();
            LoadDoctors();
            LoadAppointments();
            ClearFields();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
            LoadAppointments();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                if (row.Cells.Count > 0 && row.Cells[0].Value != null)
                {
                    selectedAppointmentId = row.Cells[0].Value.ToString();
                    LoadAppointmentDetails(selectedAppointmentId);
                }
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string searchId = GetTextBoxValue(textBox1);
                if (!string.IsNullOrEmpty(searchId))
                {
                    SearchAppointmentById(searchId);
                }
                else
                {
                    LoadAppointments();
                }
                e.Handled = true;
            }
        }

        private void panel6_Paint(object sender, PaintEventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void Appointments_Shown(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }
    }
}