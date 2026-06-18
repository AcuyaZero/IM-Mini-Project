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
        // Connection string - UPDATE WITH YOUR CREDENTIALS
        private string connectionString = "Server=localhost;Database=hospital_db;Uid=root;Pwd=1234;";
        private string selectedAppointmentId = "";
        private string lastID = "";

        public Appointments()
        {
            InitializeComponent();

            // Wire up button click events
            this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
            this.btnUpdate.Click += new EventHandler(this.btnUpdate_Click);
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);
            this.btnClear.Click += new EventHandler(this.btnClear_Click);
        }

        // =============================================
        // FORM LOAD EVENT
        // =============================================

        private void Appointments_Load(object sender, EventArgs e)
        {
            // Apply rounded corners to buttons
            ButtonRound(btnAdd, 20);
            ButtonRound(btnUpdate, 20);
            ButtonRound(btnDelete, 20);
            ButtonRound(btnRefresh, 20);
            ButtonRound(btnClear, 20);

            // Setup placeholder texts
            SetupPlaceholders();

            // Setup DataGridView with original design
            SetupDataGridView();

            // Load data into dropdowns
            LoadPatients();
            LoadDoctors();

            // Load appointments
            LoadAppointments();

            // Set default date/time
            dateTimePicker1.Value = DateTime.Now.Date.AddDays(1);
            dateTimePicker2.Value = DateTime.Now;

            // FIX: Resize DataGridView to fill the panel
            this.BeginInvoke(new Action(() => ResizeDataGridView()));

            // Add resize event to handle form resizing
            this.Resize += Appointments_Resize;
            this.ResizeEnd += Appointments_ResizeEnd;
        }

        // =============================================
        // RESIZE DATAGRIDVIEW TO FILL PANEL
        // =============================================

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

        // =============================================
        // UI HELPER METHODS
        // =============================================

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
            // Match the original design from Patient form
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

            // Set colors to match original design
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.GridColor = Color.LightGray;
            dataGridView1.ForeColor = Color.Black;

            // Column header style
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(48, 45, 109);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.EnableHeadersVisualStyles = false;

            // Row style
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 9.75F);
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView1.DefaultCellStyle.Padding = new Padding(5);
            dataGridView1.RowTemplate.Height = 35;

            // Alternating row colors for better readability
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

            // Auto-size columns
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // The columns are already defined in designer, set their styles
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
            if (string.IsNullOrEmpty(textBox1.Text) || textBox1.Text == "Search Appointment ID")
            {
                textBox1.Text = "Search Appointment ID";
                textBox1.ForeColor = Color.DarkGray;
            }
            textBox1.Enter -= TextBox_Enter;
            textBox1.Leave -= TextBox_Leave;
            textBox1.Enter += TextBox_Enter;
            textBox1.Leave += TextBox_Leave;
            textBox1.KeyPress -= textBox1_KeyPress;
            textBox1.KeyPress += textBox1_KeyPress;

            // Patient Name (textBox3)
            if (string.IsNullOrEmpty(textBox3.Text) || textBox3.Text == "Enter first name...")
            {
                textBox3.Text = "Enter patient name...";
                textBox3.ForeColor = Color.DarkGray;
            }
            textBox3.Enter -= TextBox_Enter;
            textBox3.Leave -= TextBox_Leave;
            textBox3.Enter += TextBox_Enter;
            textBox3.Leave += TextBox_Leave;

            // Doctor Name (textBox4)
            if (string.IsNullOrEmpty(textBox4.Text) || textBox4.Text == "Enter last name...")
            {
                textBox4.Text = "Enter doctor name...";
                textBox4.ForeColor = Color.DarkGray;
            }
            textBox4.Enter -= TextBox_Enter;
            textBox4.Leave -= TextBox_Leave;
            textBox4.Enter += TextBox_Enter;
            textBox4.Leave += TextBox_Leave;
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt != null && (txt.Text == "Search Appointment ID" ||
                txt.Text == "Enter patient name..." ||
                txt.Text == "Enter doctor name..."))
            {
                txt.Text = "";
                txt.ForeColor = Color.Black;
            }
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt != null && string.IsNullOrEmpty(txt.Text))
            {
                if (txt == textBox1) txt.Text = "Search Appointment ID";
                else if (txt == textBox3) txt.Text = "Enter patient name...";
                else if (txt == textBox4) txt.Text = "Enter doctor name...";
                txt.ForeColor = Color.DarkGray;
            }
        }

        private string GetTextBoxValue(TextBox txt)
        {
            if (txt.ForeColor == Color.DarkGray || string.IsNullOrEmpty(txt.Text) ||
                txt.Text == "Search Appointment ID" ||
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
            comboBox1.SelectedIndex = -1;
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

            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox1.Focus();
                return false;
            }

            return true;
        }

        // =============================================
        // LOAD DATA METHODS
        // =============================================

        private void LoadPatients()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT patient_id, CONCAT(first_name, ' ', last_name) AS patient_name FROM patient ORDER BY patient_name";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Store patient data for later use
                    textBox3.Tag = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading patients: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadDoctors()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT doctor_id, CONCAT(first_name, ' ', last_name) AS doctor_name FROM doctor ORDER BY doctor_name";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Store doctor data for later use
                    textBox4.Tag = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading doctors: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadAppointments()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT 
                                    a.appointment_id AS 'AppointmentID',
                                    CONCAT(p.first_name, ' ', p.last_name) AS 'Patient',
                                    CONCAT(d.first_name, ' ', d.last_name) AS 'Doctor'
                                    FROM appointment a
                                    LEFT JOIN patient p ON a.patient_id = p.patient_id
                                    LEFT JOIN doctor d ON a.doctor_id = d.doctor_id
                                    ORDER BY a.appointment_id";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.Rows.Clear();

                    foreach (DataRow row in dt.Rows)
                    {
                        dataGridView1.Rows.Add(row["AppointmentID"].ToString(), row["Patient"].ToString(), row["Doctor"].ToString());
                    }

                    dataGridView1.AutoResizeColumns();
                    ResizeDataGridView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading appointments: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAppointmentDetails(string appointmentId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT 
                                    a.*,
                                    CONCAT(p.first_name, ' ', p.last_name) AS patient_name,
                                    CONCAT(d.first_name, ' ', d.last_name) AS doctor_name
                                    FROM appointment a
                                    LEFT JOIN patient p ON a.patient_id = p.patient_id
                                    LEFT JOIN doctor d ON a.doctor_id = d.doctor_id
                                    WHERE a.appointment_id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", appointmentId);

                    MySqlDataReader reader = cmd.ExecuteReader();
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
                            comboBox1.SelectedItem = reader["appointment_status"].ToString();
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading appointment details: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchAppointmentById(string appointmentId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT 
                                    a.appointment_id AS 'AppointmentID',
                                    CONCAT(p.first_name, ' ', p.last_name) AS 'Patient',
                                    CONCAT(d.first_name, ' ', d.last_name) AS 'Doctor'
                                    FROM appointment a
                                    LEFT JOIN patient p ON a.patient_id = p.patient_id
                                    LEFT JOIN doctor d ON a.doctor_id = d.doctor_id
                                    WHERE a.appointment_id = @id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", appointmentId);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.Rows.Clear();

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            dataGridView1.Rows.Add(row["AppointmentID"].ToString(), row["Patient"].ToString(), row["Doctor"].ToString());
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
            catch (Exception ex)
            {
                MessageBox.Show("Error searching appointment: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetPatientIdByName(string patientName)
        {
            DataTable dt = textBox3.Tag as DataTable;
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["patient_name"].ToString() == patientName)
                    {
                        return Convert.ToInt32(row["patient_id"]);
                    }
                }
            }
            return -1;
        }

        private int GetDoctorIdByName(string doctorName)
        {
            DataTable dt = textBox4.Tag as DataTable;
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["doctor_name"].ToString() == doctorName)
                    {
                        return Convert.ToInt32(row["doctor_id"]);
                    }
                }
            }
            return -1;
        }

        // =============================================
        // CRUD OPERATIONS - ADD
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

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Check for double booking
                    string checkQuery = @"SELECT COUNT(*) FROM appointment 
                                        WHERE doctor_id = @doctorId 
                                        AND appointment_date = @date 
                                        AND appointment_time = @time 
                                        AND appointment_status IN ('Scheduled', 'No-Show')";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@doctorId", doctorId);
                    checkCmd.Parameters.AddWithValue("@date", dateTimePicker1.Value.Date);
                    checkCmd.Parameters.AddWithValue("@time", dateTimePicker2.Value.TimeOfDay);
                    int conflictCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (conflictCount > 0)
                    {
                        MessageBox.Show("Doctor already has an appointment at this time.", "Conflict Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string query = @"INSERT INTO appointment (patient_id, doctor_id, appointment_date, appointment_time, appointment_status) 
                                    VALUES (@patientId, @doctorId, @date, @time, @status)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@patientId", patientId);
                        cmd.Parameters.AddWithValue("@doctorId", doctorId);
                        cmd.Parameters.AddWithValue("@date", dateTimePicker1.Value.Date);
                        cmd.Parameters.AddWithValue("@time", dateTimePicker2.Value.TimeOfDay);
                        cmd.Parameters.AddWithValue("@status", comboBox1.SelectedItem.ToString());

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Appointment added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadAppointments();
                            ClearFields();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding appointment: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =============================================
        // CRUD OPERATIONS - UPDATE
        // =============================================

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

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Check for double booking (excluding current appointment)
                    string checkQuery = @"SELECT COUNT(*) FROM appointment 
                                        WHERE doctor_id = @doctorId 
                                        AND appointment_date = @date 
                                        AND appointment_time = @time 
                                        AND appointment_status IN ('Scheduled', 'No-Show')
                                        AND appointment_id != @appointmentId";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@doctorId", doctorId);
                    checkCmd.Parameters.AddWithValue("@date", dateTimePicker1.Value.Date);
                    checkCmd.Parameters.AddWithValue("@time", dateTimePicker2.Value.TimeOfDay);
                    checkCmd.Parameters.AddWithValue("@appointmentId", selectedAppointmentId);
                    int conflictCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (conflictCount > 0)
                    {
                        MessageBox.Show("Doctor already has an appointment at this time.", "Conflict Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string query = @"UPDATE appointment SET 
                                    patient_id = @patientId,
                                    doctor_id = @doctorId,
                                    appointment_date = @date,
                                    appointment_time = @time,
                                    appointment_status = @status
                                    WHERE appointment_id = @appointmentId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@appointmentId", selectedAppointmentId);
                        cmd.Parameters.AddWithValue("@patientId", patientId);
                        cmd.Parameters.AddWithValue("@doctorId", doctorId);
                        cmd.Parameters.AddWithValue("@date", dateTimePicker1.Value.Date);
                        cmd.Parameters.AddWithValue("@time", dateTimePicker2.Value.TimeOfDay);
                        cmd.Parameters.AddWithValue("@status", comboBox1.SelectedItem.ToString());

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Appointment updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadAppointments();
                            ClearFields();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating appointment: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =============================================
        // CRUD OPERATIONS - DELETE
        // =============================================

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
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();

                        // Check if appointment exists
                        string checkQuery = "SELECT COUNT(*) FROM appointment WHERE appointment_id = @id";
                        MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("@id", selectedAppointmentId);
                        int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (exists == 0)
                        {
                            MessageBox.Show("Appointment not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Delete appointment (cascade will handle billing)
                        string query = "DELETE FROM appointment WHERE appointment_id = @appointmentId";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@appointmentId", selectedAppointmentId);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Appointment deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadAppointments();
                                ClearFields();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting appointment: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // =============================================
        // CRUD OPERATIONS - REFRESH & CLEAR
        // =============================================

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadAppointments();
            ClearFields();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
            LoadAppointments();
        }

        // =============================================
        // DATA GRID VIEW EVENTS
        // =============================================

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

        // =============================================
        // SEARCH - Appointment ID TextBox
        // =============================================

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

        // =============================================
        // PANEL PAINT EVENTS (Keep for compatibility)
        // =============================================

        private void panel6_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void Appointments_Shown(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }
    }
}