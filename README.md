-- =====================================================
-- PATIENT AUDIT LOG TABLE
-- =====================================================

CREATE TABLE IF NOT EXISTS patient_audit_log (
    log_id INT AUTO_INCREMENT PRIMARY KEY,
    patient_id VARCHAR(20),
    patient_name VARCHAR(255),
    action VARCHAR(50),
    deleted_by VARCHAR(100),
    deleted_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    details TEXT,
    ip_address VARCHAR(50)
);

CREATE INDEX idx_patient_audit_patient_id ON patient_audit_log(patient_id);
CREATE INDEX idx_patient_audit_deleted_at ON patient_audit_log(deleted_at);

-- =====================================================
-- TRIGGER: Log Patient Deletions
-- =====================================================

DELIMITER //

DROP TRIGGER IF EXISTS trg_LogPatientDeletion //

CREATE TRIGGER trg_LogPatientDeletion
BEFORE DELETE ON patient
FOR EACH ROW
BEGIN
    INSERT INTO patient_audit_log (
        patient_id, 
        patient_name, 
        action, 
        deleted_by,
        details
    ) VALUES (
        OLD.patient_id, 
        CONCAT(OLD.first_name, ' ', OLD.last_name), 
        'DELETE', 
        USER(),
        CONCAT('Email: ', OLD.email, ', Phone: ', OLD.contact_no, ', Gender: ', OLD.gender)
    );
END //

DELIMITER ;

-- =====================================================
-- TRIGGER: Log Patient Updates
-- =====================================================

DELIMITER //

DROP TRIGGER IF EXISTS trg_LogPatientUpdate //

CREATE TRIGGER trg_LogPatientUpdate
AFTER UPDATE ON patient
FOR EACH ROW
BEGIN
    DECLARE v_changes TEXT;
    SET v_changes = '';
    
    IF OLD.first_name != NEW.first_name THEN
        SET v_changes = CONCAT(v_changes, 'First Name: ', OLD.first_name, ' -> ', NEW.first_name, '; ');
    END IF;
    
    IF OLD.last_name != NEW.last_name THEN
        SET v_changes = CONCAT(v_changes, 'Last Name: ', OLD.last_name, ' -> ', NEW.last_name, '; ');
    END IF;
    
    IF OLD.email != NEW.email THEN
        SET v_changes = CONCAT(v_changes, 'Email: ', OLD.email, ' -> ', NEW.email, '; ');
    END IF;
    
    IF OLD.contact_no != NEW.contact_no THEN
        SET v_changes = CONCAT(v_changes, 'Phone: ', OLD.contact_no, ' -> ', NEW.contact_no, '; ');
    END IF;
    
    IF OLD.address != NEW.address THEN
        SET v_changes = CONCAT(v_changes, 'Address: ', OLD.address, ' -> ', NEW.address, '; ');
    END IF;
    
    IF OLD.gender != NEW.gender THEN
        SET v_changes = CONCAT(v_changes, 'Gender: ', OLD.gender, ' -> ', NEW.gender, '; ');
    END IF;
    
    IF v_changes != '' THEN
        INSERT INTO patient_audit_log (
            patient_id, 
            patient_name, 
            action, 
            deleted_by,
            details
        ) VALUES (
            NEW.patient_id, 
            CONCAT(NEW.first_name, ' ', NEW.last_name), 
            'UPDATE', 
            USER(),
            v_changes
        );
    END IF;
END //

DELIMITER ;

-- =====================================================
-- TRIGGER: Log Patient Inserts
-- =====================================================

DELIMITER //

DROP TRIGGER IF EXISTS trg_LogPatientInsert //

CREATE TRIGGER trg_LogPatientInsert
AFTER INSERT ON patient
FOR EACH ROW
BEGIN
    INSERT INTO patient_audit_log (
        patient_id, 
        patient_name, 
        action, 
        deleted_by,
        details
    ) VALUES (
        NEW.patient_id, 
        CONCAT(NEW.first_name, ' ', NEW.last_name), 
        'INSERT', 
        USER(),
        CONCAT('Email: ', NEW.email, ', Phone: ', NEW.contact_no)
    );
END //

DELIMITER ;

