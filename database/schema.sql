PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS patients (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    pesel TEXT NOT NULL UNIQUE,
    phone TEXT,
    email TEXT,
    address TEXT,
    warning_count INTEGER NOT NULL DEFAULT 0,
    blocked_until TEXT
);

CREATE TABLE IF NOT EXISTS doctors (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    specialization TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS appointments (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    doctor_id INTEGER NOT NULL,
    patient_id INTEGER,
    start_at TEXT NOT NULL,
    status TEXT NOT NULL,
    cancel_reason TEXT,
    notes TEXT,
    FOREIGN KEY (doctor_id) REFERENCES doctors(id),
    FOREIGN KEY (patient_id) REFERENCES patients(id)
);

CREATE TABLE IF NOT EXISTS schedules (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    doctor_id INTEGER NOT NULL,
    day_of_week INTEGER NOT NULL,
    start_time TEXT NOT NULL,
    end_time TEXT NOT NULL,
    FOREIGN KEY (doctor_id) REFERENCES doctors(id)
);

CREATE TABLE IF NOT EXISTS employees (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    login TEXT NOT NULL UNIQUE,
    first_name TEXT,
    last_name TEXT,
    pesel TEXT,
    birth_date TEXT,
    display_name TEXT NOT NULL,
    role TEXT NOT NULL,
    password_hash TEXT NOT NULL,
    password_salt TEXT NOT NULL,
    created_at TEXT NOT NULL,
    is_active INTEGER NOT NULL DEFAULT 1,
    is_doctor INTEGER NOT NULL DEFAULT 0,
    specialization TEXT
);

CREATE INDEX IF NOT EXISTS idx_patients_search
    ON patients(pesel, phone, email);

CREATE INDEX IF NOT EXISTS idx_appointments_doctor_date
    ON appointments(doctor_id, start_at, status);

CREATE INDEX IF NOT EXISTS idx_appointments_patient
    ON appointments(patient_id, start_at, status);

CREATE INDEX IF NOT EXISTS idx_employees_search
    ON employees(first_name, last_name, pesel, birth_date, login);
