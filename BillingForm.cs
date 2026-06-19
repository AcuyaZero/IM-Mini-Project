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
    public partial class BillingForm : Form
    {
        // SINGLE INSTANCE OF DATABASE HELPER
        private Database db = new Database();
        private string selectedBillingId = "";
        private MethodsForAll methods = new MethodsForAll();

        public BillingForm()
        {
            InitializeComponent();

            // Wire up button click events
            this.button5.Click += new EventHandler(this.btnAdd_Click);
            this.button4.Click += new EventHandler(this.btnUpdate_Click);
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);
            this.btnClear.Click += new EventHandler(this.btnClear_Click);
            this.textBox5.KeyPress += new KeyPressEventHandler(this.textBox5_KeyPress);
            this.dataGridView1.CellClick += new DataGridViewCellEventHandler(this.dataGridView1_CellClick);
        }

        // =============================================
        // FORM LOAD EVENT
        // =============================================

        private void BillingForm_Load(object sender, EventArgs e)
        {
            // Apply rounded corners to buttons
            ButtonRound(button5, 20);
            ButtonRound(button4, 20);
            ButtonRound(btnDelete, 20);
            ButtonRound(btnRefresh, 20);
            ButtonRound(btnClear, 20);

            // Setup DataGridView
            SetupDataGridView();

            // Setup placeholder texts
            SetupPlaceholders();

            // Load data into dropdowns
            LoadInsuranceCompanies();
            LoadPaymentStatus();
            LoadPaymentMethods();

            // LOAD BILLING RECORDS
            LoadBillingRecords();

            // Update summary stats
            UpdateSummaryStats();
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
            // Clear everything
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            // Add columns - ADD HIDDEN BILLING ID COLUMN
            dataGridView1.Columns.Add("BillingID", "BillingID");
            dataGridView1.Columns.Add("Patient", "Patient");
            dataGridView1.Columns.Add("Date", "Date");
            dataGridView1.Columns.Add("Total", "Total");
            dataGridView1.Columns.Add("Coverage", "Coverage");
            dataGridView1.Columns.Add("Balance", "Balance");
            dataGridView1.Columns.Add("Status", "Status");

            // Hide the BillingID column
            dataGridView1.Columns[0].Visible = false;

            // Apply styling
            methods.FixDVG(dataGridView1);

            // Set column headers
            dataGridView1.Columns[1].HeaderText = "Patient";
            dataGridView1.Columns[2].HeaderText = "Date";
            dataGridView1.Columns[3].HeaderText = "Total";
            dataGridView1.Columns[4].HeaderText = "Coverage";
            dataGridView1.Columns[5].HeaderText = "Balance";
            dataGridView1.Columns[6].HeaderText = "Status";

            // Set column widths
            dataGridView1.Columns[1].Width = 150;
            dataGridView1.Columns[2].Width = 120;
            dataGridView1.Columns[3].Width = 100;
            dataGridView1.Columns[4].Width = 100;
            dataGridView1.Columns[5].Width = 100;
            dataGridView1.Columns[6].Width = 100;

            // Make read-only
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;

            // Wire up cell click
            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        private void SetupPlaceholders()
        {
            // Billing ID
            textBox5.Text = "Search Billing ID...";
            textBox5.ForeColor = Color.DarkGray;
            textBox5.Enter += (s, e) => { if (textBox5.Text == "Search Billing ID...") { textBox5.Text = ""; textBox5.ForeColor = Color.Black; } };
            textBox5.Leave += (s, e) => { if (string.IsNullOrEmpty(textBox5.Text)) { textBox5.Text = "Search Billing ID..."; textBox5.ForeColor = Color.DarkGray; } };

            // Patient ID
            textBox9.Text = "Enter Patient ID...";
            textBox9.ForeColor = Color.DarkGray;
            textBox9.Enter += (s, e) => { if (textBox9.Text == "Enter Patient ID...") { textBox9.Text = ""; textBox9.ForeColor = Color.Black; } };
            textBox9.Leave += (s, e) => { if (string.IsNullOrEmpty(textBox9.Text)) { textBox9.Text = "Enter Patient ID..."; textBox9.ForeColor = Color.DarkGray; } };

            // Doctor ID
            textBox6.Text = "Enter Doctor ID...";
            textBox6.ForeColor = Color.DarkGray;
            textBox6.Enter += (s, e) => { if (textBox6.Text == "Enter Doctor ID...") { textBox6.Text = ""; textBox6.ForeColor = Color.Black; } };
            textBox6.Leave += (s, e) => { if (string.IsNullOrEmpty(textBox6.Text)) { textBox6.Text = "Enter Doctor ID..."; textBox6.ForeColor = Color.DarkGray; } };

            // Appointment ID
            textBox1.Text = "Enter Appointment ID...";
            textBox1.ForeColor = Color.DarkGray;
            textBox1.Enter += (s, e) => { if (textBox1.Text == "Enter Appointment ID...") { textBox1.Text = ""; textBox1.ForeColor = Color.Black; } };
            textBox1.Leave += (s, e) => { if (string.IsNullOrEmpty(textBox1.Text)) { textBox1.Text = "Enter Appointment ID..."; textBox1.ForeColor = Color.DarkGray; } };

            // Insurance Coverage
            textBox2.Text = "0.00";
            textBox2.ForeColor = Color.DarkGray;
            textBox2.Enter += (s, e) => { if (textBox2.Text == "0.00") { textBox2.Text = ""; textBox2.ForeColor = Color.Black; } };
            textBox2.Leave += (s, e) => { if (string.IsNullOrEmpty(textBox2.Text)) { textBox2.Text = "0.00"; textBox2.ForeColor = Color.DarkGray; } };

            // Total Amount
            textBox3.Text = "0.00";
            textBox3.ForeColor = Color.DarkGray;
            textBox3.Enter += (s, e) => { if (textBox3.Text == "0.00") { textBox3.Text = ""; textBox3.ForeColor = Color.Black; } };
            textBox3.Leave += (s, e) => { if (string.IsNullOrEmpty(textBox3.Text)) { textBox3.Text = "0.00"; textBox3.ForeColor = Color.DarkGray; } };

            // Dropdowns
            comboBox2.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
        }

        private string GetTextBoxValue(TextBox txt)
        {
            if (txt.ForeColor == Color.DarkGray || string.IsNullOrEmpty(txt.Text) ||
                txt.Text == "Search Billing ID..." ||
                txt.Text == "Enter Patient ID..." ||
                txt.Text == "Enter Doctor ID..." ||
                txt.Text == "Enter Appointment ID..." ||
                txt.Text == "0.00")
                return "";
            return txt.Text.Trim();
        }

        private decimal GetDecimalValue(TextBox txt)
        {
            string value = GetTextBoxValue(txt);
            if (string.IsNullOrEmpty(value))
                return 0;
            decimal result;
            decimal.TryParse(value, out result);
            return result;
        }

        private void LoadInsuranceCompanies()
        {
            try
            {
                DataTable dt = db.GetDataTable("SELECT insurance_id, provider_name FROM insurance ORDER BY provider_name");
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow defaultRow = dt.NewRow();
                    defaultRow["insurance_id"] = DBNull.Value;
                    defaultRow["provider_name"] = "Select Insurance Company";
                    dt.Rows.InsertAt(defaultRow, 0);

                    comboBox2.DataSource = dt;
                    comboBox2.DisplayMember = "provider_name";
                    comboBox2.ValueMember = "insurance_id";
                    comboBox2.SelectedIndex = 0;
                }
                else
                {
                    comboBox2.Items.Clear();
                    comboBox2.Items.Add("Select Insurance Company");
                    comboBox2.Items.Add("Generic Insurance");
                    comboBox2.SelectedIndex = 0;
                }
            }
            catch
            {
                comboBox2.Items.Clear();
                comboBox2.Items.Add("Select Insurance Company");
                comboBox2.Items.Add("Generic Insurance");
                comboBox2.SelectedIndex = 0;
            }
        }

        private void LoadPaymentStatus()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Select Status");
            comboBox1.Items.Add("Pending");
            comboBox1.Items.Add("Paid");
            comboBox1.Items.Add("Partial");
            comboBox1.Items.Add("Cancelled");
            comboBox1.SelectedIndex = 0;
        }

        private void LoadPaymentMethods()
        {
            comboBox3.Items.Clear();
            comboBox3.Items.Add("Select Payment Method");
            comboBox3.Items.Add("Cash");
            comboBox3.Items.Add("Gcash");
            comboBox3.Items.Add("Credit Card");
            comboBox3.Items.Add("Debit Card");
            comboBox3.Items.Add("Bank Transfer");
            comboBox3.Items.Add("Insurance");
            comboBox3.SelectedIndex = 0;
        }

        // =============================================
        // LOAD BILLING RECORDS - HANDLES NULL DATES
        // =============================================

        private void LoadBillingRecords()
        {
            try
            {
                string query = @"SELECT 
                                b.billing_id AS 'BillingID',
                                CONCAT(p.first_name, ' ', p.last_name) AS 'Patient',
                                b.transaction_date AS 'Date',
                                b.total_amount AS 'Total',
                                b.insurance_coverage AS 'Coverage',
                                b.patient_balance AS 'Balance',
                                b.payment_status AS 'Status'
                                FROM billing b
                                LEFT JOIN patient p ON b.patient_id = p.patient_id
                                LEFT JOIN appointment a ON b.appointment_id = a.appointment_id
                                ORDER BY b.billing_id DESC";

                DataTable dt = db.GetDataTable(query);

                // Clear existing rows
                dataGridView1.Rows.Clear();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string billingId = row["BillingID"].ToString();
                        string patientName = row["Patient"] != DBNull.Value ? row["Patient"].ToString() : "Unknown";

                        // Handle NULL date properly
                        string dateValue = "";
                        if (row["Date"] != DBNull.Value && row["Date"] != DBNull.Value)
                        {
                            try
                            {
                                DateTime date = Convert.ToDateTime(row["Date"]);
                                dateValue = date.ToString("MM/dd/yyyy");
                            }
                            catch
                            {
                                dateValue = "N/A";
                            }
                        }
                        else
                        {
                            // If date is null, show "No Date"
                            dateValue = "No Date";
                        }

                        decimal total = row["Total"] != DBNull.Value ? Convert.ToDecimal(row["Total"]) : 0;
                        decimal coverage = row["Coverage"] != DBNull.Value ? Convert.ToDecimal(row["Coverage"]) : 0;
                        decimal balance = row["Balance"] != DBNull.Value ? Convert.ToDecimal(row["Balance"]) : 0;
                        string status = row["Status"] != DBNull.Value ? row["Status"].ToString() : "Pending";

                        dataGridView1.Rows.Add(
                            billingId,
                            patientName,
                            dateValue,
                            string.Format("{0:C}", total),
                            string.Format("{0:C}", coverage),
                            string.Format("{0:C}", balance),
                            status
                        );
                    }
                }
                else
                {
                    dataGridView1.Rows.Add("", "No billing records found", "", "", "", "", "");
                }

                dataGridView1.AutoResizeColumns();
                UpdateSummaryStats();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading billing records: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =============================================
        // LOAD BILLING DETAILS
        // =============================================

        private void LoadBillingDetails(string billingId)
        {
            try
            {
                string query = @"SELECT 
                                b.billing_id,
                                b.patient_id,
                                b.appointment_id,
                                b.insurance_id,
                                b.total_amount,
                                b.insurance_coverage,
                                b.patient_balance,
                                b.payment_status,
                                b.payment_method,
                                b.transaction_date,
                                CONCAT(p.first_name, ' ', p.last_name) AS patient_name,
                                a.doctor_id AS doctor_id,
                                CONCAT(d.first_name, ' ', d.last_name) AS doctor_name
                                FROM billing b
                                LEFT JOIN patient p ON b.patient_id = p.patient_id
                                LEFT JOIN appointment a ON b.appointment_id = a.appointment_id
                                LEFT JOIN doctor d ON a.doctor_id = d.doctor_id
                                WHERE b.billing_id = @id";

                var parameters = new Dictionary<string, object>
                {
                    { "@id", billingId }
                };

                var reader = db.GetDataReader(query, parameters);
                if (reader != null && reader.Read())
                {
                    // Billing ID
                    textBox5.Text = reader["billing_id"].ToString();
                    textBox5.ForeColor = Color.Black;

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

                    // Appointment ID
                    if (reader["appointment_id"] != DBNull.Value)
                    {
                        textBox1.Text = reader["appointment_id"].ToString();
                        textBox1.ForeColor = Color.Black;
                    }

                    // Insurance Coverage
                    if (reader["insurance_coverage"] != DBNull.Value)
                    {
                        textBox2.Text = reader["insurance_coverage"].ToString();
                        textBox2.ForeColor = Color.Black;
                    }

                    // Total Amount
                    if (reader["total_amount"] != DBNull.Value)
                    {
                        textBox3.Text = reader["total_amount"].ToString();
                        textBox3.ForeColor = Color.Black;
                    }

                    // Insurance
                    if (reader["insurance_id"] != DBNull.Value)
                        comboBox2.SelectedValue = reader["insurance_id"];
                    else
                        comboBox2.SelectedIndex = 0;

                    // Payment Status
                    if (reader["payment_status"] != DBNull.Value)
                        comboBox1.SelectedItem = reader["payment_status"].ToString();

                    // Payment Method
                    if (reader["payment_method"] != DBNull.Value)
                        comboBox3.SelectedItem = reader["payment_method"].ToString();

                    reader.Close();
                }
                else
                {
                    MessageBox.Show("Billing record not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading billing details: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchBillingById(string billingId)
        {
            try
            {
                string query = @"SELECT 
                                b.billing_id AS 'BillingID',
                                CONCAT(p.first_name, ' ', p.last_name) AS 'Patient',
                                b.transaction_date AS 'Date',
                                b.total_amount AS 'Total',
                                b.insurance_coverage AS 'Coverage',
                                b.patient_balance AS 'Balance',
                                b.payment_status AS 'Status'
                                FROM billing b
                                LEFT JOIN patient p ON b.patient_id = p.patient_id
                                LEFT JOIN appointment a ON b.appointment_id = a.appointment_id
                                WHERE b.billing_id = @id";

                var parameters = new Dictionary<string, object>
                {
                    { "@id", billingId }
                };

                DataTable dt = db.GetDataTable(query, parameters);

                dataGridView1.Rows.Clear();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string billingIdValue = row["BillingID"].ToString();
                        string patientName = row["Patient"] != DBNull.Value ? row["Patient"].ToString() : "Unknown";

                        string dateValue = "";
                        if (row["Date"] != DBNull.Value)
                        {
                            try
                            {
                                DateTime date = Convert.ToDateTime(row["Date"]);
                                dateValue = date.ToString("MM/dd/yyyy");
                            }
                            catch
                            {
                                dateValue = "N/A";
                            }
                        }
                        else
                        {
                            dateValue = "No Date";
                        }

                        decimal total = row["Total"] != DBNull.Value ? Convert.ToDecimal(row["Total"]) : 0;
                        decimal coverage = row["Coverage"] != DBNull.Value ? Convert.ToDecimal(row["Coverage"]) : 0;
                        decimal balance = row["Balance"] != DBNull.Value ? Convert.ToDecimal(row["Balance"]) : 0;
                        string status = row["Status"] != DBNull.Value ? row["Status"].ToString() : "Pending";

                        dataGridView1.Rows.Add(
                            billingIdValue,
                            patientName,
                            dateValue,
                            string.Format("{0:C}", total),
                            string.Format("{0:C}", coverage),
                            string.Format("{0:C}", balance),
                            status
                        );
                    }
                    LoadBillingDetails(billingId);
                }
                else
                {
                    MessageBox.Show("Billing ID not found.", "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBillingRecords();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching billing: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =============================================
        // UPDATE SUMMARY STATS
        // =============================================

        private void UpdateSummaryStats()
        {
            try
            {
                // Total Billed
                object totalResult = db.ExecuteScalar("SELECT COALESCE(SUM(total_amount), 0) FROM billing");
                decimal totalBilled = totalResult != null ? Convert.ToDecimal(totalResult) : 0;
                label17.Text = string.Format("{0:C}", totalBilled);

                // Total Collected (Paid)
                object collectedResult = db.ExecuteScalar("SELECT COALESCE(SUM(total_amount), 0) FROM billing WHERE payment_status = 'Paid'");
                decimal totalCollected = collectedResult != null ? Convert.ToDecimal(collectedResult) : 0;
                label18.Text = string.Format("{0:C}", totalCollected);

                // Total Pending
                object pendingResult = db.ExecuteScalar("SELECT COALESCE(SUM(total_amount), 0) FROM billing WHERE payment_status = 'Pending'");
                decimal totalPending = pendingResult != null ? Convert.ToDecimal(pendingResult) : 0;
                label19.Text = string.Format("{0:C}", totalPending);

                // Total Overdue
                label20.Text = string.Format("{0:C}", 0);
            }
            catch
            {
                // Silent fail for stats
            }
        }

        // =============================================
        // CRUD OPERATIONS
        // =============================================

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateBillingFields())
                return;

            try
            {
                string patientId = GetTextBoxValue(textBox9);
                string appointmentId = GetTextBoxValue(textBox1);

                // Check if patient exists
                object patientCheck = db.ExecuteScalar("SELECT COUNT(*) FROM patient WHERE patient_id = @id",
                    new Dictionary<string, object> { { "@id", patientId } });
                int patientExists = patientCheck != null ? Convert.ToInt32(patientCheck) : 0;

                if (patientExists == 0)
                {
                    MessageBox.Show("Patient not found. Please enter a valid Patient ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check if appointment exists
                object appointmentCheck = db.ExecuteScalar("SELECT COUNT(*) FROM appointment WHERE appointment_id = @id",
                    new Dictionary<string, object> { { "@id", appointmentId } });
                int appointmentExists = appointmentCheck != null ? Convert.ToInt32(appointmentCheck) : 0;

                if (appointmentExists == 0)
                {
                    MessageBox.Show("Appointment not found. Please enter a valid Appointment ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                decimal totalAmount = GetDecimalValue(textBox3);
                decimal insuranceCoverage = GetDecimalValue(textBox2);
                decimal patientBalance = totalAmount - insuranceCoverage;
                if (patientBalance < 0) patientBalance = 0;

                object insuranceId = comboBox2.SelectedIndex > 0 ? comboBox2.SelectedValue : DBNull.Value;
                string paymentStatus = comboBox1.SelectedIndex > 0 ? comboBox1.SelectedItem.ToString() : "Pending";
                string paymentMethod = comboBox3.SelectedIndex > 0 ? comboBox3.SelectedItem.ToString() : "";

                // FIX: Added transaction_date = CURDATE()
                string query = @"INSERT INTO billing 
                        (patient_id, appointment_id, insurance_id, total_amount, insurance_coverage, patient_balance, payment_status, payment_method, transaction_date) 
                        VALUES (@patientId, @appointmentId, @insuranceId, @totalAmount, @insuranceCoverage, @patientBalance, @paymentStatus, @paymentMethod, CURDATE())";

                var parameters = new Dictionary<string, object>
        {
            { "@patientId", patientId },
            { "@appointmentId", appointmentId },
            { "@insuranceId", insuranceId },
            { "@totalAmount", totalAmount },
            { "@insuranceCoverage", insuranceCoverage },
            { "@patientBalance", patientBalance },
            { "@paymentStatus", paymentStatus },
            { "@paymentMethod", paymentMethod }
        };

                int result = db.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Billing record added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBillingRecords();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding billing record: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedBillingId))
            {
                MessageBox.Show("Please select a billing record from the list to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateBillingFields())
                return;

            try
            {
                decimal totalAmount = GetDecimalValue(textBox3);
                decimal insuranceCoverage = GetDecimalValue(textBox2);
                decimal patientBalance = totalAmount - insuranceCoverage;
                if (patientBalance < 0) patientBalance = 0;

                object insuranceId = comboBox2.SelectedIndex > 0 ? comboBox2.SelectedValue : DBNull.Value;
                string paymentStatus = comboBox1.SelectedIndex > 0 ? comboBox1.SelectedItem.ToString() : "Pending";
                string paymentMethod = comboBox3.SelectedIndex > 0 ? comboBox3.SelectedItem.ToString() : "";

                string query = @"UPDATE billing SET 
                                insurance_id = @insuranceId,
                                total_amount = @totalAmount,
                                insurance_coverage = @insuranceCoverage,
                                patient_balance = @patientBalance,
                                payment_status = @paymentStatus,
                                payment_method = @paymentMethod,
                                transaction_date = CURDATE()
                                WHERE billing_id = @billingId";

                var parameters = new Dictionary<string, object>
                {
                    { "@billingId", selectedBillingId },
                    { "@insuranceId", insuranceId },
                    { "@totalAmount", totalAmount },
                    { "@insuranceCoverage", insuranceCoverage },
                    { "@patientBalance", patientBalance },
                    { "@paymentStatus", paymentStatus },
                    { "@paymentMethod", paymentMethod }
                };

                int result = db.ExecuteNonQuery(query, parameters);
                if (result > 0)
                {
                    MessageBox.Show("Billing record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBillingRecords();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating billing record: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedBillingId))
            {
                MessageBox.Show("Please select a billing record from the list to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this billing record?\n\n" +
                "This action cannot be undone.\n\n" +
                "This action will be logged in the audit trail.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    object checkResult = db.ExecuteScalar("SELECT COUNT(*) FROM billing WHERE billing_id = @id",
                        new Dictionary<string, object> { { "@id", selectedBillingId } });
                    int exists = checkResult != null ? Convert.ToInt32(checkResult) : 0;

                    if (exists == 0)
                    {
                        MessageBox.Show("Billing record not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int rowsAffected = db.ExecuteNonQuery("DELETE FROM billing WHERE billing_id = @billingId",
                        new Dictionary<string, object> { { "@billingId", selectedBillingId } });

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Billing record deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadBillingRecords();
                        ClearFields();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting billing record: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // =============================================
        // REFRESH & CLEAR
        // =============================================

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadBillingRecords();
            ClearFields();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
            LoadBillingRecords();
        }

        // =============================================
        // VALIDATION
        // =============================================

        private bool ValidateBillingFields()
        {
            string patientId = GetTextBoxValue(textBox9);
            string appointmentId = GetTextBoxValue(textBox1);
            decimal totalAmount = GetDecimalValue(textBox3);

            if (string.IsNullOrEmpty(patientId))
            {
                MessageBox.Show("Please enter Patient ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox9.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(appointmentId))
            {
                MessageBox.Show("Please enter Appointment ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return false;
            }

            if (totalAmount <= 0)
            {
                MessageBox.Show("Please enter a valid total amount (greater than 0).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                textBox3.SelectAll();
                return false;
            }

            if (comboBox1.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a payment status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox1.Focus();
                return false;
            }

            return true;
        }

        private void ClearFields()
        {
            textBox5.Text = "Search Billing ID...";
            textBox5.ForeColor = Color.DarkGray;
            textBox9.Text = "Enter Patient ID...";
            textBox9.ForeColor = Color.DarkGray;
            textBox6.Text = "Enter Doctor ID...";
            textBox6.ForeColor = Color.DarkGray;
            textBox1.Text = "Enter Appointment ID...";
            textBox1.ForeColor = Color.DarkGray;
            textBox2.Text = "0.00";
            textBox2.ForeColor = Color.DarkGray;
            textBox3.Text = "0.00";
            textBox3.ForeColor = Color.DarkGray;
            comboBox2.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            selectedBillingId = "";
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
                    string billingId = row.Cells[0].Value.ToString();

                    if (!string.IsNullOrEmpty(billingId))
                    {
                        selectedBillingId = billingId;
                        LoadBillingDetails(selectedBillingId);
                    }
                    else
                    {
                        string patientName = row.Cells[1].Value != null ? row.Cells[1].Value.ToString() : "";
                        if (patientName == "No billing records found")
                        {
                            // Do nothing
                        }
                        else
                        {
                            MessageBox.Show("Could not find billing ID for this record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }

        // =============================================
        // SEARCH - Billing ID TextBox
        // =============================================

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string searchId = GetTextBoxValue(textBox5);
                if (!string.IsNullOrEmpty(searchId))
                {
                    SearchBillingById(searchId);
                }
                else
                {
                    LoadBillingRecords();
                }
                e.Handled = true;
            }
        }

        // =============================================
        // PANEL PAINT EVENTS (Keep for compatibility)
        // =============================================

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            // Keep for designer compatibility
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            // Keep for designer compatibility
        }

        private void label21_Click(object sender, EventArgs e)
        {
            // Keep for designer compatibility
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Keep for designer compatibility
        }
    }
}