-- =====================================================
-- TRIGGER: Validate Phone Number on Insert
-- =====================================================

DELIMITER //

DROP TRIGGER IF EXISTS trg_ValidatePhoneNumberInsert //

CREATE TRIGGER trg_ValidatePhoneNumberInsert
BEFORE INSERT ON patient
FOR EACH ROW
BEGIN
    DECLARE v_clean_phone VARCHAR(20);
    
    IF NEW.contact_no IS NOT NULL AND NEW.contact_no != '' THEN
        SET v_clean_phone = REGEXP_REPLACE(NEW.contact_no, '[^0-9]', '');
        
        IF LENGTH(v_clean_phone) < 7 THEN
            SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Phone number must contain at least 7 digits.';
        END IF;
        
        IF v_clean_phone NOT REGEXP '^[0-9]+$' THEN
            SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Phone number can only contain digits.';
        END IF;
    END IF;
END //

DELIMITER ;

-- =====================================================
-- TRIGGER: Validate Phone Number on Update
-- =====================================================

DELIMITER //

DROP TRIGGER IF EXISTS trg_ValidatePhoneNumberUpdate //

CREATE TRIGGER trg_ValidatePhoneNumberUpdate
BEFORE UPDATE ON patient
FOR EACH ROW
BEGIN
    DECLARE v_clean_phone VARCHAR(20);
    
    IF NEW.contact_no IS NOT NULL AND NEW.contact_no != '' THEN
        SET v_clean_phone = REGEXP_REPLACE(NEW.contact_no, '[^0-9]', '');
        
        IF LENGTH(v_clean_phone) < 7 THEN
            SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Phone number must contain at least 7 digits.';
        END IF;
        
        IF v_clean_phone NOT REGEXP '^[0-9]+$' THEN
            SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Phone number can only contain digits.';
        END IF;
    END IF;
END //

DELIMITER ;




-- =====================================================
-- DOCTOR AUDIT LOG TABLE
-- =====================================================

CREATE TABLE IF NOT EXISTS doctor_audit_log (
    log_id INT AUTO_INCREMENT PRIMARY KEY,
    doctor_id VARCHAR(20),
    doctor_name VARCHAR(255),
    action VARCHAR(50),
    performed_by VARCHAR(100),
    performed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    details TEXT,
    ip_address VARCHAR(50)
);

CREATE INDEX idx_doctor_audit_doctor_id ON doctor_audit_log(doctor_id);
CREATE INDEX idx_doctor_audit_performed_at ON doctor_audit_log(performed_at);
CREATE INDEX idx_doctor_audit_action ON doctor_audit_log(action);

-- =====================================================
-- TRIGGER: Log Doctor Deletions
-- =====================================================

DELIMITER //

DROP TRIGGER IF EXISTS trg_LogDoctorDeletion //

CREATE TRIGGER trg_LogDoctorDeletion
BEFORE DELETE ON doctor
FOR EACH ROW
BEGIN
    INSERT INTO doctor_audit_log (
        doctor_id, 
        doctor_name, 
        action, 
        performed_by,
        details
    ) VALUES (
        OLD.doctor_id, 
        CONCAT(OLD.first_name, ' ', OLD.last_name), 
        'DELETE', 
        USER(),
        CONCAT('License: ', OLD.license_number, ', Email: ', OLD.email, ', Specialization: ', OLD.specialization)
    );
END //

DELIMITER ;

-- =====================================================
-- TRIGGER: Log Doctor Updates
-- =====================================================

DELIMITER //

DROP TRIGGER IF EXISTS trg_LogDoctorUpdate //

