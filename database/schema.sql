CREATE TABLE patients (
id INTEGER PRIMARY KEY,
name TEXT,
surname TEXT,
pesel TEXT,
phone TEXT,
email TEXT,
address TEXT,
warnings INTEGER DEFAULT 0,
blocked_until DATE
);

CREATE TABLE doctors (
id INTEGER PRIMARY KEY,
name TEXT,
specialization TEXT
);

CREATE TABLE appointments (
id INTEGER PRIMARY KEY,
doctor_id INTEGER,
patient_id INTEGER,
date DATE,
time TIME,
status TEXT,
cancel_reason TEXT
);

CREATE TABLE schedules (
id INTEGER PRIMARY KEY,
doctor_id INTEGER,
day_of_week TEXT,
start_time TIME,
end_time TIME
);

CREATE TABLE warnings (
id INTEGER PRIMARY KEY,
patient_id INTEGER,
date DATE,
reason TEXT
);