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
    public partial class Medical_Records : Form
    {
        // SINGLE INSTANCE OF DATABASE
        private Database db = new Database();
        private string selectedRecordId = "";
        private MethodsForAll methods = new MethodsForAll();

        public Medical_Records()
        {
            InitializeComponent();

            // Wire up button click events
            this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
            this.btnUpdate.Click += new EventHandler(this.btnUpdate_Click);
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);
            this.btnClear.Click += new EventHandler(this.btnClear_Click);
            this.textBox1.KeyPress += new KeyPressEventHandler(this.textBox1_KeyPress);
            this.dataGridView1.CellClick += new DataGridViewCellEventHandler(this.dataGridView1_CellClick);
        }

        // =============================================
        // FORM LOAD EVENT
        // =============================================

        private void Medical_Records_Load(object sender, EventArgs e)
        {
            // Apply rounded corners to buttons
            ButtonRound(btnAdd, 20);
            ButtonRound(btnUpdate, 20);
            ButtonRound(btnDelete, 20);
            ButtonRound(btnRefresh, 20);
            ButtonRound(btnClear, 20);

            // Setup DataGridView
            SetupDataGridView();

            // Setup placeholder texts
            SetupPlaceholders();

            // Load medical records
            LoadMedicalRecords();

            // Set default date
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
            dateTimePicker2.Format = DateTimePickerFormat.Time;
            dateTimePicker2.ShowUpDown = true;
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

            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Set column headers
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns[0].HeaderText = "Patient";
                dataGridView1.Columns[1].HeaderText = "Date";
                dataGridView1.Columns[2].HeaderText = "Diagnosis";
            }
        }

        private void SetupPlaceholders()
        {
            // Search Medical Record ID
            textBox1.Text = "Search Medical Record ID";
            textBox1.ForeColor = Color.DarkGray;
            textBox1.Enter += (s, e) =>
            {
                if (textBox1.Text == "Search Medical Record ID")
                {
                    textBox1.Text = "";
                    textBox1.ForeColor = Color.Black;
                }
            };
            textBox1.Leave += (s, e) =>
            {
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    textBox1.Text = "Search Medical Record ID";
                    textBox1.ForeColor = Color.DarkGray;
                }
            };

            // Patient ID
            textBox9.Text = "e.g. (Insert Patient ID Format)";
            textBox9.ForeColor = Color.DarkGray;
            textBox9.Enter += (s, e) =>
            {
                if (textBox9.Text == "e.g. (Insert Patient ID Format)")
                {
                    textBox9.Text = "";
                    textBox9.ForeColor = Color.Black;
                }
            };
            textBox9.Leave += (s, e) =>
            {
                if (string.IsNullOrEmpty(textBox9.Text))
                {
                    textBox9.Text = "e.g. (Insert Patient ID Format)";
                    textBox9.ForeColor = Color.DarkGray;
                }
            };

            // Doctor ID
            textBox6.Text = "e.g. (Insert Doctor ID Format)";
            textBox6.ForeColor = Color.DarkGray;
            textBox6.Enter += (s, e) =>
            {
                if (textBox6.Text == "e.g. (Insert Doctor ID Format)")
                {
                    textBox6.Text = "";
                    textBox6.ForeColor = Color.Black;
                }
            };
            textBox6.Leave += (s, e) =>
            {
                if (string.IsNullOrEmpty(textBox6.Text))
                {
                    textBox6.Text = "e.g. (Insert Doctor ID Format)";
                    textBox6.ForeColor = Color.DarkGray;
                }
            };
        }

        private string GetTextBoxValue(TextBox txt)
        {
            if (txt.ForeColor == Color.DarkGray || string.IsNullOrEmpty(txt.Text) ||
                txt.Text == "e.g. (Insert Patient ID Format)" ||
                txt.Text == "e.g. (Insert Doctor ID Format)" ||
                txt.Text == "Search Medical Record ID")
                return "";
            return txt.Text.Trim();
        }

        private string GetRichTextBoxValue(RichTextBox rtb)
        {
            if (string.IsNullOrEmpty(rtb.Text) || rtb.Text == "")
                return "";
            return rtb.Text.Trim();
        }

        private void ClearFields()
        {
            textBox1.Text = "Search Medical Record ID";
            textBox1.ForeColor = Color.DarkGray;
            textBox9.Text = "e.g. (Insert Patient ID Format)";
            textBox9.ForeColor = Color.DarkGray;
            textBox6.Text = "e.g. (Insert Doctor ID Format)";
            textBox6.ForeColor = Color.DarkGray;
            richTextBox1.Text = "";
            richTextBox2.Text = "";
            richTextBox3.Text = "";
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
            selectedRecordId = "";
        }

        private bool ValidateMedicalRecordFields()
        {
            string patientId = GetTextBoxValue(textBox9);
            string doctorId = GetTextBoxValue(textBox6);
            string diagnosis = GetRichTextBoxValue(richTextBox1);

            if (string.IsNullOrEmpty(patientId))
            {
                MessageBox.Show("Please enter Patient ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox9.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(doctorId))
            {
                MessageBox.Show("Please enter Doctor ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox6.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(diagnosis))
            {
                MessageBox.Show("Please enter a diagnosis.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                richTextBox1.Focus();
                return false;
            }

            return true;
        }

        // =============================================
        // LOAD DATA METHODS
        // =============================================

        private void LoadMedicalRecords()
        {
            try
            {
                string query = @"SELECT 
                                mr.record_id AS 'RecordID',
                                CONCAT(p.first_name, ' ', p.last_name) AS 'Patient',
                                mr.date_recorded AS 'Date',
                                mr.diagnosis AS 'Diagnosis'
                                FROM medical_record mr
                                LEFT JOIN patient p ON mr.patient_id = p.patient_id
                                ORDER BY mr.record_id DESC";

                DataTable dt = db.GetDataTable(query);

                dataGridView1.Rows.Clear();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string patientName = row["Patient"] != DBNull.Value ? row["Patient"].ToString() : "Unknown";
                        string dateValue = "";
                        if (row["Date"] != DBNull.Value)
                        {
                            DateTime date = Convert.ToDateTime(row["Date"]);
                            dateValue = date.ToString("MM/dd/yy");
                        }
                        string diagnosis = row["Diagnosis"] != DBNull.Value ? row["Diagnosis"].ToString() : "";

                        dataGridView1.Rows.Add(patientName, dateValue, diagnosis);
                    }
                }
                else
                {
                    dataGridView1.Rows.Add("No records found", "", "");
                }

                dataGridView1.AutoResizeColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading medical records: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadMedicalRecordDetails(string recordId)
        {
            try
            {
                string query = @"SELECT 
                                mr.*,
                                CONCAT(p.first_name, ' ', p.last_name) AS patient_name
                                FROM medical_record mr
                                LEFT JOIN patient p ON mr.patient_id = p.patient_id
                                WHERE mr.record_id = @id";

                var parameters = new Dictionary<string, object>
                {
                    { "@id", recordId }
                };

                var reader = db.GetDataReader(query, parameters);
                if (reader != null && reader.Read())
                {
                    // Patient ID
                    if (reader["patient_id"] != DBNull.Value)
                    {
                        textBox9.Text = reader["patient_id"].ToString();
                        textBox9.ForeColor = Color.Black;
                    }

                    // Doctor ID
                    if (reader["doctor_id"] != DBNull.Value)
                    {
                        textBox6.Text = reader["doctor_id"].ToString();
                        textBox6.ForeColor = Color.Black;
                    }

                    // Diagnosis
                    if (reader["diagnosis"] != DBNull.Value)
                    {
                        richTextBox1.Text = reader["diagnosis"].ToString();
                    }

                    // Treatment
                    if (reader["treatment"] != DBNull.Value)
                    {
                        richTextBox2.Text = reader["treatment"].ToString();
                    }

                    // Prescription
                    if (reader["prescription"] != DBNull.Value)
                    {
                        richTextBox3.Text = reader["prescription"].ToString();
                    }

                    // Date Recorded
                    if (reader["date_recorded"] != DBNull.Value)
                    {
                        dateTimePicker1.Value = Convert.ToDateTime(reader["date_recorded"]);
                    }

                    // Record ID
                    if (reader["record_id"] != DBNull.Value)
                    {
                        textBox1.Text = reader["record_id"].ToString();
                        textBox1.ForeColor = Color.Black;
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading medical record details: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchMedicalRecordById(string recordId)
        {
            try
            {
                string query = @"SELECT 
                                mr.record_id AS 'RecordID',
                                CONCAT(p.first_name, ' ', p.last_name) AS 'Patient',
                                mr.date_recorded AS 'Date',
                                mr.diagnosis AS 'Diagnosis'
                                FROM medical_record mr
                                LEFT JOIN patient p ON mr.patient_id = p.patient_id
                                WHERE mr.record_id = @id";

                var parameters = new Dictionary<string, object>
                {
                    { "@id", recordId }
                };

                DataTable dt = db.GetDataTable(query, parameters);

                dataGridView1.Rows.Clear();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string patientName = row["Patient"] != DBNull.Value ? row["Patient"].ToString() : "Unknown";
                        string dateValue = "";
                        if (row["Date"] != DBNull.Value)
                        {
                            DateTime date = Convert.ToDateTime(row["Date"]);
                            dateValue = date.ToString("MM/dd/yy");
                        }
                        string diagnosis = row["Diagnosis"] != DBNull.Value ? row["Diagnosis"].ToString() : "";

                        dataGridView1.Rows.Add(patientName, dateValue, diagnosis);
                    }
                    LoadMedicalRecordDetails(recordId);
                }
                else
                {
                    MessageBox.Show("Medical Record ID not found.", "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMedicalRecords();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching medical record: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =============================================
        // CRUD OPERATIONS
        // =============================================

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateMedicalRecordFields())
                return;

            try
            {
                string patientId = GetTextBoxValue(textBox9);
                string doctorId = GetTextBoxValue(textBox6);
                string diagnosis = GetRichTextBoxValue(richTextBox1);
                string treatment = GetRichTextBoxValue(richTextBox2);
                string prescription = GetRichTextBoxValue(richTextBox3);
                DateTime dateRecorded = dateTimePicker1.Value.Date;

                // Check if patient exists
                object patientCheck = db.ExecuteScalar("SELECT COUNT(*) FROM patient WHERE patient_id = @id",
                    new Dictionary<string, object> { { "@id", patientId } });
                int patientExists = patientCheck != null ? Convert.ToInt32(patientCheck) : 0;

                if (patientExists == 0)
                {
                    MessageBox.Show("Patient not found. Please enter a valid Patient ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check if doctor exists
                object doctorCheck = db.ExecuteScalar("SELECT COUNT(*) FROM doctor WHERE doctor_id = @id",
                    new Dictionary<string, object> { { "@id", doctorId } });
                int doctorExists = doctorCheck != null ? Convert.ToInt32(doctorCheck) : 0;

                if (doctorExists == 0)
                {
                    MessageBox.Show("Doctor not found. Please enter a valid Doctor ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string query = @"INSERT INTO medical_record (patient_id, doctor_id, diagnosis, treatment, prescription, date_recorded) 
                                VALUES (@patientId, @doctorId, @diagnosis, @treatment, @prescription, @dateRecorded)";

                var parameters = new Dictionary<string, object>
                {
                    { "@patientId", patientId },
                    { "@doctorId", doctorId },
                    { "@diagnosis", diagnosis },
                    { "@treatment", treatment },
                    { "@prescription", prescription },
                    { "@dateRecorded", dateRecorded }
                };

                int result = db.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Medical record added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMedicalRecords();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding medical record: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedRecordId))
            {
                MessageBox.Show("Please select a medical record from the list to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateMedicalRecordFields())
                return;

            try
            {
                string patientId = GetTextBoxValue(textBox9);
                string doctorId = GetTextBoxValue(textBox6);
                string diagnosis = GetRichTextBoxValue(richTextBox1);
                string treatment = GetRichTextBoxValue(richTextBox2);
                string prescription = GetRichTextBoxValue(richTextBox3);
                DateTime dateRecorded = dateTimePicker1.Value.Date;

                string query = @"UPDATE medical_record SET 
                                patient_id = @patientId,
                                doctor_id = @doctorId,
                                diagnosis = @diagnosis,
                                treatment = @treatment,
                                prescription = @prescription,
                                date_recorded = @dateRecorded
                                WHERE record_id = @recordId";

                var parameters = new Dictionary<string, object>
                {
                    { "@recordId", selectedRecordId },
                    { "@patientId", patientId },
                    { "@doctorId", doctorId },
                    { "@diagnosis", diagnosis },
                    { "@treatment", treatment },
                    { "@prescription", prescription },
                    { "@dateRecorded", dateRecorded }
                };

                int result = db.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Medical record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMedicalRecords();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating medical record: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedRecordId))
            {
                MessageBox.Show("Please select a medical record from the list to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this medical record?\n\n" +
                "This action cannot be undone.\n\n" +
                "This action will be logged in the audit trail.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM medical_record WHERE record_id = @recordId";
                    var parameters = new Dictionary<string, object>
                    {
                        { "@recordId", selectedRecordId }
                    };

                    int rowsAffected = db.ExecuteNonQuery(query, parameters);
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Medical record deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadMedicalRecords();
                        ClearFields();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting medical record: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // =============================================
        // REFRESH & CLEAR
        // =============================================

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadMedicalRecords();
            ClearFields();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
            LoadMedicalRecords();
        }

        // =============================================
        // DATA GRID VIEW EVENTS
        // =============================================

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                if (row.Cells.Count > 0 && row.Cells[0].Value != null && row.Cells[0].Value.ToString() != "No records found")
                {
                    // Get the record ID from the database using the patient name and date
                    string patientName = row.Cells[0].Value.ToString();
                    string dateValue = row.Cells[1].Value != null ? row.Cells[1].Value.ToString() : "";

                    if (!string.IsNullOrEmpty(patientName) && patientName != "No records found")
                    {
                        selectedRecordId = GetRecordIdByPatientAndDate(patientName, dateValue);
                        if (!string.IsNullOrEmpty(selectedRecordId))
                        {
                            LoadMedicalRecordDetails(selectedRecordId);
                        }
                    }
                }
            }
        }

        private string GetRecordIdByPatientAndDate(string patientName, string dateValue)
        {
            try
            {
                // Parse date from string
                DateTime date = DateTime.ParseExact(dateValue, "MM/dd/yy", null);

                string query = @"SELECT mr.record_id 
                                FROM medical_record mr
                                LEFT JOIN patient p ON mr.patient_id = p.patient_id
                                WHERE CONCAT(p.first_name, ' ', p.last_name) = @name 
                                AND DATE(mr.date_recorded) = @date
                                ORDER BY mr.record_id DESC LIMIT 1";

                var parameters = new Dictionary<string, object>
                {
                    { "@name", patientName },
                    { "@date", date.ToString("yyyy-MM-dd") }
                };

                object result = db.ExecuteScalar(query, parameters);
                return result != null ? result.ToString() : "";
            }
            catch
            {
                return "";
            }
        }

        // =============================================
        // SEARCH - Medical Record ID TextBox
        // =============================================

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string searchId = GetTextBoxValue(textBox1);
                if (!string.IsNullOrEmpty(searchId))
                {
                    SearchMedicalRecordById(searchId);
                }
                else
                {
                    LoadMedicalRecords();
                }
                e.Handled = true;
            }
        }

        // =============================================
        // PANEL PAINT EVENTS (Keep for compatibility)
        // =============================================

        private void panel6_Paint(object sender, PaintEventArgs e)
        {
            // Keep for designer compatibility
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Keep for designer compatibility
        }

        private void label9_Click(object sender, EventArgs e)
        {
            // Keep for designer compatibility
        }

        private void label3_Click(object sender, EventArgs e)
        {
            // Keep for designer compatibility
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            // Keep for designer compatibility
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            // Keep for designer compatibility
        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {
            // Keep for designer compatibility
        }
    }
}