CREATE TRIGGER trg_LogDoctorUpdate
AFTER UPDATE ON doctor
FOR EACH ROW
BEGIN
    DECLARE v_changes TEXT;
    SET v_changes = '';
    
    IF OLD.first_name != NEW.first_name THEN
        SET v_changes = CONCAT(v_changes, 'First Name: ', OLD.first_name, ' -> ', NEW.first_name, '; ');
    END IF;
    
    IF OLD.last_name != NEW.last_name THEN
        SET v_changes = CONCAT(v_changes, 'Last Name: ', OLD.last_name, ' -> ', NEW.last_name, '; ');
    END IF;
    
    IF OLD.specialization != NEW.specialization THEN
        SET v_changes = CONCAT(v_changes, 'Specialization: ', OLD.specialization, ' -> ', NEW.specialization, '; ');
    END IF;
    
    IF OLD.license_number != NEW.license_number THEN
        SET v_changes = CONCAT(v_changes, 'License: ', OLD.license_number, ' -> ', NEW.license_number, '; ');
    END IF;
    
    IF OLD.email != NEW.email THEN
        SET v_changes = CONCAT(v_changes, 'Email: ', OLD.email, ' -> ', NEW.email, '; ');
    END IF;
    
    IF OLD.contact_number != NEW.contact_number THEN
        SET v_changes = CONCAT(v_changes, 'Phone: ', OLD.contact_number, ' -> ', NEW.contact_number, '; ');
    END IF;
    
    IF OLD.department_id != NEW.department_id THEN
        SET v_changes = CONCAT(v_changes, 'Department: ', OLD.department_id, ' -> ', NEW.department_id, '; ');
    END IF;
    
    IF v_changes != '' THEN
        INSERT INTO doctor_audit_log (
            doctor_id, 
            doctor_name, 
            action, 
            performed_by,
            details
        ) VALUES (
            NEW.doctor_id, 
            CONCAT(NEW.first_name, ' ', NEW.last_name), 
            'UPDATE', 
            USER(),
            v_changes
        );
    END IF;
END //

DELIMITER ;

-- =====================================================
-- TRIGGER: Log Doctor Inserts
-- =====================================================

DELIMITER //

DROP TRIGGER IF EXISTS trg_LogDoctorInsert //

CREATE TRIGGER trg_LogDoctorInsert
AFTER INSERT ON doctor
FOR EACH ROW
BEGIN
    INSERT INTO doctor_audit_log (
        doctor_id, 
        doctor_name, 
        action, 
        performed_by,
        details
    ) VALUES (
        NEW.doctor_id, 
        CONCAT(NEW.first_name, ' ', NEW.last_name), 
        'INSERT', 
        USER(),
        CONCAT('License: ', NEW.license_number, ', Email: ', NEW.email, ', Specialization: ', NEW.specialization)
    );
END //

DELIMITER ;

-- =====================================================
-- APPOINTMENT AUDIT LOG TABLE
-- =====================================================

CREATE TABLE IF NOT EXISTS appointment_audit_log (
    log_id INT AUTO_INCREMENT PRIMARY KEY,
    appointment_id VARCHAR(20),
    patient_name VARCHAR(255),
    doctor_name VARCHAR(255),
    appointment_date DATE,
    appointment_time TIME,
    appointment_status VARCHAR(50),
    action VARCHAR(50),
    performed_by VARCHAR(100),
    performed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    details TEXT
);

CREATE INDEX idx_appointment_audit_appointment_id ON appointment_audit_log(appointment_id);
CREATE INDEX idx_appointment_audit_performed_at ON appointment_audit_log(performed_at);
CREATE INDEX idx_appointment_audit_action ON appointment_audit_log(action);

-- =====================================================
-- TRIGGER: Log Appointment Deletions
-- =====================================================

DELIMITER //

DROP TRIGGER IF EXISTS trg_LogAppointmentDeletion //

CREATE TRIGGER trg_LogAppointmentDeletion
BEFORE DELETE ON appointment
FOR EACH ROW
BEGIN
    DECLARE v_patient_name VARCHAR(255);
    DECLARE v_doctor_name VARCHAR(255);
    
    SELECT CONCAT(first_name, ' ', last_name) INTO v_patient_name
    FROM patient WHERE patient_id = OLD.patient_id;
    
    SELECT CONCAT(first_name, ' ', last_name) INTO v_doctor_name
    FROM doctor WHERE doctor_id = OLD.doctor_id;
    
    INSERT INTO appointment_audit_log (
        appointment_id,
        patient_name,
        doctor_name,
        appointment_date,
        appointment_time,
        appointment_status,
        action,
        performed_by,
        details
    ) VALUES (
        OLD.appointment_id,
        v_patient_name,
        v_doctor_name,
        OLD.appointment_date,
        OLD.appointment_time,
        OLD.appointment_status,
        'DELETE',
        USER(),
        CONCAT('Patient: ', v_patient_name, ', Doctor: ', v_doctor_name, ', Status: ', OLD.appointment_status)
    );
