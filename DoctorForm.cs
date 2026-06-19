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
    public partial class DoctorForm : Form
    {
        // SINGLE INSTANCE OF DATABASE HELPER
        private Database db = new Database();
        private string selectedDoctorId = "";

        public DoctorForm()
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

        private void DoctorForm_Load(object sender, EventArgs e)
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

            // Load departments into combo box
            LoadDepartments();

            // Load data
            LoadDoctors();

            // FIX: Resize DataGridView to fill the panel
            this.BeginInvoke(new Action(() => ResizeDataGridView()));

            // Add resize event to handle form resizing
            this.Resize += DoctorForm_Resize;
            this.ResizeEnd += DoctorForm_ResizeEnd;
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
            int dgvHeight = panelHeight - 119;

            if (dgvWidth > 100 && dgvHeight > 100)
            {
                dataGridView1.Width = dgvWidth;
                dataGridView1.Height = dgvHeight;
                dataGridView1.Location = new Point(11, 108);
                dataGridView1.Refresh();
            }
        }

        private void DoctorForm_Resize(object sender, EventArgs e)
        {
            ResizeDataGridView();
        }

        private void DoctorForm_ResizeEnd(object sender, EventArgs e)
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
                dataGridView1.Columns[0].HeaderText = "Doctor ID";
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[1].HeaderText = "Name";
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[2].HeaderText = "Department";
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

            dataGridView1.CellClick -= dataGridView1_CellClick;
            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        private void SetupPlaceholders()
        {
            // Search
            if (string.IsNullOrEmpty(textBox6.Text) || textBox6.Text == "Search Doctor ID")
            {
                textBox6.Text = "Search Doctor ID";
                textBox6.ForeColor = Color.DarkGray;
            }
            textBox6.Enter -= TextBox_Enter;
            textBox6.Leave -= TextBox_Leave;
            textBox6.Enter += TextBox_Enter;
            textBox6.Leave += TextBox_Leave;
            textBox6.KeyPress -= textBox6_KeyPress;
            textBox6.KeyPress += textBox6_KeyPress;

            // First Name (textBox3)
            if (string.IsNullOrEmpty(textBox3.Text) || textBox3.Text == "e.g. Juan")
            {
                textBox3.Text = "e.g. Juan";
                textBox3.ForeColor = Color.DarkGray;
            }
            textBox3.Enter -= TextBox_Enter;
            textBox3.Leave -= TextBox_Leave;
            textBox3.Enter += TextBox_Enter;
            textBox3.Leave += TextBox_Leave;

            // Last Name (textBox4)
            if (string.IsNullOrEmpty(textBox4.Text) || textBox4.Text == "e.g. Delacruz")
            {
                textBox4.Text = "e.g. Delacruz";
                textBox4.ForeColor = Color.DarkGray;
            }
            textBox4.Enter -= TextBox_Enter;
            textBox4.Leave -= TextBox_Leave;
            textBox4.Enter += TextBox_Enter;
            textBox4.Leave += TextBox_Leave;

            // Specialization (textBox7)
            if (string.IsNullOrEmpty(textBox7.Text) || textBox7.Text == "e.g. Cardiology")
            {
                textBox7.Text = "e.g. Cardiology";
                textBox7.ForeColor = Color.DarkGray;
            }
            textBox7.Enter -= TextBox_Enter;
            textBox7.Leave -= TextBox_Leave;
            textBox7.Enter += TextBox_Enter;
            textBox7.Leave += TextBox_Leave;

            // License Number (textBox8)
            if (string.IsNullOrEmpty(textBox8.Text) || textBox8.Text == "e.g. DOC-12345" || textBox8.Text == "e.g. (Insert Doctor ID format)")
            {
                textBox8.Text = "e.g. DOC-12345";
                textBox8.ForeColor = Color.DarkGray;
            }
            textBox8.Enter -= TextBox_Enter;
            textBox8.Leave -= TextBox_Leave;
            textBox8.Enter += TextBox_Enter;
            textBox8.Leave += TextBox_Leave;

            // Email (textBox2)
            if (string.IsNullOrEmpty(textBox2.Text) || textBox2.Text == "e.g. doctor@hospital.com")
            {
                textBox2.Text = "e.g. doctor@hospital.com";
                textBox2.ForeColor = Color.DarkGray;
            }
            textBox2.Enter -= TextBox_Enter;
            textBox2.Leave -= TextBox_Leave;
            textBox2.Enter += TextBox_Enter;
            textBox2.Leave += TextBox_Leave;

            // Phone (textBox1)
            if (string.IsNullOrEmpty(textBox1.Text) || textBox1.Text == "09XX-XXX-XXXX")
            {
                textBox1.Text = "09XX-XXX-XXXX";
                textBox1.ForeColor = Color.DarkGray;
            }
            textBox1.Enter -= TextBox_Enter;
            textBox1.Leave -= TextBox_Leave;
            textBox1.Enter += TextBox_Enter;
            textBox1.Leave += TextBox_Leave;
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt != null && (txt.Text == "Search Doctor ID" ||
                txt.Text == "e.g. Juan" ||
                txt.Text == "e.g. Delacruz" ||
                txt.Text == "e.g. Cardiology" ||
                txt.Text == "e.g. DOC-12345" ||
                txt.Text == "e.g. (Insert Doctor ID format)" ||
                txt.Text == "e.g. doctor@hospital.com" ||
                txt.Text == "09XX-XXX-XXXX"))
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
                if (txt == textBox6) txt.Text = "Search Doctor ID";
                else if (txt == textBox3) txt.Text = "e.g. Juan";
                else if (txt == textBox4) txt.Text = "e.g. Delacruz";
                else if (txt == textBox7) txt.Text = "e.g. Cardiology";
                else if (txt == textBox8) txt.Text = "e.g. DOC-12345";
                else if (txt == textBox2) txt.Text = "e.g. doctor@hospital.com";
                else if (txt == textBox1) txt.Text = "09XX-XXX-XXXX";
                txt.ForeColor = Color.DarkGray;
            }
        }

        // FIXED: GetTextBoxValue - Properly handles all fields including license
        private string GetTextBoxValue(TextBox txt)
        {
            // If it's a placeholder (gray text), return empty
            if (txt.ForeColor == Color.DarkGray)
                return "";

            // Check all placeholder texts (even if they somehow changed color)
            if (string.IsNullOrEmpty(txt.Text))
                return "";

            if (txt.Text == "Search Doctor ID" ||
                txt.Text == "e.g. Juan" ||
                txt.Text == "e.g. Delacruz" ||
                txt.Text == "e.g. Cardiology" ||
                txt.Text == "e.g. DOC-12345" ||
                txt.Text == "e.g. (Insert Doctor ID format)" ||
                txt.Text == "e.g. doctor@hospital.com" ||
                txt.Text == "09XX-XXX-XXXX")
                return "";

            return txt.Text.Trim();
        }

        // SPECIAL METHOD FOR LICENSE NUMBER - Always returns the text if it's not a placeholder
        private string GetLicenseNumber()
        {
            string text = textBox8.Text.Trim();

            // If it's a placeholder or empty, return empty
            if (string.IsNullOrEmpty(text) ||
                text == "e.g. DOC-12345" ||
                text == "e.g. (Insert Doctor ID format)")
                return "";

            return text;
        }

        private void ClearFields()
        {
            textBox6.Text = "Search Doctor ID";
            textBox6.ForeColor = Color.DarkGray;
            textBox3.Text = "e.g. Juan";
            textBox3.ForeColor = Color.DarkGray;
            textBox4.Text = "e.g. Delacruz";
            textBox4.ForeColor = Color.DarkGray;
            textBox7.Text = "e.g. Cardiology";
            textBox7.ForeColor = Color.DarkGray;
            textBox8.Text = "e.g. DOC-12345";
            textBox8.ForeColor = Color.DarkGray;
            textBox2.Text = "e.g. doctor@hospital.com";
            textBox2.ForeColor = Color.DarkGray;
            textBox1.Text = "09XX-XXX-XXXX";
            textBox1.ForeColor = Color.DarkGray;

            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
            else
                comboBox1.SelectedIndex = -1;

            selectedDoctorId = "";
        }

        private bool ValidateDoctorFields()
        {
            string firstName = GetTextBoxValue(textBox3);
            string lastName = GetTextBoxValue(textBox4);
            string specialization = GetTextBoxValue(textBox7);
            // Use the special method for license number
            string licenseNumber = GetLicenseNumber();
            string email = GetTextBoxValue(textBox2);
            string phone = GetTextBoxValue(textBox1);

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

            if (string.IsNullOrEmpty(specialization))
            {
                MessageBox.Show("Please enter specialization.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox7.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(licenseNumber))
            {
                MessageBox.Show("Please enter license number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox8.Focus();
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

            if (!IsValidPhoneNumber(phone))
            {
                MessageBox.Show("Please enter a valid phone number (numbers only, minimum 7 digits).\nExample: 09123456789 or 1234567",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                textBox1.SelectAll();
                return false;
            }

            if (comboBox1.SelectedIndex <= 0 || comboBox1.SelectedValue == null)
            {
                MessageBox.Show("Please select a valid department.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private bool IsValidPhoneNumber(string phone)
        {
            string cleaned = Regex.Replace(phone, @"[^\d]", "");
            return cleaned.Length >= 7 && Regex.IsMatch(cleaned, @"^\d+$");
        }

        // =============================================
        // LOAD DATA METHODS (USING DATABASEHELPER)
        // =============================================

        private void LoadDepartments()
        {
            DataTable dt = db.GetAllDepartments();
            if (dt != null)
            {
                // Add a default "Select Department" option
                DataRow defaultRow = dt.NewRow();
                defaultRow["department_id"] = DBNull.Value;
                defaultRow["department_name"] = "Select Department";
                dt.Rows.InsertAt(defaultRow, 0);

                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "department_name";
                comboBox1.ValueMember = "department_id";
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                // Fallback: Add some default departments if database doesn't have any
                comboBox1.Items.Clear();
                comboBox1.Items.Add("Select Department");
                comboBox1.Items.Add("Cardiology");
                comboBox1.Items.Add("Neurology");
                comboBox1.Items.Add("Pediatrics");
                comboBox1.Items.Add("Orthopedics");
                comboBox1.Items.Add("Dermatology");
                comboBox1.Items.Add("Ophthalmology");
                comboBox1.Items.Add("ENT");
                comboBox1.Items.Add("Psychiatry");
                comboBox1.Items.Add("General Medicine");
                comboBox1.Items.Add("Surgery");
                comboBox1.SelectedIndex = 0;
            }
        }

        private void LoadDoctors()
        {
            DataTable dt = db.GetAllDoctors();
            if (dt != null)
            {
                dataGridView1.Rows.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    dataGridView1.Rows.Add(row["DoctorID"].ToString(), row["Name"].ToString(), row["Department"].ToString());
                }

                dataGridView1.AutoResizeColumns();
                ResizeDataGridView();
            }
        }

        private void LoadDoctorDetails(string doctorId)
        {
            var reader = db.GetDoctorDetails(doctorId);
            if (reader != null)
            {
                if (reader.Read())
                {
                    textBox3.Text = reader["first_name"].ToString();
                    textBox3.ForeColor = Color.Black;
                    textBox4.Text = reader["last_name"].ToString();
                    textBox4.ForeColor = Color.Black;
                    textBox7.Text = reader["specialization"].ToString();
                    textBox7.ForeColor = Color.Black;
                    textBox8.Text = reader["license_number"].ToString();
                    textBox8.ForeColor = Color.Black;
                    textBox2.Text = reader["email"].ToString();
                    textBox2.ForeColor = Color.Black;
                    textBox1.Text = reader["contact_number"].ToString();
                    textBox1.ForeColor = Color.Black;

                    if (reader["department_id"] != DBNull.Value)
                        comboBox1.SelectedValue = reader["department_id"];
                    else
                        comboBox1.SelectedIndex = 0;
                }
                reader.Close();
            }
        }

        private void SearchDoctorById(string doctorId)
        {
            DataTable dt = db.GetDoctorById(doctorId);
            if (dt != null)
            {
                dataGridView1.Rows.Clear();

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        dataGridView1.Rows.Add(row["DoctorID"].ToString(), row["Name"].ToString(), row["Department"].ToString());
                    }
                    LoadDoctorDetails(doctorId);
                }
                else
                {
                    MessageBox.Show("Doctor ID not found.", "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDoctors();
                }

                ResizeDataGridView();
            }
        }

        // =============================================
        // CRUD OPERATIONS (USING DATABASEHELPER)
        // =============================================

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateDoctorFields())
                return;

            string firstName = GetTextBoxValue(textBox3);
            string lastName = GetTextBoxValue(textBox4);
            string specialization = GetTextBoxValue(textBox7);
            string licenseNumber = GetLicenseNumber();
            string email = GetTextBoxValue(textBox2);
            string phone = GetTextBoxValue(textBox1);
            object departmentId = comboBox1.SelectedValue;

            // Check if license already exists
            if (db.LicenseExists(licenseNumber))
            {
                MessageBox.Show("License number already exists. Please use a different license number.", "Duplicate Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if email already exists
            if (db.DoctorEmailExists(email))
            {
                MessageBox.Show("Email already exists. Please use a different email.", "Duplicate Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int result = db.AddDoctor(firstName, lastName, specialization, licenseNumber, phone, email, departmentId);
            if (result > 0)
            {
                MessageBox.Show("Doctor added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDoctors();
                ClearFields();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedDoctorId))
            {
                MessageBox.Show("Please select a doctor from the list to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateDoctorFields())
                return;

            string firstName = GetTextBoxValue(textBox3);
            string lastName = GetTextBoxValue(textBox4);
            string specialization = GetTextBoxValue(textBox7);
            string licenseNumber = GetLicenseNumber();
            string email = GetTextBoxValue(textBox2);
            string phone = GetTextBoxValue(textBox1);
            object departmentId = comboBox1.SelectedValue;

            // Check if license exists for another doctor
            if (db.LicenseExists(licenseNumber, selectedDoctorId))
            {
                MessageBox.Show("License number already exists for another doctor.", "Duplicate Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if email exists for another doctor
            if (db.DoctorEmailExists(email, selectedDoctorId))
            {
                MessageBox.Show("Email already exists for another doctor.", "Duplicate Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int result = db.UpdateDoctor(selectedDoctorId, firstName, lastName, specialization, licenseNumber, phone, email, departmentId);
            if (result > 0)
            {
                MessageBox.Show("Doctor updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDoctors();
                ClearFields();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedDoctorId))
            {
                MessageBox.Show("Please select a doctor from the list to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this doctor?\n\n" +
                "This will also delete all associated records:\n" +
                "• Appointments\n" +
                "• Schedules\n" +
                "• Medical records\n" +
                "• Prescriptions\n\n" +
                "This action will be logged in the audit trail.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                int rowsAffected = db.DeleteDoctor(selectedDoctorId);
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Doctor deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDoctors();
                    ClearFields();
                }
            }
        }

        // =============================================
        // CRUD OPERATIONS - REFRESH & CLEAR
        // =============================================

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDoctors();
            ClearFields();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
            LoadDoctors();
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
                    selectedDoctorId = row.Cells[0].Value.ToString();
                    LoadDoctorDetails(selectedDoctorId);
                }
            }
        }

        // =============================================
        // SEARCH - Doctor ID TextBox
        // =============================================

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string searchId = GetTextBoxValue(textBox6);
                if (!string.IsNullOrEmpty(searchId))
                {
                    SearchDoctorById(searchId);
                }
                else
                {
                    LoadDoctors();
                }
                e.Handled = true;
            }
        }

        // =============================================
        // PANEL PAINT EVENT (Keep for compatibility)
        // =============================================

        private void panel6_Paint(object sender, PaintEventArgs e)
        {
            // Keep this method for designer compatibility
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Keep this method for designer compatibility
        }
    }
}