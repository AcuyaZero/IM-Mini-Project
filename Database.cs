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
        // SINGLE CONNECTION STRING
        // =============================================
        private string connectionString = "Server=localhost;Database=hospital_db;Uid=root;Pwd=1234;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        // =============================================
        // EXECUTE STORED PROCEDURE WITH OUTPUT PARAMETERS
        // =============================================
        public int ExecuteStoredProcedureWithOutput(string procedureName, Dictionary<string, object> parameters)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(procedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        foreach (var param in parameters)
                        {
                            if (param.Value is MySqlDbType)
                            {
                                cmd.Parameters.Add(param.Key, (MySqlDbType)param.Value);
                                cmd.Parameters[param.Key].Direction = ParameterDirection.Output;
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value);
                            }
                        }

                        cmd.ExecuteNonQuery();

                        foreach (var param in new List<string>(parameters.Keys))
                        {
                            if (cmd.Parameters.Contains(param))
                            {
                                var dbParam = cmd.Parameters[param];
                                if (dbParam.Direction == ParameterDirection.Output || dbParam.Direction == ParameterDirection.InputOutput)
                                {
                                    parameters[param] = dbParam.Value;
                                }
                            }
                        }

                        if (parameters.ContainsKey("p_result_code"))
                        {
                            return Convert.ToInt32(parameters["p_result_code"]);
                        }

                        return 1;
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
        // EXECUTE STORED PROCEDURE (NO OUTPUT PARAMETERS)
        // =============================================
        public int ExecuteStoredProcedure(string procedureName, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(procedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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
        // EXECUTE STORED PROCEDURE (SCALAR)
        // =============================================
        public object ExecuteScalarProcedure(string procedureName, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(procedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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
        // EXECUTE STORED PROCEDURE (READER)
        // =============================================
        public MySqlDataReader ExecuteReaderProcedure(string procedureName, Dictionary<string, object> parameters = null)
        {
            try
            {
                MySqlConnection conn = GetConnection();
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(procedureName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
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
        // EXECUTE STORED PROCEDURE (DATATABLE)
        // =============================================
        public DataTable ExecuteDataTableProcedure(string procedureName, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(procedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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
        // GET DATA TABLE (FOR INLINE SQL - LEGACY)
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
        // EXECUTE SCALAR (FOR INLINE SQL - LEGACY)
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
        // PATIENT METHODS
        // =============================================

        public DataTable GetAllPatients()
        {
            return ExecuteDataTableProcedure("sp_GetAllPatients");
        }

        public DataTable GetPatientById(string patientId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_patient_id", patientId }
            };
            return ExecuteDataTableProcedure("sp_GetPatientById", parameters);
        }

        public MySqlDataReader GetPatientDetails(string patientId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_patient_id", patientId }
            };
            return ExecuteReaderProcedure("sp_GetPatientDetails", parameters);
        }

        public int AddPatient(string firstName, string lastName, DateTime dob, string email, string phone, string address, string gender)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_first_name", firstName },
                { "p_last_name", lastName },
                { "p_date_of_birth", dob },
                { "p_email", email },
                { "p_contact_no", phone },
                { "p_address", address },
                { "p_gender", gender },
                { "p_result_code", MySqlDbType.Int32 },
                { "p_result_message", MySqlDbType.VarChar }
            };
            return ExecuteStoredProcedureWithOutput("sp_AddPatient", parameters);
        }

        public int UpdatePatient(string patientId, string firstName, string lastName, DateTime dob, string email, string phone, string address, string gender)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_patient_id", patientId },
                { "p_first_name", firstName },
                { "p_last_name", lastName },
                { "p_date_of_birth", dob },
                { "p_email", email },
                { "p_contact_no", phone },
                { "p_address", address },
                { "p_gender", gender },
                { "p_result_code", MySqlDbType.Int32 },
                { "p_result_message", MySqlDbType.VarChar }
            };
            return ExecuteStoredProcedureWithOutput("sp_UpdatePatient", parameters);
        }

        public int DeletePatient(string patientId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_patient_id", patientId },
                { "p_result_code", MySqlDbType.Int32 },
                { "p_result_message", MySqlDbType.VarChar }
            };
            return ExecuteStoredProcedureWithOutput("sp_DeletePatient", parameters);
        }

        public DataTable GetPatientsForAutoComplete(string searchText)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_search_text", searchText }
            };
            return ExecuteDataTableProcedure("sp_GetPatientsForAutoComplete", parameters);
        }

        public DataTable GetAllPatientNames()
        {
            return ExecuteDataTableProcedure("sp_GetAllPatientNames");
        }

        public int GetPatientIdByName(string patientName)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_patient_name", patientName }
            };
            var result = ExecuteScalarProcedure("sp_GetPatientIdByName", parameters);
            return result != null ? Convert.ToInt32(result) : -1;
        }


        // =============================================
        // DOCTOR METHODS
        // =============================================

        public DataTable GetAllDoctors()
        {
            return ExecuteDataTableProcedure("sp_GetAllDoctors");
        }

        public DataTable GetDoctorById(string doctorId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_doctor_id", doctorId }
            };
            return ExecuteDataTableProcedure("sp_GetDoctorById", parameters);
        }

        public MySqlDataReader GetDoctorDetails(string doctorId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_doctor_id", doctorId }
            };
            return ExecuteReaderProcedure("sp_GetDoctorDetails", parameters);
        }

        public int AddDoctor(string firstName, string lastName, string specialization, string licenseNumber, string phone, string email, object departmentId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_first_name", firstName },
                { "p_last_name", lastName },
                { "p_specialization", specialization },
                { "p_license_number", licenseNumber },
                { "p_contact_number", phone },
                { "p_email", email },
                { "p_department_id", departmentId ?? DBNull.Value },
                { "p_result_code", MySqlDbType.Int32 },
                { "p_result_message", MySqlDbType.VarChar }
            };
            return ExecuteStoredProcedureWithOutput("sp_AddDoctor", parameters);
        }

        public int UpdateDoctor(string doctorId, string firstName, string lastName, string specialization, string licenseNumber, string phone, string email, object departmentId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_doctor_id", doctorId },
                { "p_first_name", firstName },
                { "p_last_name", lastName },
                { "p_specialization", specialization },
                { "p_license_number", licenseNumber },
                { "p_contact_number", phone },
                { "p_email", email },
                { "p_department_id", departmentId ?? DBNull.Value },
                { "p_result_code", MySqlDbType.Int32 },
                { "p_result_message", MySqlDbType.VarChar }
            };
            return ExecuteStoredProcedureWithOutput("sp_UpdateDoctor", parameters);
        }

        public int DeleteDoctor(string doctorId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_doctor_id", doctorId },
                { "p_result_code", MySqlDbType.Int32 },
                { "p_result_message", MySqlDbType.VarChar }
            };
            return ExecuteStoredProcedureWithOutput("sp_DeleteDoctor", parameters);
        }

        public DataTable GetDoctorsForAutoComplete(string searchText)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_search_text", searchText }
            };
            return ExecuteDataTableProcedure("sp_GetDoctorsForAutoComplete", parameters);
        }

        public DataTable GetAllDoctorNames()
        {
            return ExecuteDataTableProcedure("sp_GetAllDoctorNames");
        }

        public int GetDoctorIdByName(string doctorName)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_doctor_name", doctorName }
            };
            var result = ExecuteScalarProcedure("sp_GetDoctorIdByName", parameters);
            return result != null ? Convert.ToInt32(result) : -1;
        }

        public DataTable GetAllDepartments()
        {
            return ExecuteDataTableProcedure("sp_GetAllDepartments");
        }


        // =============================================
        // APPOINTMENT METHODS
        // =============================================

        public DataTable GetAllAppointments()
        {
            return ExecuteDataTableProcedure("sp_GetAllAppointments");
        }

        public DataTable GetAppointmentById(string appointmentId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_appointment_id", appointmentId }
            };
            return ExecuteDataTableProcedure("sp_GetAppointmentById", parameters);
        }

        public MySqlDataReader GetAppointmentDetails(string appointmentId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_appointment_id", appointmentId }
            };
            return ExecuteReaderProcedure("sp_GetAppointmentDetails", parameters);
        }

        public int AddAppointment(string patientId, string doctorId, DateTime date, TimeSpan time, string status)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_patient_id", patientId },
                { "p_doctor_id", doctorId },
                { "p_appointment_date", date },
                { "p_appointment_time", time },
                { "p_appointment_status", status },
                { "p_appointment_id", MySqlDbType.Int32 },
                { "p_result_code", MySqlDbType.Int32 },
                { "p_result_message", MySqlDbType.VarChar }
            };
            return ExecuteStoredProcedureWithOutput("sp_AddAppointment", parameters);
        }

        public int UpdateAppointment(string appointmentId, string patientId, string doctorId, DateTime date, TimeSpan time, string status)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_appointment_id", appointmentId },
                { "p_patient_id", patientId },
                { "p_doctor_id", doctorId },
                { "p_appointment_date", date },
                { "p_appointment_time", time },
                { "p_appointment_status", status },
                { "p_result_code", MySqlDbType.Int32 },
                { "p_result_message", MySqlDbType.VarChar }
            };
            return ExecuteStoredProcedureWithOutput("sp_UpdateAppointment", parameters);
        }

        public int DeleteAppointment(string appointmentId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_appointment_id", appointmentId },
                { "p_result_code", MySqlDbType.Int32 },
                { "p_result_message", MySqlDbType.VarChar }
            };
            return ExecuteStoredProcedureWithOutput("sp_DeleteAppointment", parameters);
        }

        public bool IsDoctorAvailable(string doctorId, DateTime date, TimeSpan time, string excludeAppointmentId = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_doctor_id", doctorId },
                { "p_appointment_date", date },
                { "p_appointment_time", time },
                { "p_exclude_appointment_id", excludeAppointmentId ?? "" },
                { "p_is_available", MySqlDbType.Int32 }
            };
            ExecuteStoredProcedureWithOutput("sp_IsDoctorAvailable", parameters);

            if (parameters.ContainsKey("p_is_available"))
            {
                return Convert.ToInt32(parameters["p_is_available"]) == 1;
            }
            return false;
        }


        // =============================================
        // BILLING METHODS
        // =============================================

        public DataTable GetAllBillingRecords()
        {
            return ExecuteDataTableProcedure("sp_GetAllBillingRecords");
        }

        public MySqlDataReader GetBillingDetails(string billingId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_billing_id", billingId }
            };
            return ExecuteReaderProcedure("sp_GetBillingDetails", parameters);
        }

        public int AddBilling(string patientId, string appointmentId, object insuranceId, decimal totalAmount, decimal insuranceCoverage, decimal patientBalance, string paymentStatus, string paymentMethod)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_patient_id", patientId },
                { "p_appointment_id", appointmentId },
                { "p_insurance_id", insuranceId ?? DBNull.Value },
                { "p_total_amount", totalAmount },
                { "p_insurance_coverage", insuranceCoverage },
                { "p_patient_balance", patientBalance },
                { "p_payment_status", paymentStatus },
                { "p_payment_method", paymentMethod ?? "" },
                { "p_billing_id", MySqlDbType.Int32 },
                { "p_result_code", MySqlDbType.Int32 },
                { "p_result_message", MySqlDbType.VarChar }
            };
            return ExecuteStoredProcedureWithOutput("sp_AddBilling", parameters);
        }

        public int UpdateBilling(string billingId, object insuranceId, decimal totalAmount, decimal insuranceCoverage, decimal patientBalance, string paymentStatus, string paymentMethod)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_billing_id", billingId },
                { "p_insurance_id", insuranceId ?? DBNull.Value },
                { "p_total_amount", totalAmount },
                { "p_insurance_coverage", insuranceCoverage },
                { "p_patient_balance", patientBalance },
                { "p_payment_status", paymentStatus },
                { "p_payment_method", paymentMethod ?? "" },
                { "p_result_code", MySqlDbType.Int32 },
                { "p_result_message", MySqlDbType.VarChar }
            };
            return ExecuteStoredProcedureWithOutput("sp_UpdateBilling", parameters);
        }

        public int DeleteBilling(string billingId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_billing_id", billingId },
                { "p_result_code", MySqlDbType.Int32 },
                { "p_result_message", MySqlDbType.VarChar }
            };
            return ExecuteStoredProcedureWithOutput("sp_DeleteBilling", parameters);
        }


        // =============================================
        // MEDICAL RECORD METHODS
        // =============================================

        public DataTable GetAllMedicalRecords()
        {
            return ExecuteDataTableProcedure("sp_GetAllMedicalRecords");
        }

        public DataTable GetMedicalRecordById(string recordId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_record_id", recordId }
            };
            return ExecuteDataTableProcedure("sp_GetMedicalRecordById", parameters);
        }

        public MySqlDataReader GetMedicalRecordDetails(string recordId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_record_id", recordId }
            };
            return ExecuteReaderProcedure("sp_GetMedicalRecordDetails", parameters);
        }

        public int AddMedicalRecord(string patientId, string doctorId, string diagnosis, string treatment, string prescription, DateTime dateRecorded)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_patient_id", patientId },
                { "p_doctor_id", doctorId },
                { "p_diagnosis", diagnosis },
                { "p_treatment", treatment ?? "" },
                { "p_prescription", prescription ?? "" },
                { "p_date_recorded", dateRecorded },
                { "p_record_id", MySqlDbType.Int32 },
                { "p_result_code", MySqlDbType.Int32 },
                { "p_result_message", MySqlDbType.VarChar }
            };
            return ExecuteStoredProcedureWithOutput("sp_AddMedicalRecord", parameters);
        }

        public int UpdateMedicalRecord(string recordId, string patientId, string doctorId, string diagnosis, string treatment, string prescription, DateTime dateRecorded)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_record_id", recordId },
                { "p_patient_id", patientId },
                { "p_doctor_id", doctorId },
                { "p_diagnosis", diagnosis },
                { "p_treatment", treatment ?? "" },
                { "p_prescription", prescription ?? "" },
                { "p_date_recorded", dateRecorded },
                { "p_result_code", MySqlDbType.Int32 },
                { "p_result_message", MySqlDbType.VarChar }
            };
            return ExecuteStoredProcedureWithOutput("sp_UpdateMedicalRecord", parameters);
        }

        public int DeleteMedicalRecord(string recordId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "p_record_id", recordId },
                { "p_result_code", MySqlDbType.Int32 },
                { "p_result_message", MySqlDbType.VarChar }
            };
            return ExecuteStoredProcedureWithOutput("sp_DeleteMedicalRecord", parameters);
        }
    }
}