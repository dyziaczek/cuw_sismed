using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace VS_CUWSISMED
{
    public interface IClinicDataStore
    {
        IReadOnlyList<Doctor> GetDoctors();
        IReadOnlyList<AvailableSlot> GetAvailableSlots(int doctorId, DateTime date);
        IReadOnlyList<Appointment> GetAppointmentsForDoctor(int doctorId, DateTime date);
        IReadOnlyList<Appointment> GetAppointmentsForPatient(int patientId);
        Patient FindPatient(string query);
        Patient AddPatient(Patient patient);
        Patient GetPatient(int patientId);
        Doctor GetDoctor(int doctorId);
        Appointment ReserveAppointment(int doctorId, int patientId, DateTime startAt);
        void CancelAppointment(int appointmentId, string reason);
        void SwapAppointmentPatient(int appointmentId, int newPatientId);
        Employee FindEmployeeByLogin(string login);
        Employee AddEmployee(Employee employee);
    }

    internal static class AppServices
    {
        private static readonly Lazy<IClinicDataStore> Store =
            new Lazy<IClinicDataStore>(DataStoreFactory.Create);

        private static readonly Lazy<AuthService> Auth =
            new Lazy<AuthService>(() => new AuthService(DataStore));

        public static string StorageInfo { get; internal set; }

        public static IClinicDataStore DataStore
        {
            get { return Store.Value; }
        }

        public static AuthService AuthService
        {
            get { return Auth.Value; }
        }
    }

    internal static class DataStoreFactory
    {
        public static IClinicDataStore Create()
        {
            string mode = (ConfigurationManager.AppSettings["SismedStorageMode"] ?? "Memory").Trim();

            if (mode.Equals("SQLite", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string dbPath = ResolveDatabasePath(
                        ConfigurationManager.AppSettings["SismedDatabasePath"]);
                    AppServices.StorageInfo = "SQLite: " + dbPath;
                    return new SqliteClinicDataStore(dbPath);
                }
                catch (Exception ex)
                {
                    AppServices.StorageInfo =
                        "SQLite niedostepne (" + ex.Message + "), uzyto danych pamieciowych.";
                }
            }

            if (string.IsNullOrWhiteSpace(AppServices.StorageInfo))
            {
                AppServices.StorageInfo = "Dane pamieciowe";
            }

            return new InMemoryClinicDataStore(SampleData.Create());
        }

        private static string ResolveDatabasePath(string configuredPath)
        {
            string path = string.IsNullOrWhiteSpace(configuredPath)
                ? "%LOCALAPPDATA%\\CUW_SISMED\\sismed.db"
                : configuredPath;

            path = Environment.ExpandEnvironmentVariables(path);
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }

            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return path;
        }
    }

    public sealed class AuthService
    {
        private readonly IClinicDataStore dataStore;

        public AuthService(IClinicDataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        public Employee Authenticate(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            Employee employee = dataStore.FindEmployeeByLogin(login.Trim());
            if (employee == null || !employee.IsActive)
            {
                return null;
            }

            return PasswordHasher.Verify(password, employee.PasswordSalt, employee.PasswordHash)
                ? employee
                : null;
        }

        public RegistrationResult RegisterEmployee(
            string login,
            string displayName,
            string role,
            string password,
            string repeatedPassword)
        {
            login = (login ?? string.Empty).Trim();
            displayName = (displayName ?? string.Empty).Trim();
            role = string.IsNullOrWhiteSpace(role) ? "Rejestracja" : role.Trim();

            if (login.Length < 3)
            {
                return RegistrationResult.Fail("Login musi miec co najmniej 3 znaki.");
            }

            if (displayName.Length < 3)
            {
                return RegistrationResult.Fail("Podaj imie i nazwisko pracownika.");
            }

            if (string.IsNullOrEmpty(password) || password.Length < 4)
            {
                return RegistrationResult.Fail("Haslo musi miec co najmniej 4 znaki.");
            }

            if (!string.Equals(password, repeatedPassword, StringComparison.Ordinal))
            {
                return RegistrationResult.Fail("Hasla nie sa identyczne.");
            }

            if (dataStore.FindEmployeeByLogin(login) != null)
            {
                return RegistrationResult.Fail("Pracownik o takim loginie juz istnieje.");
            }

            string salt;
            string hash = PasswordHasher.CreateHash(password, out salt);

            var employee = dataStore.AddEmployee(new Employee
            {
                Login = login,
                DisplayName = displayName,
                Role = role,
                PasswordSalt = salt,
                PasswordHash = hash,
                CreatedAt = DateTime.Now,
                IsActive = true
            });

            return RegistrationResult.Ok(employee);
        }
    }

    internal static class PasswordHasher
    {
        private const int Iterations = 10000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        public static string CreateHash(string password, out string salt)
        {
            byte[] saltBytes = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            salt = Convert.ToBase64String(saltBytes);
            return Hash(password, saltBytes);
        }

        public static bool Verify(string password, string salt, string expectedHash)
        {
            if (string.IsNullOrEmpty(salt) || string.IsNullOrEmpty(expectedHash))
            {
                return false;
            }

            byte[] saltBytes = Convert.FromBase64String(salt);
            string actualHash = Hash(password, saltBytes);
            return SlowEquals(actualHash, expectedHash);
        }

        private static string Hash(string password, byte[] saltBytes)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(
                password ?? string.Empty,
                saltBytes,
                Iterations))
            {
                return Convert.ToBase64String(deriveBytes.GetBytes(HashSize));
            }
        }

        private static bool SlowEquals(string left, string right)
        {
            byte[] leftBytes = Encoding.UTF8.GetBytes(left ?? string.Empty);
            byte[] rightBytes = Encoding.UTF8.GetBytes(right ?? string.Empty);
            int diff = leftBytes.Length ^ rightBytes.Length;

            for (int i = 0; i < leftBytes.Length && i < rightBytes.Length; i++)
            {
                diff |= leftBytes[i] ^ rightBytes[i];
            }

            return diff == 0;
        }
    }

    internal sealed class InMemoryClinicDataStore : IClinicDataStore
    {
        private readonly List<Patient> patients;
        private readonly List<Doctor> doctors;
        private readonly List<ScheduleEntry> schedules;
        private readonly List<Appointment> appointments;
        private readonly List<Employee> employees;
        private int nextPatientId;
        private int nextAppointmentId;
        private int nextEmployeeId;

        public InMemoryClinicDataStore(ClinicSeedData seed)
        {
            patients = seed.Patients;
            doctors = seed.Doctors;
            schedules = seed.Schedules;
            appointments = seed.Appointments;
            employees = seed.Employees;
            nextPatientId = NextId(patients.Select(p => p.Id));
            nextAppointmentId = NextId(appointments.Select(a => a.Id));
            nextEmployeeId = NextId(employees.Select(e => e.Id));
        }

        public IReadOnlyList<Doctor> GetDoctors()
        {
            return doctors.OrderBy(d => d.LastName).ThenBy(d => d.FirstName).ToList();
        }

        public IReadOnlyList<AvailableSlot> GetAvailableSlots(int doctorId, DateTime date)
        {
            Doctor doctor = GetDoctor(doctorId);
            if (doctor == null)
            {
                return new List<AvailableSlot>();
            }

            return GetAppointmentsForDoctor(doctorId, date)
                .Where(a => a.Status == AppointmentStatus.Free && a.StartAt >= DateTime.Now)
                .Select(a => new AvailableSlot { Doctor = doctor, StartAt = a.StartAt })
                .ToList();
        }

        public IReadOnlyList<Appointment> GetAppointmentsForDoctor(int doctorId, DateTime date)
        {
            var reserved = appointments
                .Where(a => a.DoctorId == doctorId
                    && a.StartAt.Date == date.Date
                    && a.Status == AppointmentStatus.Reserved)
                .ToList();

            var result = new List<Appointment>();
            foreach (DateTime slot in GenerateSlots(doctorId, date.Date))
            {
                Appointment appointment = reserved.FirstOrDefault(a => a.StartAt == slot);
                result.Add(appointment ?? new Appointment
                {
                    DoctorId = doctorId,
                    StartAt = slot,
                    Status = AppointmentStatus.Free
                });
            }

            return result;
        }

        public IReadOnlyList<Appointment> GetAppointmentsForPatient(int patientId)
        {
            return appointments
                .Where(a => a.PatientId == patientId && a.Status == AppointmentStatus.Reserved)
                .OrderBy(a => a.StartAt)
                .ToList();
        }

        public Patient FindPatient(string query)
        {
            string normalized = Normalize(query);
            if (string.IsNullOrEmpty(normalized))
            {
                return null;
            }

            return patients.FirstOrDefault(p =>
                Normalize(p.Pesel).Contains(normalized)
                || Normalize(p.Phone).Contains(normalized)
                || Normalize(p.Email).Contains(normalized)
                || Normalize(p.DisplayName).Contains(normalized));
        }

        public Patient AddPatient(Patient patient)
        {
            if (patient == null)
            {
                throw new ArgumentNullException("patient");
            }

            if (!string.IsNullOrWhiteSpace(patient.Pesel)
                && patients.Any(p => string.Equals(p.Pesel, patient.Pesel, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Pacjent o podanym numerze PESEL juz istnieje.");
            }

            patient.Id = nextPatientId++;
            patients.Add(patient);
            return patient;
        }

        public Patient GetPatient(int patientId)
        {
            return patients.FirstOrDefault(p => p.Id == patientId);
        }

        public Doctor GetDoctor(int doctorId)
        {
            return doctors.FirstOrDefault(d => d.Id == doctorId);
        }

        public Appointment ReserveAppointment(int doctorId, int patientId, DateTime startAt)
        {
            ValidateReservation(doctorId, patientId, startAt, 0);

            var appointment = new Appointment
            {
                Id = nextAppointmentId++,
                DoctorId = doctorId,
                PatientId = patientId,
                StartAt = startAt,
                Status = AppointmentStatus.Reserved
            };
            appointments.Add(appointment);
            return appointment;
        }

        public void CancelAppointment(int appointmentId, string reason)
        {
            Appointment appointment = appointments.FirstOrDefault(a => a.Id == appointmentId);
            if (appointment == null || appointment.Status != AppointmentStatus.Reserved)
            {
                return;
            }

            if (appointment.PatientId.HasValue
                && appointment.StartAt > DateTime.Now
                && appointment.StartAt.Subtract(DateTime.Now).TotalHours < 24)
            {
                AddWarning(appointment.PatientId.Value);
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancelReason = string.IsNullOrWhiteSpace(reason)
                ? "Anulowano przez pracownika"
                : reason.Trim();
        }

        public void SwapAppointmentPatient(int appointmentId, int newPatientId)
        {
            Appointment appointment = appointments.FirstOrDefault(a => a.Id == appointmentId);
            if (appointment == null || appointment.Status != AppointmentStatus.Reserved)
            {
                throw new InvalidOperationException("Nie wybrano aktywnej wizyty do zamiany.");
            }

            ValidateReservation(appointment.DoctorId, newPatientId, appointment.StartAt, appointment.Id);
            appointment.PatientId = newPatientId;
        }

        public Employee FindEmployeeByLogin(string login)
        {
            string normalized = Normalize(login);
            return employees.FirstOrDefault(e => Normalize(e.Login) == normalized);
        }

        public Employee AddEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException("employee");
            }

            if (FindEmployeeByLogin(employee.Login) != null)
            {
                throw new InvalidOperationException("Pracownik o takim loginie juz istnieje.");
            }

            employee.Id = nextEmployeeId++;
            employees.Add(employee);
            return employee;
        }

        private void ValidateReservation(int doctorId, int patientId, DateTime startAt, int ignoredAppointmentId)
        {
            Doctor doctor = GetDoctor(doctorId);
            Patient patient = GetPatient(patientId);

            if (doctor == null)
            {
                throw new InvalidOperationException("Nie znaleziono lekarza.");
            }

            if (patient == null)
            {
                throw new InvalidOperationException("Nie znaleziono pacjenta.");
            }

            if (patient.IsBlocked)
            {
                throw new InvalidOperationException(
                    "Pacjent ma aktywna blokade wizyt do "
                    + patient.BlockedUntil.Value.ToString("dd.MM.yyyy") + ".");
            }

            if (!IsDoctorWorking(doctorId, startAt))
            {
                throw new InvalidOperationException("Lekarz nie pracuje w wybranym terminie.");
            }

            if (IsSlotTaken(doctorId, startAt, ignoredAppointmentId))
            {
                throw new InvalidOperationException("Wybrany termin jest juz zajety.");
            }

            if (HasPatientConflict(patientId, doctor.Specialization, startAt, ignoredAppointmentId))
            {
                throw new InvalidOperationException(
                    "Pacjent ma juz wizyte w tym terminie lub u tej specjalizacji tego dnia.");
            }
        }

        private bool IsDoctorWorking(int doctorId, DateTime startAt)
        {
            TimeSpan time = startAt.TimeOfDay;
            return schedules.Any(s => s.DoctorId == doctorId
                && s.DayOfWeek == startAt.DayOfWeek
                && time >= s.StartTime
                && time < s.EndTime);
        }

        private bool IsSlotTaken(int doctorId, DateTime startAt, int ignoredAppointmentId)
        {
            return appointments.Any(a => a.Id != ignoredAppointmentId
                && a.DoctorId == doctorId
                && a.StartAt == startAt
                && a.Status == AppointmentStatus.Reserved);
        }

        private bool HasPatientConflict(
            int patientId,
            string specialization,
            DateTime startAt,
            int ignoredAppointmentId)
        {
            return appointments.Any(a =>
            {
                if (a.Id == ignoredAppointmentId
                    || a.PatientId != patientId
                    || a.Status != AppointmentStatus.Reserved)
                {
                    return false;
                }

                if (a.StartAt == startAt)
                {
                    return true;
                }

                Doctor doctor = GetDoctor(a.DoctorId);
                return doctor != null
                    && a.StartAt.Date == startAt.Date
                    && string.Equals(doctor.Specialization, specialization, StringComparison.OrdinalIgnoreCase);
            });
        }

        private IEnumerable<DateTime> GenerateSlots(int doctorId, DateTime date)
        {
            foreach (ScheduleEntry schedule in schedules.Where(s => s.DoctorId == doctorId && s.DayOfWeek == date.DayOfWeek))
            {
                DateTime current = date.Add(schedule.StartTime);
                DateTime end = date.Add(schedule.EndTime);
                while (current < end)
                {
                    yield return current;
                    current = current.AddMinutes(15);
                }
            }
        }

        private void AddWarning(int patientId)
        {
            Patient patient = GetPatient(patientId);
            if (patient == null)
            {
                return;
            }

            patient.WarningCount++;
            if (patient.WarningCount >= 3)
            {
                patient.BlockedUntil = DateTime.Today.AddDays(30);
            }
        }

        private static int NextId(IEnumerable<int> ids)
        {
            return ids.Any() ? ids.Max() + 1 : 1;
        }

        private static string Normalize(string value)
        {
            return (value ?? string.Empty).Trim().ToLowerInvariant();
        }
    }

    internal sealed class SqliteClinicDataStore : IClinicDataStore
    {
        private readonly DbProviderFactory factory;
        private readonly string connectionString;

        public SqliteClinicDataStore(string databasePath)
        {
            factory = LoadFactory();
            connectionString = "Data Source=" + databasePath + ";Version=3;";
            EnsureSchema();
            SeedIfEmpty();
        }

        public IReadOnlyList<Doctor> GetDoctors()
        {
            var doctors = new List<Doctor>();
            using (DbConnection connection = OpenConnection())
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id, first_name, last_name, specialization FROM doctors ORDER BY last_name, first_name";
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        doctors.Add(ReadDoctor(reader));
                    }
                }
            }

            return doctors;
        }

        public IReadOnlyList<AvailableSlot> GetAvailableSlots(int doctorId, DateTime date)
        {
            Doctor doctor = GetDoctor(doctorId);
            if (doctor == null)
            {
                return new List<AvailableSlot>();
            }

            return GetAppointmentsForDoctor(doctorId, date)
                .Where(a => a.Status == AppointmentStatus.Free && a.StartAt >= DateTime.Now)
                .Select(a => new AvailableSlot { Doctor = doctor, StartAt = a.StartAt })
                .ToList();
        }

        public IReadOnlyList<Appointment> GetAppointmentsForDoctor(int doctorId, DateTime date)
        {
            var booked = LoadAppointments(
                "doctor_id = @doctor_id AND substr(start_at, 1, 10) = @day AND status = @status",
                command =>
                {
                    AddParameter(command, "@doctor_id", doctorId);
                    AddParameter(command, "@day", date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                    AddParameter(command, "@status", AppointmentStatus.Reserved.ToString());
                });

            var result = new List<Appointment>();
            foreach (DateTime slot in GenerateSlots(doctorId, date.Date))
            {
                Appointment appointment = booked.FirstOrDefault(a => a.StartAt == slot);
                result.Add(appointment ?? new Appointment
                {
                    DoctorId = doctorId,
                    StartAt = slot,
                    Status = AppointmentStatus.Free
                });
            }

            return result;
        }

        public IReadOnlyList<Appointment> GetAppointmentsForPatient(int patientId)
        {
            return LoadAppointments(
                "patient_id = @patient_id AND status = @status",
                command =>
                {
                    AddParameter(command, "@patient_id", patientId);
                    AddParameter(command, "@status", AppointmentStatus.Reserved.ToString());
                })
                .OrderBy(a => a.StartAt)
                .ToList();
        }

        public Patient FindPatient(string query)
        {
            string normalized = (query ?? string.Empty).Trim().ToLowerInvariant();
            if (normalized.Length == 0)
            {
                return null;
            }

            using (DbConnection connection = OpenConnection())
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT id, first_name, last_name, pesel, phone, email, address, warning_count, blocked_until " +
                    "FROM patients " +
                    "WHERE lower(pesel) LIKE @q OR lower(phone) LIKE @q OR lower(email) LIKE @q " +
                    "OR lower(first_name || ' ' || last_name) LIKE @q " +
                    "ORDER BY last_name, first_name LIMIT 1";
                AddParameter(command, "@q", "%" + normalized + "%");

                using (DbDataReader reader = command.ExecuteReader())
                {
                    return reader.Read() ? ReadPatient(reader) : null;
                }
            }
        }

        public Patient AddPatient(Patient patient)
        {
            if (patient == null)
            {
                throw new ArgumentNullException("patient");
            }

            using (DbConnection connection = OpenConnection())
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "INSERT INTO patients(first_name, last_name, pesel, phone, email, address, warning_count, blocked_until) " +
                    "VALUES(@first_name, @last_name, @pesel, @phone, @email, @address, @warning_count, @blocked_until); " +
                    "SELECT last_insert_rowid();";
                AddPatientParameters(command, patient);
                patient.Id = Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture);
            }

            return patient;
        }

        public Patient GetPatient(int patientId)
        {
            using (DbConnection connection = OpenConnection())
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT id, first_name, last_name, pesel, phone, email, address, warning_count, blocked_until " +
                    "FROM patients WHERE id = @id";
                AddParameter(command, "@id", patientId);
                using (DbDataReader reader = command.ExecuteReader())
                {
                    return reader.Read() ? ReadPatient(reader) : null;
                }
            }
        }

        public Doctor GetDoctor(int doctorId)
        {
            using (DbConnection connection = OpenConnection())
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id, first_name, last_name, specialization FROM doctors WHERE id = @id";
                AddParameter(command, "@id", doctorId);
                using (DbDataReader reader = command.ExecuteReader())
                {
                    return reader.Read() ? ReadDoctor(reader) : null;
                }
            }
        }

        public Appointment ReserveAppointment(int doctorId, int patientId, DateTime startAt)
        {
            ValidateReservation(doctorId, patientId, startAt, 0);

            var appointment = new Appointment
            {
                DoctorId = doctorId,
                PatientId = patientId,
                StartAt = startAt,
                Status = AppointmentStatus.Reserved
            };

            using (DbConnection connection = OpenConnection())
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "INSERT INTO appointments(doctor_id, patient_id, start_at, status, cancel_reason, notes) " +
                    "VALUES(@doctor_id, @patient_id, @start_at, @status, @cancel_reason, @notes); " +
                    "SELECT last_insert_rowid();";
                AddAppointmentParameters(command, appointment);
                appointment.Id = Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture);
            }

            return appointment;
        }

        public void CancelAppointment(int appointmentId, string reason)
        {
            Appointment appointment = GetAppointment(appointmentId);
            if (appointment == null || appointment.Status != AppointmentStatus.Reserved)
            {
                return;
            }

            if (appointment.PatientId.HasValue
                && appointment.StartAt > DateTime.Now
                && appointment.StartAt.Subtract(DateTime.Now).TotalHours < 24)
            {
                AddWarning(appointment.PatientId.Value);
            }

            using (DbConnection connection = OpenConnection())
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "UPDATE appointments SET status = @status, cancel_reason = @reason WHERE id = @id";
                AddParameter(command, "@status", AppointmentStatus.Cancelled.ToString());
                AddParameter(command, "@reason", string.IsNullOrWhiteSpace(reason)
                    ? "Anulowano przez pracownika"
                    : reason.Trim());
                AddParameter(command, "@id", appointmentId);
                command.ExecuteNonQuery();
            }
        }

        public void SwapAppointmentPatient(int appointmentId, int newPatientId)
        {
            Appointment appointment = GetAppointment(appointmentId);
            if (appointment == null || appointment.Status != AppointmentStatus.Reserved)
            {
                throw new InvalidOperationException("Nie wybrano aktywnej wizyty do zamiany.");
            }

            ValidateReservation(appointment.DoctorId, newPatientId, appointment.StartAt, appointment.Id);

            using (DbConnection connection = OpenConnection())
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE appointments SET patient_id = @patient_id WHERE id = @id";
                AddParameter(command, "@patient_id", newPatientId);
                AddParameter(command, "@id", appointmentId);
                command.ExecuteNonQuery();
            }
        }

        public Employee FindEmployeeByLogin(string login)
        {
            using (DbConnection connection = OpenConnection())
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT id, login, display_name, role, password_hash, password_salt, created_at, is_active " +
                    "FROM employees WHERE lower(login) = @login";
                AddParameter(command, "@login", (login ?? string.Empty).Trim().ToLowerInvariant());
                using (DbDataReader reader = command.ExecuteReader())
                {
                    return reader.Read() ? ReadEmployee(reader) : null;
                }
            }
        }

        public Employee AddEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException("employee");
            }

            using (DbConnection connection = OpenConnection())
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "INSERT INTO employees(login, display_name, role, password_hash, password_salt, created_at, is_active) " +
                    "VALUES(@login, @display_name, @role, @password_hash, @password_salt, @created_at, @is_active); " +
                    "SELECT last_insert_rowid();";
                AddParameter(command, "@login", employee.Login);
                AddParameter(command, "@display_name", employee.DisplayName);
                AddParameter(command, "@role", employee.Role);
                AddParameter(command, "@password_hash", employee.PasswordHash);
                AddParameter(command, "@password_salt", employee.PasswordSalt);
                AddParameter(command, "@created_at", ToDbDate(employee.CreatedAt));
                AddParameter(command, "@is_active", employee.IsActive ? 1 : 0);
                employee.Id = Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture);
            }

            return employee;
        }

        private void ValidateReservation(int doctorId, int patientId, DateTime startAt, int ignoredAppointmentId)
        {
            Doctor doctor = GetDoctor(doctorId);
            Patient patient = GetPatient(patientId);

            if (doctor == null)
            {
                throw new InvalidOperationException("Nie znaleziono lekarza.");
            }

            if (patient == null)
            {
                throw new InvalidOperationException("Nie znaleziono pacjenta.");
            }

            if (patient.IsBlocked)
            {
                throw new InvalidOperationException(
                    "Pacjent ma aktywna blokade wizyt do "
                    + patient.BlockedUntil.Value.ToString("dd.MM.yyyy") + ".");
            }

            if (!IsDoctorWorking(doctorId, startAt))
            {
                throw new InvalidOperationException("Lekarz nie pracuje w wybranym terminie.");
            }

            if (IsSlotTaken(doctorId, startAt, ignoredAppointmentId))
            {
                throw new InvalidOperationException("Wybrany termin jest juz zajety.");
            }

            if (HasPatientConflict(patientId, doctor.Specialization, startAt, ignoredAppointmentId))
            {
                throw new InvalidOperationException(
                    "Pacjent ma juz wizyte w tym terminie lub u tej specjalizacji tego dnia.");
            }
        }

        private bool IsDoctorWorking(int doctorId, DateTime startAt)
        {
            return LoadSchedules(doctorId).Any(s => s.DayOfWeek == startAt.DayOfWeek
                && startAt.TimeOfDay >= s.StartTime
                && startAt.TimeOfDay < s.EndTime);
        }

        private bool IsSlotTaken(int doctorId, DateTime startAt, int ignoredAppointmentId)
        {
            return LoadAppointments(
                "doctor_id = @doctor_id AND start_at = @start_at AND status = @status AND id <> @ignored_id",
                command =>
                {
                    AddParameter(command, "@doctor_id", doctorId);
                    AddParameter(command, "@start_at", ToDbDate(startAt));
                    AddParameter(command, "@status", AppointmentStatus.Reserved.ToString());
                    AddParameter(command, "@ignored_id", ignoredAppointmentId);
                })
                .Any();
        }

        private bool HasPatientConflict(
            int patientId,
            string specialization,
            DateTime startAt,
            int ignoredAppointmentId)
        {
            var patientAppointments = LoadAppointments(
                "patient_id = @patient_id AND substr(start_at, 1, 10) = @day AND status = @status AND id <> @ignored_id",
                command =>
                {
                    AddParameter(command, "@patient_id", patientId);
                    AddParameter(command, "@day", startAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                    AddParameter(command, "@status", AppointmentStatus.Reserved.ToString());
                    AddParameter(command, "@ignored_id", ignoredAppointmentId);
                });

            foreach (Appointment appointment in patientAppointments)
            {
                if (appointment.StartAt == startAt)
                {
                    return true;
                }

                Doctor doctor = GetDoctor(appointment.DoctorId);
                if (doctor != null
                    && string.Equals(doctor.Specialization, specialization, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<DateTime> GenerateSlots(int doctorId, DateTime date)
        {
            foreach (ScheduleEntry schedule in LoadSchedules(doctorId).Where(s => s.DayOfWeek == date.DayOfWeek))
            {
                DateTime current = date.Add(schedule.StartTime);
                DateTime end = date.Add(schedule.EndTime);
                while (current < end)
                {
                    yield return current;
                    current = current.AddMinutes(15);
                }
            }
        }

        private Appointment GetAppointment(int appointmentId)
        {
            return LoadAppointments(
                "id = @id",
                command => AddParameter(command, "@id", appointmentId))
                .FirstOrDefault();
        }

        private List<Appointment> LoadAppointments(string whereClause, Action<DbCommand> configure)
        {
            var appointments = new List<Appointment>();
            using (DbConnection connection = OpenConnection())
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT id, doctor_id, patient_id, start_at, status, cancel_reason, notes FROM appointments WHERE "
                    + whereClause;
                configure(command);
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        appointments.Add(ReadAppointment(reader));
                    }
                }
            }

            return appointments;
        }

        private List<ScheduleEntry> LoadSchedules(int doctorId)
        {
            var schedules = new List<ScheduleEntry>();
            using (DbConnection connection = OpenConnection())
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT id, doctor_id, day_of_week, start_time, end_time FROM schedules WHERE doctor_id = @doctor_id";
                AddParameter(command, "@doctor_id", doctorId);
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        schedules.Add(new ScheduleEntry
                        {
                            Id = Convert.ToInt32(reader["id"], CultureInfo.InvariantCulture),
                            DoctorId = Convert.ToInt32(reader["doctor_id"], CultureInfo.InvariantCulture),
                            DayOfWeek = (DayOfWeek)Convert.ToInt32(reader["day_of_week"], CultureInfo.InvariantCulture),
                            StartTime = TimeSpan.Parse(Convert.ToString(reader["start_time"], CultureInfo.InvariantCulture), CultureInfo.InvariantCulture),
                            EndTime = TimeSpan.Parse(Convert.ToString(reader["end_time"], CultureInfo.InvariantCulture), CultureInfo.InvariantCulture)
                        });
                    }
                }
            }

            return schedules;
        }

        private void AddWarning(int patientId)
        {
            Patient patient = GetPatient(patientId);
            if (patient == null)
            {
                return;
            }

            patient.WarningCount++;
            if (patient.WarningCount >= 3)
            {
                patient.BlockedUntil = DateTime.Today.AddDays(30);
            }

            using (DbConnection connection = OpenConnection())
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "UPDATE patients SET warning_count = @warning_count, blocked_until = @blocked_until WHERE id = @id";
                AddParameter(command, "@warning_count", patient.WarningCount);
                AddParameter(command, "@blocked_until", patient.BlockedUntil.HasValue
                    ? ToDbDate(patient.BlockedUntil.Value)
                    : null);
                AddParameter(command, "@id", patient.Id);
                command.ExecuteNonQuery();
            }
        }

        private void EnsureSchema()
        {
            ExecuteNonQuery(
                "CREATE TABLE IF NOT EXISTS patients (" +
                "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "first_name TEXT NOT NULL, last_name TEXT NOT NULL, pesel TEXT UNIQUE, " +
                "phone TEXT, email TEXT, address TEXT, warning_count INTEGER NOT NULL DEFAULT 0, blocked_until TEXT);" );
            ExecuteNonQuery(
                "CREATE TABLE IF NOT EXISTS doctors (" +
                "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "first_name TEXT NOT NULL, last_name TEXT NOT NULL, specialization TEXT NOT NULL);" );
            ExecuteNonQuery(
                "CREATE TABLE IF NOT EXISTS appointments (" +
                "id INTEGER PRIMARY KEY AUTOINCREMENT, doctor_id INTEGER NOT NULL, patient_id INTEGER, " +
                "start_at TEXT NOT NULL, status TEXT NOT NULL, cancel_reason TEXT, notes TEXT, " +
                "FOREIGN KEY(doctor_id) REFERENCES doctors(id), FOREIGN KEY(patient_id) REFERENCES patients(id));" );
            ExecuteNonQuery(
                "CREATE TABLE IF NOT EXISTS schedules (" +
                "id INTEGER PRIMARY KEY AUTOINCREMENT, doctor_id INTEGER NOT NULL, day_of_week INTEGER NOT NULL, " +
                "start_time TEXT NOT NULL, end_time TEXT NOT NULL, FOREIGN KEY(doctor_id) REFERENCES doctors(id));" );
            ExecuteNonQuery(
                "CREATE TABLE IF NOT EXISTS employees (" +
                "id INTEGER PRIMARY KEY AUTOINCREMENT, login TEXT NOT NULL UNIQUE, display_name TEXT NOT NULL, " +
                "role TEXT NOT NULL, password_hash TEXT NOT NULL, password_salt TEXT NOT NULL, " +
                "created_at TEXT NOT NULL, is_active INTEGER NOT NULL DEFAULT 1);" );
        }

        private void SeedIfEmpty()
        {
            if (Convert.ToInt32(ExecuteScalar("SELECT COUNT(*) FROM doctors"), CultureInfo.InvariantCulture) > 0)
            {
                return;
            }

            ClinicSeedData seed = SampleData.Create();
            foreach (Doctor doctor in seed.Doctors)
            {
                ExecuteNonQuery(
                    "INSERT INTO doctors(id, first_name, last_name, specialization) VALUES(@id, @first_name, @last_name, @specialization)",
                    command =>
                    {
                        AddParameter(command, "@id", doctor.Id);
                        AddParameter(command, "@first_name", doctor.FirstName);
                        AddParameter(command, "@last_name", doctor.LastName);
                        AddParameter(command, "@specialization", doctor.Specialization);
                    });
            }

            foreach (ScheduleEntry schedule in seed.Schedules)
            {
                ExecuteNonQuery(
                    "INSERT INTO schedules(id, doctor_id, day_of_week, start_time, end_time) VALUES(@id, @doctor_id, @day_of_week, @start_time, @end_time)",
                    command =>
                    {
                        AddParameter(command, "@id", schedule.Id);
                        AddParameter(command, "@doctor_id", schedule.DoctorId);
                        AddParameter(command, "@day_of_week", (int)schedule.DayOfWeek);
                        AddParameter(command, "@start_time", schedule.StartTime.ToString());
                        AddParameter(command, "@end_time", schedule.EndTime.ToString());
                    });
            }

            foreach (Patient patient in seed.Patients)
            {
                ExecuteNonQuery(
                    "INSERT INTO patients(id, first_name, last_name, pesel, phone, email, address, warning_count, blocked_until) " +
                    "VALUES(@id, @first_name, @last_name, @pesel, @phone, @email, @address, @warning_count, @blocked_until)",
                    command =>
                    {
                        AddParameter(command, "@id", patient.Id);
                        AddPatientParameters(command, patient);
                    });
            }

            foreach (Appointment appointment in seed.Appointments)
            {
                ExecuteNonQuery(
                    "INSERT INTO appointments(id, doctor_id, patient_id, start_at, status, cancel_reason, notes) " +
                    "VALUES(@id, @doctor_id, @patient_id, @start_at, @status, @cancel_reason, @notes)",
                    command =>
                    {
                        AddParameter(command, "@id", appointment.Id);
                        AddAppointmentParameters(command, appointment);
                    });
            }

            foreach (Employee employee in seed.Employees)
            {
                ExecuteNonQuery(
                    "INSERT INTO employees(id, login, display_name, role, password_hash, password_salt, created_at, is_active) " +
                    "VALUES(@id, @login, @display_name, @role, @password_hash, @password_salt, @created_at, @is_active)",
                    command =>
                    {
                        AddParameter(command, "@id", employee.Id);
                        AddParameter(command, "@login", employee.Login);
                        AddParameter(command, "@display_name", employee.DisplayName);
                        AddParameter(command, "@role", employee.Role);
                        AddParameter(command, "@password_hash", employee.PasswordHash);
                        AddParameter(command, "@password_salt", employee.PasswordSalt);
                        AddParameter(command, "@created_at", ToDbDate(employee.CreatedAt));
                        AddParameter(command, "@is_active", employee.IsActive ? 1 : 0);
                    });
            }
        }

        private DbConnection OpenConnection()
        {
            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;
            connection.Open();
            return connection;
        }

        private static DbProviderFactory LoadFactory()
        {
            try
            {
                return DbProviderFactories.GetFactory("System.Data.SQLite");
            }
            catch
            {
                Type factoryType = Type.GetType("System.Data.SQLite.SQLiteFactory, System.Data.SQLite");
                if (factoryType != null)
                {
                    var instanceField = factoryType.GetField("Instance");
                    if (instanceField != null)
                    {
                        return (DbProviderFactory)instanceField.GetValue(null);
                    }
                }

                throw new InvalidOperationException("Brak providera System.Data.SQLite.");
            }
        }

        private object ExecuteScalar(string sql)
        {
            using (DbConnection connection = OpenConnection())
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                return command.ExecuteScalar();
            }
        }

        private void ExecuteNonQuery(string sql)
        {
            ExecuteNonQuery(sql, null);
        }

        private void ExecuteNonQuery(string sql, Action<DbCommand> configure)
        {
            using (DbConnection connection = OpenConnection())
            using (DbCommand command = connection.CreateCommand())
            {
                command.CommandText = sql;
                if (configure != null)
                {
                    configure(command);
                }

                command.ExecuteNonQuery();
            }
        }

        private static void AddPatientParameters(DbCommand command, Patient patient)
        {
            AddParameter(command, "@first_name", patient.FirstName);
            AddParameter(command, "@last_name", patient.LastName);
            AddParameter(command, "@pesel", patient.Pesel);
            AddParameter(command, "@phone", patient.Phone);
            AddParameter(command, "@email", patient.Email);
            AddParameter(command, "@address", patient.Address);
            AddParameter(command, "@warning_count", patient.WarningCount);
            AddParameter(command, "@blocked_until", patient.BlockedUntil.HasValue ? ToDbDate(patient.BlockedUntil.Value) : null);
        }

        private static void AddAppointmentParameters(DbCommand command, Appointment appointment)
        {
            AddParameter(command, "@doctor_id", appointment.DoctorId);
            AddParameter(command, "@patient_id", appointment.PatientId.HasValue ? (object)appointment.PatientId.Value : null);
            AddParameter(command, "@start_at", ToDbDate(appointment.StartAt));
            AddParameter(command, "@status", appointment.Status.ToString());
            AddParameter(command, "@cancel_reason", appointment.CancelReason);
            AddParameter(command, "@notes", appointment.Notes);
        }

        private static void AddParameter(DbCommand command, string name, object value)
        {
            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        private static Doctor ReadDoctor(IDataRecord reader)
        {
            return new Doctor
            {
                Id = Convert.ToInt32(reader["id"], CultureInfo.InvariantCulture),
                FirstName = Convert.ToString(reader["first_name"], CultureInfo.InvariantCulture),
                LastName = Convert.ToString(reader["last_name"], CultureInfo.InvariantCulture),
                Specialization = Convert.ToString(reader["specialization"], CultureInfo.InvariantCulture)
            };
        }

        private static Patient ReadPatient(IDataRecord reader)
        {
            return new Patient
            {
                Id = Convert.ToInt32(reader["id"], CultureInfo.InvariantCulture),
                FirstName = Convert.ToString(reader["first_name"], CultureInfo.InvariantCulture),
                LastName = Convert.ToString(reader["last_name"], CultureInfo.InvariantCulture),
                Pesel = Convert.ToString(reader["pesel"], CultureInfo.InvariantCulture),
                Phone = Convert.ToString(reader["phone"], CultureInfo.InvariantCulture),
                Email = Convert.ToString(reader["email"], CultureInfo.InvariantCulture),
                Address = Convert.ToString(reader["address"], CultureInfo.InvariantCulture),
                WarningCount = Convert.ToInt32(reader["warning_count"], CultureInfo.InvariantCulture),
                BlockedUntil = ReadNullableDate(reader["blocked_until"])
            };
        }

        private static Appointment ReadAppointment(IDataRecord reader)
        {
            return new Appointment
            {
                Id = Convert.ToInt32(reader["id"], CultureInfo.InvariantCulture),
                DoctorId = Convert.ToInt32(reader["doctor_id"], CultureInfo.InvariantCulture),
                PatientId = reader["patient_id"] == DBNull.Value
                    ? (int?)null
                    : Convert.ToInt32(reader["patient_id"], CultureInfo.InvariantCulture),
                StartAt = DateTime.Parse(Convert.ToString(reader["start_at"], CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                Status = (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), Convert.ToString(reader["status"], CultureInfo.InvariantCulture)),
                CancelReason = Convert.ToString(reader["cancel_reason"], CultureInfo.InvariantCulture),
                Notes = Convert.ToString(reader["notes"], CultureInfo.InvariantCulture)
            };
        }

        private static Employee ReadEmployee(IDataRecord reader)
        {
            return new Employee
            {
                Id = Convert.ToInt32(reader["id"], CultureInfo.InvariantCulture),
                Login = Convert.ToString(reader["login"], CultureInfo.InvariantCulture),
                DisplayName = Convert.ToString(reader["display_name"], CultureInfo.InvariantCulture),
                Role = Convert.ToString(reader["role"], CultureInfo.InvariantCulture),
                PasswordHash = Convert.ToString(reader["password_hash"], CultureInfo.InvariantCulture),
                PasswordSalt = Convert.ToString(reader["password_salt"], CultureInfo.InvariantCulture),
                CreatedAt = DateTime.Parse(Convert.ToString(reader["created_at"], CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                IsActive = Convert.ToInt32(reader["is_active"], CultureInfo.InvariantCulture) == 1
            };
        }

        private static DateTime? ReadNullableDate(object value)
        {
            if (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(Convert.ToString(value, CultureInfo.InvariantCulture)))
            {
                return null;
            }

            return DateTime.Parse(Convert.ToString(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        private static string ToDbDate(DateTime value)
        {
            return value.ToString("o", CultureInfo.InvariantCulture);
        }
    }
}
