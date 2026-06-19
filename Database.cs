using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace IM_Mini_Project
{
    public class Database
    {
        // =============================================
        // SINGLE CONNECTION STRING - ONE PLACE TO UPDATE
        // =============================================
        private string connectionString = "Server=localhost;Database=hospital_db;Uid=root;Pwd=1234;";

        // =============================================
        // GET CONNECTION
        // =============================================
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        // =============================================
        // EXECUTE NON-QUERY (INSERT, UPDATE, DELETE)
        // =============================================
        public int ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value);
                            }
                        }
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        // =============================================
        // EXECUTE SCALAR (GET SINGLE VALUE)
        // =============================================
        public object ExecuteScalar(string query, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value);
                            }
                        }
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // =============================================
        // GET DATA TABLE (FOR GRIDS/LISTS)
        // =============================================
        public DataTable GetDataTable(string query, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value);
                            }
                        }
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // =============================================
        // GET DATA READER (FOR SINGLE RECORD)
        // =============================================
        public MySqlDataReader GetDataReader(string query, Dictionary<string, object> parameters = null)
        {
            try
            {
                MySqlConnection conn = GetConnection();
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // =============================================
        // PATIENT METHODS
        // =============================================

        // GET ALL PATIENTS
        public DataTable GetAllPatients()
        {
            string query = @"SELECT 
                            patient_id AS 'PatientID',
                            CONCAT(first_name, ' ', last_name) AS 'Name',
                            gender AS 'Gender'
                            FROM patient 
                            ORDER BY patient_id";
            return GetDataTable(query);
        }

        // GET PATIENT BY ID
        public DataTable GetPatientById(string patientId)
        {
            string query = @"SELECT 
                            patient_id AS 'PatientID',
                            CONCAT(first_name, ' ', last_name) AS 'Name',
                            gender AS 'Gender'
                            FROM patient 
                            WHERE patient_id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", patientId }
            };
            return GetDataTable(query, parameters);
        }

        // GET PATIENT DETAILS
        public MySqlDataReader GetPatientDetails(string patientId)
        {
            string query = "SELECT * FROM patient WHERE patient_id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", patientId }
            };
            return GetDataReader(query, parameters);
        }

        // ADD PATIENT
        public int AddPatient(string firstName, string lastName, DateTime dob, string email, string phone, string address, string gender)
        {
            string query = @"INSERT INTO patient (first_name, last_name, date_of_birth, email, contact_no, address, gender) 
                            VALUES (@firstName, @lastName, @dob, @email, @phone, @address, @gender)";
            var parameters = new Dictionary<string, object>
            {
                { "@firstName", firstName },
                { "@lastName", lastName },
                { "@dob", dob },
                { "@email", email },
                { "@phone", phone },
                { "@address", address },
                { "@gender", gender }
            };
            return ExecuteNonQuery(query, parameters);
        }

        // UPDATE PATIENT
        public int UpdatePatient(string patientId, string firstName, string lastName, DateTime dob, string email, string phone, string address, string gender)
        {
            string query = @"UPDATE patient SET 
                            first_name = @firstName,
                            last_name = @lastName,
                            date_of_birth = @dob,
                            email = @email,
                            contact_no = @phone,
                            address = @address,
                            gender = @gender
                            WHERE patient_id = @patientId";
            var parameters = new Dictionary<string, object>
            {
                { "@patientId", patientId },
                { "@firstName", firstName },
                { "@lastName", lastName },
                { "@dob", dob },
                { "@email", email },
                { "@phone", phone },
                { "@address", address },
                { "@gender", gender }
            };
            return ExecuteNonQuery(query, parameters);
        }

        // DELETE PATIENT
        public int DeletePatient(string patientId)
        {
            string query = "DELETE FROM patient WHERE patient_id = @patientId";
            var parameters = new Dictionary<string, object>
            {
                { "@patientId", patientId }
            };
            return ExecuteNonQuery(query, parameters);
        }

        // CHECK IF EMAIL EXISTS
        public bool EmailExists(string email, string excludePatientId = null)
        {
            string query = "SELECT COUNT(*) FROM patient WHERE email = @email";
            var parameters = new Dictionary<string, object>
            {
                { "@email", email }
            };

            if (!string.IsNullOrEmpty(excludePatientId))
            {
                query += " AND patient_id != @patientId";
                parameters.Add("@patientId", excludePatientId);
            }

            object result = ExecuteScalar(query, parameters);
            if (result == null) return true;
            return Convert.ToInt32(result) > 0;
        }

        // GET PATIENTS FOR AUTO-COMPLETE
        public DataTable GetPatientsForAutoComplete(string searchText)
        {
            string query = @"SELECT patient_id, CONCAT(first_name, ' ', last_name) AS patient_name 
                            FROM patient 
                            WHERE first_name LIKE @search OR last_name LIKE @search 
                            LIMIT 10";
            var parameters = new Dictionary<string, object>
            {
                { "@search", "%" + searchText + "%" }
            };
            return GetDataTable(query, parameters);
        }

        // GET ALL PATIENT NAMES
        public DataTable GetAllPatientNames()
        {
            string query = "SELECT patient_id, CONCAT(first_name, ' ', last_name) AS patient_name FROM patient ORDER BY patient_name";
            return GetDataTable(query);
        }

        // =============================================
        // DOCTOR METHODS
        // =============================================

        // GET ALL DOCTORS
        public DataTable GetAllDoctors()
        {
            string query = @"SELECT 
                            doctor_id AS 'DoctorID',
                            CONCAT(first_name, ' ', last_name) AS 'Name',
                            d.department_name AS 'Department'
                            FROM doctor doc
                            LEFT JOIN department d ON doc.department_id = d.department_id
                            ORDER BY doctor_id";
            return GetDataTable(query);
        }

        // GET DOCTOR BY ID
        public DataTable GetDoctorById(string doctorId)
        {
            string query = @"SELECT 
                            doctor_id AS 'DoctorID',
                            CONCAT(first_name, ' ', last_name) AS 'Name',
                            d.department_name AS 'Department'
                            FROM doctor doc
                            LEFT JOIN department d ON doc.department_id = d.department_id
                            WHERE doctor_id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", doctorId }
            };
            return GetDataTable(query, parameters);
        }

        // GET DOCTOR DETAILS
        public MySqlDataReader GetDoctorDetails(string doctorId)
        {
            string query = @"SELECT doc.*, d.department_name 
                            FROM doctor doc
                            LEFT JOIN department d ON doc.department_id = d.department_id
                            WHERE doc.doctor_id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", doctorId }
            };
            return GetDataReader(query, parameters);
        }

        // ADD DOCTOR
        public int AddDoctor(string firstName, string lastName, string specialization, string licenseNumber, string phone, string email, object departmentId)
        {
            string query = @"INSERT INTO doctor (first_name, last_name, specialization, license_number, contact_number, email, department_id) 
                            VALUES (@firstName, @lastName, @specialization, @license, @phone, @email, @department)";
            var parameters = new Dictionary<string, object>
            {
                { "@firstName", firstName },
                { "@lastName", lastName },
                { "@specialization", specialization },
                { "@license", licenseNumber },
                { "@phone", phone },
                { "@email", email },
                { "@department", departmentId }
            };
            return ExecuteNonQuery(query, parameters);
        }

        // UPDATE DOCTOR
        public int UpdateDoctor(string doctorId, string firstName, string lastName, string specialization, string licenseNumber, string phone, string email, object departmentId)
        {
            string query = @"UPDATE doctor SET 
                            first_name = @firstName,
                            last_name = @lastName,
                            specialization = @specialization,
                            license_number = @license,
                            contact_number = @phone,
                            email = @email,
                            department_id = @department
                            WHERE doctor_id = @doctorId";
            var parameters = new Dictionary<string, object>
            {
                { "@doctorId", doctorId },
                { "@firstName", firstName },
                { "@lastName", lastName },
                { "@specialization", specialization },
                { "@license", licenseNumber },
                { "@phone", phone },
                { "@email", email },
                { "@department", departmentId }
            };
            return ExecuteNonQuery(query, parameters);
        }

        // DELETE DOCTOR
        public int DeleteDoctor(string doctorId)
        {
            string query = "DELETE FROM doctor WHERE doctor_id = @doctorId";
            var parameters = new Dictionary<string, object>
            {
                { "@doctorId", doctorId }
            };
            return ExecuteNonQuery(query, parameters);
        }

        // CHECK IF LICENSE EXISTS
        public bool LicenseExists(string licenseNumber, string excludeDoctorId = null)
        {
            string query = "SELECT COUNT(*) FROM doctor WHERE license_number = @license";
            var parameters = new Dictionary<string, object>
            {
                { "@license", licenseNumber }
            };

            if (!string.IsNullOrEmpty(excludeDoctorId))
            {
                query += " AND doctor_id != @doctorId";
                parameters.Add("@doctorId", excludeDoctorId);
            }

            object result = ExecuteScalar(query, parameters);
            if (result == null) return true;
            return Convert.ToInt32(result) > 0;
        }

        // CHECK IF DOCTOR EMAIL EXISTS
        public bool DoctorEmailExists(string email, string excludeDoctorId = null)
        {
            string query = "SELECT COUNT(*) FROM doctor WHERE email = @email";
            var parameters = new Dictionary<string, object>
            {
                { "@email", email }
            };

            if (!string.IsNullOrEmpty(excludeDoctorId))
            {
                query += " AND doctor_id != @doctorId";
                parameters.Add("@doctorId", excludeDoctorId);
            }

            object result = ExecuteScalar(query, parameters);
            if (result == null) return true;
            return Convert.ToInt32(result) > 0;
        }

        // GET DOCTORS FOR AUTO-COMPLETE
        public DataTable GetDoctorsForAutoComplete(string searchText)
        {
            string query = @"SELECT doctor_id, CONCAT(first_name, ' ', last_name) AS doctor_name 
                            FROM doctor 
                            WHERE first_name LIKE @search OR last_name LIKE @search 
                            LIMIT 10";
            var parameters = new Dictionary<string, object>
            {
                { "@search", "%" + searchText + "%" }
            };
            return GetDataTable(query, parameters);
        }

        // GET ALL DOCTOR NAMES
        public DataTable GetAllDoctorNames()
        {
            string query = "SELECT doctor_id, CONCAT(first_name, ' ', last_name) AS doctor_name FROM doctor ORDER BY doctor_name";
            return GetDataTable(query);
        }

        // GET ALL DEPARTMENTS
        public DataTable GetAllDepartments()
        {
            string query = "SELECT department_id, department_name FROM department ORDER BY department_name";
            return GetDataTable(query);
        }

        // =============================================
        // APPOINTMENT METHODS
        // =============================================

        // GET ALL APPOINTMENTS
        public DataTable GetAllAppointments()
        {
            string query = @"SELECT 
                            a.appointment_id AS 'AppointmentID',
                            CONCAT(p.first_name, ' ', p.last_name) AS 'Patient',
                            CONCAT(d.first_name, ' ', d.last_name) AS 'Doctor'
                            FROM appointment a
                            LEFT JOIN patient p ON a.patient_id = p.patient_id
                            LEFT JOIN doctor d ON a.doctor_id = d.doctor_id
                            ORDER BY a.appointment_id";
            return GetDataTable(query);
        }

        // GET APPOINTMENT BY ID
        public DataTable GetAppointmentById(string appointmentId)
        {
            string query = @"SELECT 
                            a.appointment_id AS 'AppointmentID',
                            CONCAT(p.first_name, ' ', p.last_name) AS 'Patient',
                            CONCAT(d.first_name, ' ', d.last_name) AS 'Doctor'
                            FROM appointment a
                            LEFT JOIN patient p ON a.patient_id = p.patient_id
                            LEFT JOIN doctor d ON a.doctor_id = d.doctor_id
                            WHERE a.appointment_id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", appointmentId }
            };
            return GetDataTable(query, parameters);
        }

        // GET APPOINTMENT DETAILS
        public MySqlDataReader GetAppointmentDetails(string appointmentId)
        {
            string query = @"SELECT 
                            a.*,
                            CONCAT(p.first_name, ' ', p.last_name) AS patient_name,
                            CONCAT(d.first_name, ' ', d.last_name) AS doctor_name
                            FROM appointment a
                            LEFT JOIN patient p ON a.patient_id = p.patient_id
                            LEFT JOIN doctor d ON a.doctor_id = d.doctor_id
                            WHERE a.appointment_id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", appointmentId }
            };
            return GetDataReader(query, parameters);
        }

        // ADD APPOINTMENT
        public int AddAppointment(string patientId, string doctorId, DateTime date, TimeSpan time, string status)
        {
            string query = @"INSERT INTO appointment (patient_id, doctor_id, appointment_date, appointment_time, appointment_status) 
                            VALUES (@patientId, @doctorId, @date, @time, @status)";
            var parameters = new Dictionary<string, object>
            {
                { "@patientId", patientId },
                { "@doctorId", doctorId },
                { "@date", date },
                { "@time", time },
                { "@status", status }
            };
            return ExecuteNonQuery(query, parameters);
        }

        // UPDATE APPOINTMENT
        public int UpdateAppointment(string appointmentId, string patientId, string doctorId, DateTime date, TimeSpan time, string status)
        {
            string query = @"UPDATE appointment SET 
                            patient_id = @patientId,
                            doctor_id = @doctorId,
                            appointment_date = @date,
                            appointment_time = @time,
                            appointment_status = @status
                            WHERE appointment_id = @appointmentId";
            var parameters = new Dictionary<string, object>
            {
                { "@appointmentId", appointmentId },
                { "@patientId", patientId },
                { "@doctorId", doctorId },
                { "@date", date },
                { "@time", time },
                { "@status", status }
            };
            return ExecuteNonQuery(query, parameters);
        }

        // DELETE APPOINTMENT
        public int DeleteAppointment(string appointmentId)
        {
            string query = "DELETE FROM appointment WHERE appointment_id = @appointmentId";
            var parameters = new Dictionary<string, object>
            {
                { "@appointmentId", appointmentId }
            };
            return ExecuteNonQuery(query, parameters);
        }

        // CHECK FOR DOUBLE BOOKING
        public bool IsDoctorAvailable(string doctorId, DateTime date, TimeSpan time, string excludeAppointmentId = null)
        {
            string query = @"SELECT COUNT(*) FROM appointment 
                            WHERE doctor_id = @doctorId 
                            AND appointment_date = @date 
                            AND appointment_time = @time 
                            AND appointment_status IN ('Scheduled', 'No-Show')";
            var parameters = new Dictionary<string, object>
            {
                { "@doctorId", doctorId },
                { "@date", date },
                { "@time", time }
            };

            if (!string.IsNullOrEmpty(excludeAppointmentId))
            {
                query += " AND appointment_id != @appointmentId";
                parameters.Add("@appointmentId", excludeAppointmentId);
            }

            object result = ExecuteScalar(query, parameters);
            if (result == null) return false;
            return Convert.ToInt32(result) == 0;
        }

        // GET PATIENT ID BY NAME
        public int GetPatientIdByName(string patientName)
        {
            string query = "SELECT patient_id FROM patient WHERE CONCAT(first_name, ' ', last_name) = @name";
            var parameters = new Dictionary<string, object>
            {
                { "@name", patientName }
            };
            object result = ExecuteScalar(query, parameters);
            if (result == null) return -1;
            return Convert.ToInt32(result);
        }

        // GET DOCTOR ID BY NAME
        public int GetDoctorIdByName(string doctorName)
        {
            string query = "SELECT doctor_id FROM doctor WHERE CONCAT(first_name, ' ', last_name) = @name";
            var parameters = new Dictionary<string, object>
            {
                { "@name", doctorName }
            };
            object result = ExecuteScalar(query, parameters);
            if (result == null) return -1;
            return Convert.ToInt32(result);
        }

        // =============================================
        // BILLING METHODS - ADD THIS SECTION
        // =============================================

        // GET ALL BILLING RECORDS
        public DataTable GetAllBillingRecords()
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
            return GetDataTable(query);
        }

        // GET BILLING DETAILS BY ID
        public MySqlDataReader GetBillingDetails(string billingId)
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
            return GetDataReader(query, parameters);
        }

        // ADD BILLING RECORD
        public int AddBilling(string patientId, string appointmentId, object insuranceId, decimal totalAmount, decimal insuranceCoverage, decimal patientBalance, string paymentStatus, string paymentMethod)
        {
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
            return ExecuteNonQuery(query, parameters);
        }

        // UPDATE BILLING RECORD
        public int UpdateBilling(string billingId, object insuranceId, decimal totalAmount, decimal insuranceCoverage, decimal patientBalance, string paymentStatus, string paymentMethod)
        {
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
        { "@billingId", billingId },
        { "@insuranceId", insuranceId },
        { "@totalAmount", totalAmount },
        { "@insuranceCoverage", insuranceCoverage },
        { "@patientBalance", patientBalance },
        { "@paymentStatus", paymentStatus },
        { "@paymentMethod", paymentMethod }
    };
            return ExecuteNonQuery(query, parameters);
        }

        // DELETE BILLING RECORD
        public int DeleteBilling(string billingId)
        {
            string query = "DELETE FROM billing WHERE billing_id = @billingId";
            var parameters = new Dictionary<string, object>
    {
        { "@billingId", billingId }
    };
            return ExecuteNonQuery(query, parameters);
        }
    }
}