END //

DELIMITER ;

-- =====================================================
-- TRIGGER: Log Appointment Updates
-- =====================================================

DELIMITER //

DROP TRIGGER IF EXISTS trg_LogAppointmentUpdate //

CREATE TRIGGER trg_LogAppointmentUpdate
AFTER UPDATE ON appointment
FOR EACH ROW
BEGIN
    DECLARE v_patient_name VARCHAR(255);
    DECLARE v_doctor_name VARCHAR(255);
    DECLARE v_changes TEXT;
    SET v_changes = '';
    
    SELECT CONCAT(first_name, ' ', last_name) INTO v_patient_name
    FROM patient WHERE patient_id = NEW.patient_id;
    
    SELECT CONCAT(first_name, ' ', last_name) INTO v_doctor_name
    FROM doctor WHERE doctor_id = NEW.doctor_id;
    
    IF OLD.patient_id != NEW.patient_id THEN
        SET v_changes = CONCAT(v_changes, 'Patient ID: ', OLD.patient_id, ' -> ', NEW.patient_id, '; ');
    END IF;
    
    IF OLD.doctor_id != NEW.doctor_id THEN
        SET v_changes = CONCAT(v_changes, 'Doctor ID: ', OLD.doctor_id, ' -> ', NEW.doctor_id, '; ');
    END IF;
    
    IF OLD.appointment_date != NEW.appointment_date THEN
        SET v_changes = CONCAT(v_changes, 'Date: ', OLD.appointment_date, ' -> ', NEW.appointment_date, '; ');
    END IF;
    
    IF OLD.appointment_time != NEW.appointment_time THEN
        SET v_changes = CONCAT(v_changes, 'Time: ', OLD.appointment_time, ' -> ', NEW.appointment_time, '; ');
    END IF;
    
    IF OLD.appointment_status != NEW.appointment_status THEN
        SET v_changes = CONCAT(v_changes, 'Status: ', OLD.appointment_status, ' -> ', NEW.appointment_status, '; ');
    END IF;
    
    IF v_changes != '' THEN
        INSERT INTO appointment_audit_log (
            appointment_id,
            patient_name,
            doctor_name,
            appointment_date,
            appointment_time,
            appointment_status,
            action,
            performed_by,
            details
        ) VALUES (
            NEW.appointment_id,
            v_patient_name,
            v_doctor_name,
            NEW.appointment_date,
            NEW.appointment_time,
            NEW.appointment_status,
            'UPDATE',
            USER(),
            v_changes
        );
    END IF;
END //

DELIMITER ;

-- =====================================================
-- TRIGGER: Log Appointment Inserts
-- =====================================================

DELIMITER //

DROP TRIGGER IF EXISTS trg_LogAppointmentInsert //

CREATE TRIGGER trg_LogAppointmentInsert
AFTER INSERT ON appointment
FOR EACH ROW
BEGIN
    DECLARE v_patient_name VARCHAR(255);
    DECLARE v_doctor_name VARCHAR(255);
    
    SELECT CONCAT(first_name, ' ', last_name) INTO v_patient_name
    FROM patient WHERE patient_id = NEW.patient_id;
    
    SELECT CONCAT(first_name, ' ', last_name) INTO v_doctor_name
    FROM doctor WHERE doctor_id = NEW.doctor_id;
    
    INSERT INTO appointment_audit_log (
        appointment_id,
        patient_name,
        doctor_name,
        appointment_date,
        appointment_time,
        appointment_status,
        action,
        performed_by,
        details
    ) VALUES (
        NEW.appointment_id,
        v_patient_name,
        v_doctor_name,
        NEW.appointment_date,
        NEW.appointment_time,
        NEW.appointment_status,
        'INSERT',
        USER(),
        CONCAT('Patient: ', v_patient_name, ', Doctor: ', v_doctor_name, ', Status: ', NEW.appointment_status)
    );
END //

DELIMITER ;
