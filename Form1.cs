using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace IM_Mini_Project
{
    public partial class Form1 : Form
    {
        // Connection string - UPDATE WITH YOUR CREDENTIALS
        private string connectionString = "Server=localhost;Database=hospital_db;Uid=root;Pwd=1234;";
        private string selectedPatientId = "";
        private string lastID = "";

        public Form1()
        {
            InitializeComponent();
        }

        // =============================================
        // FORM LOAD EVENT
        // =============================================

        private void Form1_Load(object sender, EventArgs e)
        {
            // Apply rounded corners to buttons
            ButtonRound(btnAdd, 20);
            ButtonRound(btnUpdate, 20);
            ButtonRound(btnDelete, 20);
            ButtonRound(btnRefresh, 20);
            ButtonRound(btnClear, 20);

            // Setup placeholder texts
            SetupPlaceholders();

            // Load data
            LoadPatients();

            // Setup DataGridView
            SetupDataGridView();
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
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;

            // Remove any existing event subscriptions to avoid duplicates
            dataGridView1.CellClick -= dataGridView1_CellClick;
            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        private void SetupPlaceholders()
        {
            // First Name
            if (string.IsNullOrEmpty(textBox3.Text) || textBox3.Text == "Enter first name...")
            {
                textBox3.Text = "Enter first name...";
                textBox3.ForeColor = Color.DarkGray;
            }
            textBox3.Enter -= TextBox_Enter;
            textBox3.Leave -= TextBox_Leave;
            textBox3.Enter += TextBox_Enter;
            textBox3.Leave += TextBox_Leave;

            // Last Name
            if (string.IsNullOrEmpty(textBox4.Text) || textBox4.Text == "Enter last name...")
            {
                textBox4.Text = "Enter last name...";
                textBox4.ForeColor = Color.DarkGray;
            }
            textBox4.Enter -= TextBox_Enter;
            textBox4.Leave -= TextBox_Leave;
            textBox4.Enter += TextBox_Enter;
            textBox4.Leave += TextBox_Leave;

            // Email
            if (string.IsNullOrEmpty(textBox2.Text) || textBox2.Text == "Enter email address...")
            {
                textBox2.Text = "Enter email address...";
                textBox2.ForeColor = Color.DarkGray;
            }
            textBox2.Enter -= TextBox_Enter;
            textBox2.Leave -= TextBox_Leave;
            textBox2.Enter += TextBox_Enter;
            textBox2.Leave += TextBox_Leave;

            // Phone
            if (string.IsNullOrEmpty(textBox1.Text) || textBox1.Text == "Enter phone number...")
            {
                textBox1.Text = "Enter phone number...";
                textBox1.ForeColor = Color.DarkGray;
            }
            textBox1.Enter -= TextBox_Enter;
            textBox1.Leave -= TextBox_Leave;
            textBox1.Enter += TextBox_Enter;
            textBox1.Leave += TextBox_Leave;

            // Address
            if (string.IsNullOrEmpty(textBox5.Text) || textBox5.Text == "Enter address...")
            {
                textBox5.Text = "Enter address...";
                textBox5.ForeColor = Color.DarkGray;
            }
            textBox5.Enter -= TextBox_Enter;
            textBox5.Leave -= TextBox_Leave;
            textBox5.Enter += TextBox_Enter;
            textBox5.Leave += TextBox_Leave;

            // Search
            if (string.IsNullOrEmpty(textBox6.Text) || textBox6.Text == "Enter patient ID...")
            {
                textBox6.Text = "Enter patient ID...";
                textBox6.ForeColor = Color.DarkGray;
            }
            textBox6.Enter -= TextBox_Enter;
            textBox6.Leave -= TextBox_Leave;
            textBox6.Enter += TextBox_Enter;
            textBox6.Leave += TextBox_Leave;
            textBox6.KeyPress -= textBox6_KeyPress;
            textBox6.KeyPress += textBox6_KeyPress;
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt != null && (txt.Text == "Enter first name..." ||
                txt.Text == "Enter last name..." ||
                txt.Text == "Enter email address..." ||
                txt.Text == "Enter phone number..." ||
                txt.Text == "Enter address..." ||
                txt.Text == "Enter patient ID..."))
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
                if (txt == textBox3) txt.Text = "Enter first name...";
                else if (txt == textBox4) txt.Text = "Enter last name...";
                else if (txt == textBox2) txt.Text = "Enter email address...";
                else if (txt == textBox1) txt.Text = "Enter phone number...";
                else if (txt == textBox5) txt.Text = "Enter address...";
                else if (txt == textBox6) txt.Text = "Enter patient ID...";
                txt.ForeColor = Color.DarkGray;
            }
        }

        private string GetTextBoxValue(TextBox txt)
        {
            if (txt.ForeColor == Color.DarkGray || string.IsNullOrEmpty(txt.Text) ||
                txt.Text == "Enter first name..." || txt.Text == "Enter last name..." ||
                txt.Text == "Enter email address..." || txt.Text == "Enter phone number..." ||
                txt.Text == "Enter address..." || txt.Text == "Enter patient ID...")
                return "";
            return txt.Text.Trim();
        }

        private void ClearFields()
        {
            textBox3.Text = "Enter first name...";
            textBox3.ForeColor = Color.DarkGray;
            textBox4.Text = "Enter last name...";
            textBox4.ForeColor = Color.DarkGray;
            textBox2.Text = "Enter email address...";
            textBox2.ForeColor = Color.DarkGray;
            textBox1.Text = "Enter phone number...";
            textBox1.ForeColor = Color.DarkGray;
            textBox5.Text = "Enter address...";
            textBox5.ForeColor = Color.DarkGray;
            textBox6.Text = "Enter patient ID...";
            textBox6.ForeColor = Color.DarkGray;
            dateTimePicker1.Value = DateTime.Now.AddYears(-30);
            comboBox1.SelectedIndex = -1;
            selectedPatientId = "";
        }

        private bool ValidatePatientFields()
        {
            string firstName = GetTextBoxValue(textBox3);
            string lastName = GetTextBoxValue(textBox4);
            string email = GetTextBoxValue(textBox2);
            string phone = GetTextBoxValue(textBox1);
            string address = GetTextBoxValue(textBox5);

            if (string.IsNullOrEmpty(firstName))
            {
                MessageBox.Show("Please enter first name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(lastName))
            {
                MessageBox.Show("Please enter last name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox4.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please enter email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return false;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Please enter phone number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return false;
            }

            // PHONE NUMBER VALIDATION - Check if it's a valid phone number
            if (!IsValidPhoneNumber(phone))
            {
                MessageBox.Show("Please enter a valid phone number (numbers only, minimum 7 digits).\nExample: 09123456789 or 1234567",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                textBox1.SelectAll();
                return false;
            }

            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select gender.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox1.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // PHONE NUMBER VALIDATION METHOD
        private bool IsValidPhoneNumber(string phone)
        {
            // Remove any spaces, dashes, parentheses, etc.
            string cleaned = Regex.Replace(phone, @"[^\d]", "");

            // Check if it contains only numbers and has at least 7 digits
            return cleaned.Length >= 7 && Regex.IsMatch(cleaned, @"^\d+$");
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
                    string query = @"SELECT 
                                    patient_id AS 'PatientID',
                                    CONCAT(first_name, ' ', last_name) AS 'Name',
                                    gender AS 'Gender'
                                    FROM patient 
                                    ORDER BY patient_id";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Clear existing rows but keep columns
                    dataGridView1.Rows.Clear();

                    // Add rows manually to ensure correct column mapping
                    foreach (DataRow row in dt.Rows)
                    {
                        dataGridView1.Rows.Add(row["PatientID"].ToString(), row["Name"].ToString(), row["Gender"].ToString());
                    }

                    dataGridView1.AutoResizeColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading patients: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPatientDetails(string patientId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM patient WHERE patient_id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", patientId);

                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        textBox3.Text = reader["first_name"].ToString();
                        textBox3.ForeColor = Color.Black;
                        textBox4.Text = reader["last_name"].ToString();
                        textBox4.ForeColor = Color.Black;
                        if (reader["date_of_birth"] != DBNull.Value)
                            dateTimePicker1.Value = Convert.ToDateTime(reader["date_of_birth"]);
                        textBox2.Text = reader["email"].ToString();
                        textBox2.ForeColor = Color.Black;
                        textBox1.Text = reader["contact_no"].ToString();
                        textBox1.ForeColor = Color.Black;
                        textBox5.Text = reader["address"].ToString();
                        textBox5.ForeColor = Color.Black;
                        if (reader["gender"] != DBNull.Value)
                            comboBox1.SelectedItem = reader["gender"].ToString();
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading patient details: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchPatientById(string patientId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT 
                                    patient_id AS 'PatientID',
                                    CONCAT(first_name, ' ', last_name) AS 'Name',
                                    gender AS 'Gender'
                                    FROM patient 
                                    WHERE patient_id = @id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", patientId);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.Rows.Clear();

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            dataGridView1.Rows.Add(row["PatientID"].ToString(), row["Name"].ToString(), row["Gender"].ToString());
                        }
                        LoadPatientDetails(patientId);
                    }
                    else
                    {
                        MessageBox.Show("Patient ID not found.", "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPatients();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching patient: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =============================================
        // CRUD OPERATIONS
        // =============================================

        // CREATE - Add Patient
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidatePatientFields())
                return;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if email already exists
                    string checkQuery = "SELECT COUNT(*) FROM patient WHERE email = @email";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@email", GetTextBoxValue(textBox2));
                    int emailExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (emailExists > 0)
                    {
                        MessageBox.Show("Email already exists. Please use a different email.", "Duplicate Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string query = @"INSERT INTO patient (first_name, last_name, date_of_birth, email, contact_no, address, gender) 
                                    VALUES (@firstName, @lastName, @dob, @email, @phone, @address, @gender)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@firstName", GetTextBoxValue(textBox3));
                        cmd.Parameters.AddWithValue("@lastName", GetTextBoxValue(textBox4));
                        cmd.Parameters.AddWithValue("@dob", dateTimePicker1.Value.Date);
                        cmd.Parameters.AddWithValue("@email", GetTextBoxValue(textBox2));
                        cmd.Parameters.AddWithValue("@phone", GetTextBoxValue(textBox1));
                        cmd.Parameters.AddWithValue("@address", GetTextBoxValue(textBox5));
                        cmd.Parameters.AddWithValue("@gender", comboBox1.SelectedItem.ToString());

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Patient added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadPatients();
                            ClearFields();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding patient: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // UPDATE - Update Patient
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedPatientId))
            {
                MessageBox.Show("Please select a patient from the list to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidatePatientFields())
                return;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if email exists for another patient
                    string checkQuery = "SELECT COUNT(*) FROM patient WHERE email = @email AND patient_id != @id";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@email", GetTextBoxValue(textBox2));
                    checkCmd.Parameters.AddWithValue("@id", selectedPatientId);
                    int emailExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (emailExists > 0)
                    {
                        MessageBox.Show("Email already exists for another patient.", "Duplicate Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string query = @"UPDATE patient SET 
                                    first_name = @firstName,
                                    last_name = @lastName,
                                    date_of_birth = @dob,
                                    email = @email,
                                    contact_no = @phone,
                                    address = @address,
                                    gender = @gender
                                    WHERE patient_id = @patientId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@patientId", selectedPatientId);
                        cmd.Parameters.AddWithValue("@firstName", GetTextBoxValue(textBox3));
                        cmd.Parameters.AddWithValue("@lastName", GetTextBoxValue(textBox4));
                        cmd.Parameters.AddWithValue("@dob", dateTimePicker1.Value.Date);
                        cmd.Parameters.AddWithValue("@email", GetTextBoxValue(textBox2));
                        cmd.Parameters.AddWithValue("@phone", GetTextBoxValue(textBox1));
                        cmd.Parameters.AddWithValue("@address", GetTextBoxValue(textBox5));
                        cmd.Parameters.AddWithValue("@gender", comboBox1.SelectedItem.ToString());

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Patient updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadPatients();
                            ClearFields();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating patient: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // DELETE - Delete Patient
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedPatientId))
            {
                MessageBox.Show("Please select a patient from the list to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this patient?\n\n" +
                "This will also delete all associated records:\n" +
                "• Appointments\n" +
                "• Billing records\n" +
                "• Medical records\n" +
                "• Prescriptions\n" +
                "• Lab results\n\n" +
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

                        // Check if patient exists
                        string checkQuery = "SELECT COUNT(*) FROM patient WHERE patient_id = @id";
                        MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("@id", selectedPatientId);
                        int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (exists == 0)
                        {
                            MessageBox.Show("Patient not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        string query = "DELETE FROM patient WHERE patient_id = @patientId";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@patientId", selectedPatientId);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Patient deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadPatients();
                                ClearFields();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting patient: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // REFRESH
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadPatients();
            ClearFields();
        }

        // CLEAR - Reset everything and show all patients
        private void btnClear_Click(object sender, EventArgs e)
        {
            // Clear all input fields (placeholders)
            textBox3.Text = "Enter first name...";
            textBox3.ForeColor = Color.DarkGray;
            textBox4.Text = "Enter last name...";
            textBox4.ForeColor = Color.DarkGray;
            textBox2.Text = "Enter email address...";
            textBox2.ForeColor = Color.DarkGray;
            textBox1.Text = "Enter phone number...";
            textBox1.ForeColor = Color.DarkGray;
            textBox5.Text = "Enter address...";
            textBox5.ForeColor = Color.DarkGray;
            textBox6.Text = "Enter patient ID...";
            textBox6.ForeColor = Color.DarkGray;
            dateTimePicker1.Value = DateTime.Now.AddYears(-30);
            comboBox1.SelectedIndex = -1;
            selectedPatientId = "";

            // Reset the DataGridView to show ALL patients
            LoadPatients();

            // Optional: Show a confirmation message
            // MessageBox.Show("All fields cleared and patient list refreshed.", "Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    selectedPatientId = row.Cells[0].Value.ToString();
                    LoadPatientDetails(selectedPatientId);
                }
            }
        }

        // =============================================
        // SEARCH - Patient ID TextBox
        // =============================================

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string searchId = GetTextBoxValue(textBox6);
                if (!string.IsNullOrEmpty(searchId))
                {
                    SearchPatientById(searchId);
                }
                else
                {
                    LoadPatients();
                }
                e.Handled = true;
            }
        }

        // =============================================
        // TIMER EVENT
        // =============================================

        private void studentIDTimer_Tick(object sender, EventArgs e)
        {
            // Timer logic if needed
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Label click event
        }
    }
}