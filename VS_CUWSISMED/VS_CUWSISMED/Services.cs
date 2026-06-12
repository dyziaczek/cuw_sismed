using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace VS_CUWSISMED
{
    public interface IClinicDataStore
    {
        IReadOnlyList<Doctor> GetDoctors();
        IReadOnlyList<MedicalService> GetServices();
        IReadOnlyList<AvailableSlot> GetAvailableSlots(int doctorId, DateTime date);
        IReadOnlyList<Appointment> GetAppointmentsForDoctor(int doctorId, DateTime date);
        IReadOnlyList<Appointment> GetAllAppointmentsForDoctor(int doctorId, DateTime date);
        IReadOnlyList<Appointment> GetAppointmentsForPatient(int patientId);
        IReadOnlyList<Appointment> GetAllAppointmentsForPatient(int patientId);
        int GetPatientCount();
        IReadOnlyList<Patient> SearchPatients(PatientSearchCriteria criteria);
        Patient FindPatient(string query);
        Patient AddPatient(Patient patient);
        Patient GetPatient(int patientId);
        IReadOnlyList<PatientNote> GetPatientNotes(int patientId);
        PatientNote AddPatientNote(int patientId, string text, string createdByEmployee);
        void DeletePatientNote(int noteId);
        IReadOnlyList<PatientWarning> GetPatientWarnings(int patientId);
        Doctor GetDoctor(int doctorId);
        Appointment ReserveAppointment(int doctorId, int patientId, DateTime startAt);
        void CancelAppointment(int appointmentId, string reason);
        void SwapAppointmentPatient(int appointmentId, int newPatientId);
        void SwapAppointmentPatient(int appointmentId, int newPatientId, string changedByEmployee);
        Employee FindEmployeeByLogin(string login);
        Employee GetEmployee(int employeeId);
        IReadOnlyList<Employee> SearchEmployees(string query);
        Employee AddEmployee(Employee employee);
        void DeactivateEmployee(int employeeId);
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
            string mode = (ConfigurationManager.AppSettings["SismedStorageMode"] ?? "SQLite").Trim();

            if (mode.Equals("Memory", StringComparison.OrdinalIgnoreCase))
            {
                AppServices.StorageInfo = "Dane pamieciowe";
                return new InMemoryClinicDataStore(SampleData.Create());
            }

            string dbPath = ResolveDatabasePath(ConfigurationManager.AppSettings["SismedDatabasePath"]);
            AppServices.StorageInfo = "SQLite: " + dbPath;
            return new PersistentSqliteClinicDataStore(dbPath);
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
            string firstName;
            string lastName;
            SplitDisplayName(displayName, out firstName, out lastName);

            return RegisterEmployee(
                login,
                firstName,
                lastName,
                string.Empty,
                null,
                role,
                password,
                repeatedPassword,
                false,
                string.Empty);
        }

        public RegistrationResult RegisterEmployee(
            string login,
            string firstName,
            string lastName,
            string pesel,
            DateTime? birthDate,
            string role,
            string password,
            string repeatedPassword,
            bool isDoctor,
            string specialization)
        {
            login = (login ?? string.Empty).Trim();
            firstName = (firstName ?? string.Empty).Trim();
            lastName = (lastName ?? string.Empty).Trim();
            pesel = (pesel ?? string.Empty).Trim();
            role = EmployeeRoles.Normalize(role);
            specialization = (specialization ?? string.Empty).Trim();

            if (login.Length < 3)
            {
                return RegistrationResult.Fail("Login musi miec co najmniej 3 znaki.");
            }

            if (firstName.Length < 2 || lastName.Length < 2)
            {
                return RegistrationResult.Fail("Podaj imie i nazwisko pracownika.");
            }

            if (pesel.Length > 0 && (pesel.Length != 11 || !pesel.All(char.IsDigit)))
            {
                return RegistrationResult.Fail("PESEL musi skladac sie z 11 cyfr.");
            }

            if (isDoctor && specialization.Length == 0)
            {
                return RegistrationResult.Fail("Podaj specjalizacje lekarza.");
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
                FirstName = firstName,
                LastName = lastName,
                Pesel = pesel,
                BirthDate = birthDate,
                DisplayName = string.Format("{0} {1}", firstName, lastName).Trim(),
                Role = role,
                PasswordSalt = salt,
                PasswordHash = hash,
                CreatedAt = DateTime.Now,
                IsActive = true,
                IsDoctor = isDoctor,
                Specialization = specialization
            });

            return RegistrationResult.Ok(employee);
        }

        public void EnsureAdministrator(Employee employee)
        {
            if (employee == null || !employee.IsAdministrator)
            {
                throw new UnauthorizedAccessException("Brak uprawnien administratora.");
            }
        }

        public void DeactivateEmployee(Employee currentEmployee, int employeeId)
        {
            EnsureAdministrator(currentEmployee);
            dataStore.DeactivateEmployee(employeeId);
        }

        private static void SplitDisplayName(string displayName, out string firstName, out string lastName)
        {
            string[] parts = (displayName ?? string.Empty)
                .Trim()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            firstName = parts.Length > 0 ? parts[0] : string.Empty;
            lastName = parts.Length > 1
                ? string.Join(" ", parts.Skip(1).ToArray())
                : string.Empty;
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
        private readonly List<MedicalService> services;
        private readonly List<ScheduleEntry> schedules;
        private readonly List<Appointment> appointments;
        private readonly List<Employee> employees;
        private readonly List<PatientNote> patientNotes;
        private readonly List<PatientWarning> patientWarnings;
        private int nextPatientId;
        private int nextAppointmentId;
        private int nextEmployeeId;
        private int nextNoteId;
        private int nextWarningId;

        public InMemoryClinicDataStore(ClinicSeedData seed)
        {
            patients = seed.Patients;
            doctors = seed.Doctors;
            services = seed.Services;
            schedules = seed.Schedules;
            appointments = seed.Appointments;
            employees = seed.Employees;
            patientNotes = seed.PatientNotes;
            patientWarnings = seed.PatientWarnings;
            nextPatientId = NextId(patients.Select(p => p.Id));
            nextAppointmentId = NextId(appointments.Select(a => a.Id));
            nextEmployeeId = NextId(employees.Select(e => e.Id));
            nextNoteId = NextId(patientNotes.Select(n => n.Id));
            nextWarningId = NextId(patientWarnings.Select(w => w.Id));
        }

        public IReadOnlyList<Doctor> GetDoctors()
        {
            return doctors.OrderBy(d => d.LastName).ThenBy(d => d.FirstName).ToList();
        }

        public IReadOnlyList<MedicalService> GetServices()
        {
            return services.Where(s => s.IsActive).OrderBy(s => s.Name).ToList();
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

        public IReadOnlyList<Appointment> GetAllAppointmentsForDoctor(int doctorId, DateTime date)
        {
            return appointments
                .Where(a => a.DoctorId == doctorId && a.StartAt.Date == date.Date)
                .OrderBy(a => a.StartAt)
                .ToList();
        }

        public IReadOnlyList<Appointment> GetAppointmentsForPatient(int patientId)
        {
            return appointments
                .Where(a => a.PatientId == patientId && a.Status == AppointmentStatus.Reserved)
                .OrderBy(a => a.StartAt)
                .ToList();
        }

        public IReadOnlyList<Appointment> GetAllAppointmentsForPatient(int patientId)
        {
            return appointments
                .Where(a => a.PatientId == patientId)
                .OrderBy(a => a.StartAt)
                .ToList();
        }

        public int GetPatientCount()
        {
            return patients.Count;
        }

        public IReadOnlyList<Patient> SearchPatients(PatientSearchCriteria criteria)
        {
            if (criteria == null || criteria.IsEmpty)
            {
                return new List<Patient>();
            }

            IEnumerable<Patient> result = patients;
            string pesel = Normalize(criteria.Pesel);
            string firstName = Normalize(criteria.FirstName);
            string lastName = Normalize(criteria.LastName);
            string phone = Normalize(criteria.Phone);
            string email = Normalize(criteria.Email);

            if (!string.IsNullOrEmpty(pesel))
            {
                result = result.Where(p => Normalize(p.Pesel).Contains(pesel));
            }

            if (!string.IsNullOrEmpty(firstName))
            {
                result = result.Where(p => Normalize(p.FirstName).Contains(firstName));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                result = result.Where(p => Normalize(p.LastName).Contains(lastName));
            }

            if (criteria.BirthDate.HasValue)
            {
                result = result.Where(p => p.BirthDate.HasValue && p.BirthDate.Value.Date == criteria.BirthDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(phone))
            {
                result = result.Where(p => Normalize(p.Phone).Contains(phone));
            }

            if (!string.IsNullOrEmpty(email))
            {
                result = result.Where(p => Normalize(p.Email).Contains(email));
            }

            return result.OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ToList();
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

            patient.Id = patient.Id > 0 ? patient.Id : nextPatientId++;
            patients.Add(patient);

            if (!string.IsNullOrWhiteSpace(patient.Notes))
            {
                patientNotes.Add(new PatientNote
                {
                    Id = nextNoteId++,
                    PatientId = patient.Id,
                    CreatedAt = DateTime.Now,
                    CreatedByEmployee = "System",
                    Text = patient.Notes
                });
            }

            return patient;
        }

        public Patient GetPatient(int patientId)
        {
            return patients.FirstOrDefault(p => p.Id == patientId);
        }

        public IReadOnlyList<PatientNote> GetPatientNotes(int patientId)
        {
            return patientNotes
                .Where(n => n.PatientId == patientId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public PatientNote AddPatientNote(int patientId, string text, string createdByEmployee)
        {
            if (GetPatient(patientId) == null)
            {
                throw new InvalidOperationException("Nie znaleziono pacjenta.");
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new InvalidOperationException("Treść notatki nie może być pusta.");
            }

            var note = new PatientNote
            {
                Id = nextNoteId++,
                PatientId = patientId,
                CreatedAt = DateTime.Now,
                CreatedByEmployee = string.IsNullOrWhiteSpace(createdByEmployee)
                    ? "Pracownik"
                    : createdByEmployee.Trim(),
                Text = text.Trim()
            };
            patientNotes.Add(note);
            return note;
        }

        public void DeletePatientNote(int noteId)
        {
            PatientNote note = patientNotes.FirstOrDefault(n => n.Id == noteId);
            if (note != null)
            {
                patientNotes.Remove(note);
            }
        }

        public IReadOnlyList<PatientWarning> GetPatientWarnings(int patientId)
        {
            return patientWarnings
                .Where(w => w.PatientId == patientId)
                .OrderByDescending(w => w.CreatedAt)
                .ToList();
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
                && appointment.StartAt.Subtract(DateTime.Now).TotalHours < 12)
            {
                string cancelReason = string.IsNullOrWhiteSpace(reason)
                    ? "Brak powodu anulowania."
                    : reason.Trim();
                AddWarning(
                    appointment.PatientId.Value,
                    "Anulowanie wizyty mniej niż 12h przed terminem. Powód: " + cancelReason);
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancelReason = string.IsNullOrWhiteSpace(reason)
                ? "Anulowano przez pracownika"
                : reason.Trim();
        }

        public void SwapAppointmentPatient(int appointmentId, int newPatientId)
        {
            SwapAppointmentPatient(appointmentId, newPatientId, "Pracownik");
        }

        public void SwapAppointmentPatient(int appointmentId, int newPatientId, string changedByEmployee)
        {
            Appointment appointment = appointments.FirstOrDefault(a => a.Id == appointmentId);
            if (appointment == null || appointment.Status != AppointmentStatus.Reserved)
            {
                throw new InvalidOperationException("Nie wybrano aktywnej wizyty do zamiany.");
            }

            if (appointment.PatientId.HasValue && appointment.PatientId.Value == newPatientId)
            {
                throw new InvalidOperationException("Nie można zamienić wizyty na tego samego pacjenta.");
            }

            Patient previousPatient = appointment.PatientId.HasValue
                ? GetPatient(appointment.PatientId.Value)
                : null;
            Patient newPatient = GetPatient(newPatientId);

            ValidateReservation(appointment.DoctorId, newPatientId, appointment.StartAt, appointment.Id);
            appointment.PatientId = newPatientId;
            AppendSwapNote(appointment, previousPatient, newPatient, changedByEmployee);
        }

        public Employee FindEmployeeByLogin(string login)
        {
            string normalized = Normalize(login);
            return employees.FirstOrDefault(e => Normalize(e.Login) == normalized);
        }

        public Employee GetEmployee(int employeeId)
        {
            return employees.FirstOrDefault(e => e.Id == employeeId);
        }

        public Appointment GetAppointment(int appointmentId)
        {
            return appointments.FirstOrDefault(a => a.Id == appointmentId);
        }

        public IReadOnlyList<Employee> SearchEmployees(string query)
        {
            string normalized = Normalize(query);
            IEnumerable<Employee> result = employees;

            if (!string.IsNullOrEmpty(normalized))
            {
                result = result.Where(e =>
                    Normalize(e.FirstName).Contains(normalized)
                    || Normalize(e.LastName).Contains(normalized)
                    || Normalize(e.FullName).Contains(normalized)
                    || Normalize(e.Pesel).Contains(normalized)
                    || Normalize(e.Login).Contains(normalized)
                    || Normalize(FormatDate(e.BirthDate)).Contains(normalized));
            }

            return result.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToList();
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

            if (!string.IsNullOrWhiteSpace(employee.Pesel)
                && employees.Any(e => string.Equals(e.Pesel, employee.Pesel, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Pracownik o podanym numerze PESEL juz istnieje.");
            }

            employee.Id = employee.Id > 0 ? employee.Id : nextEmployeeId++;
            employee.DisplayName = employee.FullName;
            employee.Role = EmployeeRoles.Normalize(employee.Role);
            employees.Add(employee);
            return employee;
        }

        public void DeactivateEmployee(int employeeId)
        {
            Employee employee = GetEmployee(employeeId);
            if (employee == null)
            {
                throw new InvalidOperationException("Nie znaleziono pracownika.");
            }

            if (!employee.IsActive)
            {
                return;
            }

            if (employee.IsAdministrator && employees.Count(e => e.IsActive && e.IsAdministrator) <= 1)
            {
                throw new InvalidOperationException("Nie mozna dezaktywowac ostatniego aktywnego administratora.");
            }

            employee.IsActive = false;
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

        private void AddWarning(int patientId, string reason)
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

            patientWarnings.Add(new PatientWarning
            {
                Id = nextWarningId++,
                PatientId = patientId,
                CreatedAt = DateTime.Now,
                Reason = reason
            });
        }

        private static void AppendSwapNote(
            Appointment appointment,
            Patient previousPatient,
            Patient newPatient,
            string changedByEmployee)
        {
            string note = string.Format(
                "[{0:dd.MM.yyyy HH:mm}] Zamiana pacjenta: {1} -> {2}. Pracownik: {3}.",
                DateTime.Now,
                previousPatient == null ? "-" : previousPatient.DisplayName,
                newPatient == null ? "-" : newPatient.DisplayName,
                string.IsNullOrWhiteSpace(changedByEmployee) ? "Pracownik" : changedByEmployee.Trim());

            appointment.Notes = string.IsNullOrWhiteSpace(appointment.Notes)
                ? note
                : appointment.Notes + Environment.NewLine + note;
        }

        private static int NextId(IEnumerable<int> ids)
        {
            return ids.Any() ? ids.Max() + 1 : 1;
        }

        private static string Normalize(string value)
        {
            return (value ?? string.Empty).Trim().ToLowerInvariant();
        }

        private static string FormatDate(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : string.Empty;
        }
    }

    internal sealed class PersistentSqliteClinicDataStore : IClinicDataStore
    {
        private readonly NativeSqliteDatabase database;
        private readonly InMemoryClinicDataStore memory;

        public PersistentSqliteClinicDataStore(string databasePath)
        {
            database = new NativeSqliteDatabase(databasePath);
            EnsureSchema();
            SeedIfEmpty();
            memory = new InMemoryClinicDataStore(LoadData());
        }

        public IReadOnlyList<Doctor> GetDoctors()
        {
            return memory.GetDoctors();
        }

        public IReadOnlyList<MedicalService> GetServices()
        {
            return memory.GetServices();
        }

        public IReadOnlyList<AvailableSlot> GetAvailableSlots(int doctorId, DateTime date)
        {
            return memory.GetAvailableSlots(doctorId, date);
        }

        public IReadOnlyList<Appointment> GetAppointmentsForDoctor(int doctorId, DateTime date)
        {
            return memory.GetAppointmentsForDoctor(doctorId, date);
        }

        public IReadOnlyList<Appointment> GetAllAppointmentsForDoctor(int doctorId, DateTime date)
        {
            return memory.GetAllAppointmentsForDoctor(doctorId, date);
        }

        public IReadOnlyList<Appointment> GetAppointmentsForPatient(int patientId)
        {
            return memory.GetAppointmentsForPatient(patientId);
        }

        public IReadOnlyList<Appointment> GetAllAppointmentsForPatient(int patientId)
        {
            return memory.GetAllAppointmentsForPatient(patientId);
        }

        public int GetPatientCount()
        {
            return memory.GetPatientCount();
        }

        public IReadOnlyList<Patient> SearchPatients(PatientSearchCriteria criteria)
        {
            return memory.SearchPatients(criteria);
        }

        public Patient FindPatient(string query)
        {
            return memory.FindPatient(query);
        }

        public Patient AddPatient(Patient patient)
        {
            Patient saved = memory.AddPatient(patient);
            InsertPatient(saved);

            if (!string.IsNullOrWhiteSpace(saved.Notes))
            {
                PatientNote note = memory.GetPatientNotes(saved.Id).FirstOrDefault();
                if (note != null)
                {
                    InsertPatientNote(note);
                }
            }

            return saved;
        }

        public Patient GetPatient(int patientId)
        {
            return memory.GetPatient(patientId);
        }

        public IReadOnlyList<PatientNote> GetPatientNotes(int patientId)
        {
            return memory.GetPatientNotes(patientId);
        }

        public PatientNote AddPatientNote(int patientId, string text, string createdByEmployee)
        {
            PatientNote note = memory.AddPatientNote(patientId, text, createdByEmployee);
            InsertPatientNote(note);
            return note;
        }

        public void DeletePatientNote(int noteId)
        {
            memory.DeletePatientNote(noteId);
            database.ExecuteNonQuery(
                "DELETE FROM patient_notes WHERE id = ?",
                SqlValue.Int(noteId));
        }

        public IReadOnlyList<PatientWarning> GetPatientWarnings(int patientId)
        {
            return memory.GetPatientWarnings(patientId);
        }

        public Doctor GetDoctor(int doctorId)
        {
            return memory.GetDoctor(doctorId);
        }

        public Appointment ReserveAppointment(int doctorId, int patientId, DateTime startAt)
        {
            Appointment appointment = memory.ReserveAppointment(doctorId, patientId, startAt);
            InsertAppointment(appointment);
            return appointment;
        }

        public void CancelAppointment(int appointmentId, string reason)
        {
            Appointment appointment = GetStoredAppointment(appointmentId);
            int patientId = appointment != null && appointment.PatientId.HasValue ? appointment.PatientId.Value : 0;
            int warningCountBefore = patientId > 0 ? memory.GetPatientWarnings(patientId).Count : 0;

            memory.CancelAppointment(appointmentId, reason);

            Appointment updated = GetStoredAppointment(appointmentId);
            if (updated != null)
            {
                UpdateAppointment(updated);
            }

            if (patientId > 0)
            {
                Patient patient = memory.GetPatient(patientId);
                UpdatePatient(patient);

                PatientWarning warning = memory.GetPatientWarnings(patientId)
                    .OrderByDescending(w => w.CreatedAt)
                    .FirstOrDefault();
                if (warning != null && memory.GetPatientWarnings(patientId).Count > warningCountBefore)
                {
                    InsertPatientWarning(warning);
                }
            }
        }

        public void SwapAppointmentPatient(int appointmentId, int newPatientId)
        {
            SwapAppointmentPatient(appointmentId, newPatientId, "Pracownik");
        }

        public void SwapAppointmentPatient(int appointmentId, int newPatientId, string changedByEmployee)
        {
            memory.SwapAppointmentPatient(appointmentId, newPatientId, changedByEmployee);
            Appointment appointment = GetStoredAppointment(appointmentId);
            if (appointment != null)
            {
                UpdateAppointment(appointment);
            }
        }

        public Employee FindEmployeeByLogin(string login)
        {
            return memory.FindEmployeeByLogin(login);
        }

        public Employee GetEmployee(int employeeId)
        {
            return memory.GetEmployee(employeeId);
        }

        public IReadOnlyList<Employee> SearchEmployees(string query)
        {
            return memory.SearchEmployees(query);
        }

        public Employee AddEmployee(Employee employee)
        {
            Employee saved = memory.AddEmployee(employee);
            InsertEmployee(saved);
            return saved;
        }

        public void DeactivateEmployee(int employeeId)
        {
            memory.DeactivateEmployee(employeeId);
            Employee employee = memory.GetEmployee(employeeId);
            database.ExecuteNonQuery(
                "UPDATE employees SET is_active = ? WHERE id = ?",
                SqlValue.Bool(employee.IsActive),
                SqlValue.Int(employee.Id));
        }

        private Appointment GetStoredAppointment(int appointmentId)
        {
            return memory.GetAppointment(appointmentId);
        }

        private void EnsureSchema()
        {
            database.ExecuteNonQuery(
                "CREATE TABLE IF NOT EXISTS patients (" +
                "id INTEGER PRIMARY KEY, first_name TEXT NOT NULL, last_name TEXT NOT NULL, pesel TEXT UNIQUE, " +
                "birth_date TEXT, phone TEXT, email TEXT, address TEXT, notes TEXT, " +
                "warning_count INTEGER NOT NULL DEFAULT 0, blocked_until TEXT)");
            database.ExecuteNonQuery(
                "CREATE TABLE IF NOT EXISTS doctors (" +
                "id INTEGER PRIMARY KEY, first_name TEXT NOT NULL, last_name TEXT NOT NULL, specialization TEXT NOT NULL)");
            database.ExecuteNonQuery(
                "CREATE TABLE IF NOT EXISTS services (" +
                "id INTEGER PRIMARY KEY, name TEXT NOT NULL, specialization TEXT NOT NULL, is_active INTEGER NOT NULL DEFAULT 1)");
            database.ExecuteNonQuery(
                "CREATE TABLE IF NOT EXISTS schedules (" +
                "id INTEGER PRIMARY KEY, doctor_id INTEGER NOT NULL, day_of_week INTEGER NOT NULL, " +
                "start_time TEXT NOT NULL, end_time TEXT NOT NULL)");
            database.ExecuteNonQuery(
                "CREATE TABLE IF NOT EXISTS appointments (" +
                "id INTEGER PRIMARY KEY, doctor_id INTEGER NOT NULL, patient_id INTEGER, start_at TEXT NOT NULL, " +
                "status TEXT NOT NULL, cancel_reason TEXT, notes TEXT)");
            database.ExecuteNonQuery(
                "CREATE TABLE IF NOT EXISTS employees (" +
                "id INTEGER PRIMARY KEY, login TEXT NOT NULL UNIQUE, first_name TEXT, last_name TEXT, pesel TEXT, " +
                "birth_date TEXT, display_name TEXT NOT NULL, role TEXT NOT NULL, password_hash TEXT NOT NULL, " +
                "password_salt TEXT NOT NULL, created_at TEXT NOT NULL, is_active INTEGER NOT NULL DEFAULT 1, " +
                "is_doctor INTEGER NOT NULL DEFAULT 0, specialization TEXT)");
            database.ExecuteNonQuery(
                "CREATE TABLE IF NOT EXISTS patient_notes (" +
                "id INTEGER PRIMARY KEY, patient_id INTEGER NOT NULL, created_at TEXT NOT NULL, " +
                "created_by_employee TEXT, note_text TEXT NOT NULL)");
            database.ExecuteNonQuery(
                "CREATE TABLE IF NOT EXISTS patient_warnings (" +
                "id INTEGER PRIMARY KEY, patient_id INTEGER NOT NULL, created_at TEXT NOT NULL, reason TEXT NOT NULL)");

            EnsureColumn("patients", "birth_date", "TEXT");
            EnsureColumn("patients", "notes", "TEXT");
            EnsureColumn("employees", "first_name", "TEXT");
            EnsureColumn("employees", "last_name", "TEXT");
            EnsureColumn("employees", "pesel", "TEXT");
            EnsureColumn("employees", "birth_date", "TEXT");
            EnsureColumn("employees", "is_doctor", "INTEGER NOT NULL DEFAULT 0");
            EnsureColumn("employees", "specialization", "TEXT");
            EnsureColumn("patient_notes", "created_by_employee", "TEXT");

            database.ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_patients_search ON patients(pesel, first_name, last_name, birth_date, phone, email)");
            database.ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_appointments_doctor_date ON appointments(doctor_id, start_at, status)");
            database.ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_appointments_patient ON appointments(patient_id, start_at, status)");
            database.ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_employees_search ON employees(first_name, last_name, pesel, birth_date, login)");
        }

        private void EnsureColumn(string tableName, string columnName, string type)
        {
            try
            {
                database.ExecuteNonQuery("ALTER TABLE " + tableName + " ADD COLUMN " + columnName + " " + type);
            }
            catch
            {
                // Older SQLite builds do not support ADD COLUMN IF NOT EXISTS.
            }
        }

        private void SeedIfEmpty()
        {
            ClinicSeedData seed = SampleData.Create();

            if (database.Count("doctors") == 0)
            {
                foreach (Doctor doctor in seed.Doctors)
                {
                    InsertDoctor(doctor);
                }
            }

            if (database.Count("services") == 0)
            {
                foreach (MedicalService service in seed.Services)
                {
                    InsertService(service);
                }
            }

            if (database.Count("schedules") == 0)
            {
                foreach (ScheduleEntry schedule in seed.Schedules)
                {
                    InsertSchedule(schedule);
                }
            }

            if (database.Count("patients") == 0)
            {
                foreach (Patient patient in seed.Patients)
                {
                    InsertPatient(patient);
                }
            }

            if (database.Count("appointments") == 0)
            {
                foreach (Appointment appointment in seed.Appointments)
                {
                    InsertAppointment(appointment);
                }
            }

            if (database.Count("patient_notes") == 0)
            {
                foreach (PatientNote note in seed.PatientNotes)
                {
                    InsertPatientNote(note);
                }
            }

            if (database.Count("patient_warnings") == 0)
            {
                foreach (PatientWarning warning in seed.PatientWarnings)
                {
                    InsertPatientWarning(warning);
                }
            }

            foreach (Employee employee in seed.Employees)
            {
                if (!EmployeeLoginExists(employee.Login)
                    && (employee.IsAdministrator || string.Equals(employee.Login, "rejestrator", StringComparison.OrdinalIgnoreCase)))
                {
                    InsertEmployee(employee);
                }
            }
        }

        private ClinicSeedData LoadData()
        {
            var data = new ClinicSeedData();
            data.Doctors.AddRange(database.Query("SELECT id, first_name, last_name, specialization FROM doctors").Select(ReadDoctor));
            data.Services.AddRange(database.Query("SELECT id, name, specialization, is_active FROM services").Select(ReadService));
            data.Schedules.AddRange(database.Query("SELECT id, doctor_id, day_of_week, start_time, end_time FROM schedules").Select(ReadSchedule));
            data.Patients.AddRange(database.Query("SELECT id, first_name, last_name, pesel, birth_date, phone, email, address, notes, warning_count, blocked_until FROM patients").Select(ReadPatient));
            data.Appointments.AddRange(database.Query("SELECT id, doctor_id, patient_id, start_at, status, cancel_reason, notes FROM appointments").Select(ReadAppointment));
            data.Employees.AddRange(database.Query("SELECT id, login, first_name, last_name, pesel, birth_date, display_name, role, password_hash, password_salt, created_at, is_active, is_doctor, specialization FROM employees").Select(ReadEmployee));
            data.PatientNotes.AddRange(database.Query("SELECT id, patient_id, created_at, created_by_employee, note_text FROM patient_notes").Select(ReadPatientNote));
            data.PatientWarnings.AddRange(database.Query("SELECT id, patient_id, created_at, reason FROM patient_warnings").Select(ReadPatientWarning));
            return data;
        }

        private bool EmployeeLoginExists(string login)
        {
            object result = database.ExecuteScalar(
                "SELECT COUNT(*) FROM employees WHERE lower(login) = lower(?)",
                SqlValue.Text(login));
            return Convert.ToInt32(result, CultureInfo.InvariantCulture) > 0;
        }

        private void InsertPatient(Patient patient)
        {
            database.ExecuteNonQuery(
                "INSERT OR REPLACE INTO patients(id, first_name, last_name, pesel, birth_date, phone, email, address, notes, warning_count, blocked_until) " +
                "VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                SqlValue.Int(patient.Id),
                SqlValue.Text(patient.FirstName),
                SqlValue.Text(patient.LastName),
                SqlValue.Text(patient.Pesel),
                SqlValue.Date(patient.BirthDate),
                SqlValue.Text(patient.Phone),
                SqlValue.Text(patient.Email),
                SqlValue.Text(patient.Address),
                SqlValue.Text(patient.Notes),
                SqlValue.Int(patient.WarningCount),
                SqlValue.Date(patient.BlockedUntil));
        }

        private void UpdatePatient(Patient patient)
        {
            if (patient != null)
            {
                InsertPatient(patient);
            }
        }

        private void InsertDoctor(Doctor doctor)
        {
            database.ExecuteNonQuery(
                "INSERT OR REPLACE INTO doctors(id, first_name, last_name, specialization) VALUES(?, ?, ?, ?)",
                SqlValue.Int(doctor.Id),
                SqlValue.Text(doctor.FirstName),
                SqlValue.Text(doctor.LastName),
                SqlValue.Text(doctor.Specialization));
        }

        private void InsertService(MedicalService service)
        {
            database.ExecuteNonQuery(
                "INSERT OR REPLACE INTO services(id, name, specialization, is_active) VALUES(?, ?, ?, ?)",
                SqlValue.Int(service.Id),
                SqlValue.Text(service.Name),
                SqlValue.Text(service.Specialization),
                SqlValue.Bool(service.IsActive));
        }

        private void InsertSchedule(ScheduleEntry schedule)
        {
            database.ExecuteNonQuery(
                "INSERT OR REPLACE INTO schedules(id, doctor_id, day_of_week, start_time, end_time) VALUES(?, ?, ?, ?, ?)",
                SqlValue.Int(schedule.Id),
                SqlValue.Int(schedule.DoctorId),
                SqlValue.Int((int)schedule.DayOfWeek),
                SqlValue.Text(schedule.StartTime.ToString()),
                SqlValue.Text(schedule.EndTime.ToString()));
        }

        private void InsertAppointment(Appointment appointment)
        {
            database.ExecuteNonQuery(
                "INSERT OR REPLACE INTO appointments(id, doctor_id, patient_id, start_at, status, cancel_reason, notes) VALUES(?, ?, ?, ?, ?, ?, ?)",
                SqlValue.Int(appointment.Id),
                SqlValue.Int(appointment.DoctorId),
                appointment.PatientId.HasValue ? SqlValue.Int(appointment.PatientId.Value) : SqlValue.Null(),
                SqlValue.Text(ToDbDateTime(appointment.StartAt)),
                SqlValue.Text(appointment.Status.ToString()),
                SqlValue.Text(appointment.CancelReason),
                SqlValue.Text(appointment.Notes));
        }

        private void UpdateAppointment(Appointment appointment)
        {
            InsertAppointment(appointment);
        }

        private void InsertEmployee(Employee employee)
        {
            database.ExecuteNonQuery(
                "INSERT OR REPLACE INTO employees(id, login, first_name, last_name, pesel, birth_date, display_name, role, password_hash, password_salt, created_at, is_active, is_doctor, specialization) " +
                "VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                SqlValue.Int(employee.Id),
                SqlValue.Text(employee.Login),
                SqlValue.Text(employee.FirstName),
                SqlValue.Text(employee.LastName),
                SqlValue.Text(employee.Pesel),
                SqlValue.Date(employee.BirthDate),
                SqlValue.Text(employee.FullName),
                SqlValue.Text(EmployeeRoles.Normalize(employee.Role)),
                SqlValue.Text(employee.PasswordHash),
                SqlValue.Text(employee.PasswordSalt),
                SqlValue.Text(ToDbDateTime(employee.CreatedAt)),
                SqlValue.Bool(employee.IsActive),
                SqlValue.Bool(employee.IsDoctor),
                SqlValue.Text(employee.Specialization));
        }

        private void InsertPatientNote(PatientNote note)
        {
            database.ExecuteNonQuery(
                "INSERT OR REPLACE INTO patient_notes(id, patient_id, created_at, created_by_employee, note_text) VALUES(?, ?, ?, ?, ?)",
                SqlValue.Int(note.Id),
                SqlValue.Int(note.PatientId),
                SqlValue.Text(ToDbDateTime(note.CreatedAt)),
                SqlValue.Text(note.CreatedByEmployee),
                SqlValue.Text(note.Text));
        }

        private void InsertPatientWarning(PatientWarning warning)
        {
            database.ExecuteNonQuery(
                "INSERT OR REPLACE INTO patient_warnings(id, patient_id, created_at, reason) VALUES(?, ?, ?, ?)",
                SqlValue.Int(warning.Id),
                SqlValue.Int(warning.PatientId),
                SqlValue.Text(ToDbDateTime(warning.CreatedAt)),
                SqlValue.Text(warning.Reason));
        }

        private static Doctor ReadDoctor(Dictionary<string, object> row)
        {
            return new Doctor
            {
                Id = RowInt(row, "id"),
                FirstName = RowText(row, "first_name"),
                LastName = RowText(row, "last_name"),
                Specialization = RowText(row, "specialization")
            };
        }

        private static MedicalService ReadService(Dictionary<string, object> row)
        {
            return new MedicalService
            {
                Id = RowInt(row, "id"),
                Name = RowText(row, "name"),
                Specialization = RowText(row, "specialization"),
                IsActive = RowBool(row, "is_active")
            };
        }

        private static ScheduleEntry ReadSchedule(Dictionary<string, object> row)
        {
            return new ScheduleEntry
            {
                Id = RowInt(row, "id"),
                DoctorId = RowInt(row, "doctor_id"),
                DayOfWeek = (DayOfWeek)RowInt(row, "day_of_week"),
                StartTime = TimeSpan.Parse(RowText(row, "start_time"), CultureInfo.InvariantCulture),
                EndTime = TimeSpan.Parse(RowText(row, "end_time"), CultureInfo.InvariantCulture)
            };
        }

        private static Patient ReadPatient(Dictionary<string, object> row)
        {
            return new Patient
            {
                Id = RowInt(row, "id"),
                FirstName = RowText(row, "first_name"),
                LastName = RowText(row, "last_name"),
                Pesel = RowText(row, "pesel"),
                BirthDate = RowDate(row, "birth_date"),
                Phone = RowText(row, "phone"),
                Email = RowText(row, "email"),
                Address = RowText(row, "address"),
                Notes = RowText(row, "notes"),
                WarningCount = RowInt(row, "warning_count"),
                BlockedUntil = RowDate(row, "blocked_until")
            };
        }

        private static Appointment ReadAppointment(Dictionary<string, object> row)
        {
            return new Appointment
            {
                Id = RowInt(row, "id"),
                DoctorId = RowInt(row, "doctor_id"),
                PatientId = RowHasValue(row, "patient_id") ? (int?)RowInt(row, "patient_id") : null,
                StartAt = RowDateTime(row, "start_at") ?? DateTime.Today,
                Status = (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), RowText(row, "status")),
                CancelReason = RowText(row, "cancel_reason"),
                Notes = RowText(row, "notes")
            };
        }

        private static Employee ReadEmployee(Dictionary<string, object> row)
        {
            var employee = new Employee
            {
                Id = RowInt(row, "id"),
                Login = RowText(row, "login"),
                FirstName = RowText(row, "first_name"),
                LastName = RowText(row, "last_name"),
                Pesel = RowText(row, "pesel"),
                BirthDate = RowDate(row, "birth_date"),
                DisplayName = RowText(row, "display_name"),
                Role = EmployeeRoles.Normalize(RowText(row, "role")),
                PasswordHash = RowText(row, "password_hash"),
                PasswordSalt = RowText(row, "password_salt"),
                CreatedAt = RowDateTime(row, "created_at") ?? DateTime.Now,
                IsActive = RowBool(row, "is_active"),
                IsDoctor = RowBool(row, "is_doctor"),
                Specialization = RowText(row, "specialization")
            };

            if (string.IsNullOrWhiteSpace(employee.DisplayName))
            {
                employee.DisplayName = employee.FullName;
            }

            return employee;
        }

        private static PatientNote ReadPatientNote(Dictionary<string, object> row)
        {
            return new PatientNote
            {
                Id = RowInt(row, "id"),
                PatientId = RowInt(row, "patient_id"),
                CreatedAt = RowDateTime(row, "created_at") ?? DateTime.Now,
                CreatedByEmployee = RowText(row, "created_by_employee"),
                Text = RowText(row, "note_text")
            };
        }

        private static PatientWarning ReadPatientWarning(Dictionary<string, object> row)
        {
            return new PatientWarning
            {
                Id = RowInt(row, "id"),
                PatientId = RowInt(row, "patient_id"),
                CreatedAt = RowDateTime(row, "created_at") ?? DateTime.Now,
                Reason = RowText(row, "reason")
            };
        }

        private static bool RowHasValue(Dictionary<string, object> row, string name)
        {
            return row.ContainsKey(name) && row[name] != null;
        }

        private static string RowText(Dictionary<string, object> row, string name)
        {
            return RowHasValue(row, name) ? Convert.ToString(row[name], CultureInfo.InvariantCulture) : string.Empty;
        }

        private static int RowInt(Dictionary<string, object> row, string name)
        {
            return RowHasValue(row, name) ? Convert.ToInt32(row[name], CultureInfo.InvariantCulture) : 0;
        }

        private static bool RowBool(Dictionary<string, object> row, string name)
        {
            return RowInt(row, name) == 1;
        }

        private static DateTime? RowDate(Dictionary<string, object> row, string name)
        {
            string value = RowText(row, name);
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return DateTime.Parse(value, CultureInfo.InvariantCulture).Date;
        }

        private static DateTime? RowDateTime(Dictionary<string, object> row, string name)
        {
            string value = RowText(row, name);
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        private static string ToDbDateTime(DateTime value)
        {
            return value.ToString("o", CultureInfo.InvariantCulture);
        }
    }

    internal sealed class SqlValue
    {
        public object Value { get; private set; }

        private SqlValue(object value)
        {
            Value = value;
        }

        public static SqlValue Text(string value)
        {
            return new SqlValue(value);
        }

        public static SqlValue Int(int value)
        {
            return new SqlValue(value);
        }

        public static SqlValue Bool(bool value)
        {
            return new SqlValue(value ? 1 : 0);
        }

        public static SqlValue Date(DateTime? value)
        {
            return new SqlValue(value.HasValue ? value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null);
        }

        public static SqlValue Null()
        {
            return new SqlValue(null);
        }
    }

    internal sealed class NativeSqliteDatabase
    {
        private const int SQLITE_OK = 0;
        private const int SQLITE_ROW = 100;
        private const int SQLITE_DONE = 101;
        private const int SQLITE_OPEN_READWRITE = 0x00000002;
        private const int SQLITE_OPEN_CREATE = 0x00000004;
        private const int SQLITE_INTEGER = 1;
        private const int SQLITE_TEXT = 3;
        private const int SQLITE_NULL = 5;
        private static readonly IntPtr SQLITE_TRANSIENT = new IntPtr(-1);
        private readonly string databasePath;

        public NativeSqliteDatabase(string databasePath)
        {
            this.databasePath = databasePath;
        }

        public int Count(string tableName)
        {
            object result = ExecuteScalar("SELECT COUNT(*) FROM " + tableName);
            return Convert.ToInt32(result, CultureInfo.InvariantCulture);
        }

        public object ExecuteScalar(string sql, params SqlValue[] parameters)
        {
            List<Dictionary<string, object>> rows = Query(sql, parameters);
            if (rows.Count == 0 || rows[0].Count == 0)
            {
                return null;
            }

            return rows[0].Values.FirstOrDefault();
        }

        public void ExecuteNonQuery(string sql, params SqlValue[] parameters)
        {
            IntPtr db = Open();
            IntPtr statement = IntPtr.Zero;
            try
            {
                statement = Prepare(db, sql);
                Bind(statement, parameters);
                int result = sqlite3_step(statement);
                if (result != SQLITE_DONE && result != SQLITE_ROW)
                {
                    ThrowSqlite(db, result);
                }
            }
            finally
            {
                FinalizeStatement(statement);
                Close(db);
            }
        }

        public List<Dictionary<string, object>> Query(string sql, params SqlValue[] parameters)
        {
            IntPtr db = Open();
            IntPtr statement = IntPtr.Zero;
            try
            {
                statement = Prepare(db, sql);
                Bind(statement, parameters);

                var rows = new List<Dictionary<string, object>>();
                while (true)
                {
                    int result = sqlite3_step(statement);
                    if (result == SQLITE_DONE)
                    {
                        return rows;
                    }

                    if (result != SQLITE_ROW)
                    {
                        ThrowSqlite(db, result);
                    }

                    rows.Add(ReadRow(statement));
                }
            }
            finally
            {
                FinalizeStatement(statement);
                Close(db);
            }
        }

        private IntPtr Open()
        {
            IntPtr db;
            int result = sqlite3_open_v2(ToUtf8Bytes(databasePath), out db, SQLITE_OPEN_READWRITE | SQLITE_OPEN_CREATE, IntPtr.Zero);
            if (result != SQLITE_OK)
            {
                string message = GetSqliteMessage(db, result);
                Close(db);
                throw new InvalidOperationException(message);
            }

            IntPtr statement = IntPtr.Zero;
            try
            {
                statement = Prepare(db, "PRAGMA foreign_keys = ON;");
                result = sqlite3_step(statement);
                if (result != SQLITE_DONE && result != SQLITE_ROW)
                {
                    ThrowSqlite(db, result);
                }
            }
            catch
            {
                Close(db);
                throw;
            }
            finally
            {
                FinalizeStatement(statement);
            }

            return db;
        }

        private static IntPtr Prepare(IntPtr db, string sql)
        {
            IntPtr statement;
            int result = sqlite3_prepare_v2(db, ToUtf8Bytes(sql), -1, out statement, IntPtr.Zero);
            if (result != SQLITE_OK)
            {
                ThrowSqlite(db, result);
            }

            return statement;
        }

        private static void Bind(IntPtr statement, SqlValue[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                object value = parameters[i] == null ? null : parameters[i].Value;
                int index = i + 1;
                int result;

                if (value == null)
                {
                    result = sqlite3_bind_null(statement, index);
                }
                else if (value is int)
                {
                    result = sqlite3_bind_int(statement, index, (int)value);
                }
                else
                {
                    byte[] bytes = ToUtf8Bytes(Convert.ToString(value, CultureInfo.InvariantCulture));
                    result = sqlite3_bind_text(statement, index, bytes, bytes.Length - 1, SQLITE_TRANSIENT);
                }

                if (result != SQLITE_OK)
                {
                    throw new InvalidOperationException("Nie udalo sie powiazac parametru SQLite.");
                }
            }
        }

        private static Dictionary<string, object> ReadRow(IntPtr statement)
        {
            int count = sqlite3_column_count(statement);
            var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < count; i++)
            {
                string name = PtrToStringUtf8(sqlite3_column_name(statement, i));
                int type = sqlite3_column_type(statement, i);

                if (type == SQLITE_NULL)
                {
                    row[name] = null;
                }
                else if (type == SQLITE_INTEGER)
                {
                    row[name] = sqlite3_column_int(statement, i);
                }
                else if (type == SQLITE_TEXT)
                {
                    row[name] = PtrToStringUtf8(sqlite3_column_text(statement, i), sqlite3_column_bytes(statement, i));
                }
                else
                {
                    row[name] = PtrToStringUtf8(sqlite3_column_text(statement, i), sqlite3_column_bytes(statement, i));
                }
            }

            return row;
        }

        private static byte[] ToUtf8Bytes(string value)
        {
            byte[] source = Encoding.UTF8.GetBytes(value ?? string.Empty);
            var target = new byte[source.Length + 1];
            Buffer.BlockCopy(source, 0, target, 0, source.Length);
            return target;
        }

        private static string PtrToStringUtf8(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
            {
                return string.Empty;
            }

            int length = 0;
            while (Marshal.ReadByte(pointer, length) != 0)
            {
                length++;
            }

            return PtrToStringUtf8(pointer, length);
        }

        private static string PtrToStringUtf8(IntPtr pointer, int byteCount)
        {
            if (pointer == IntPtr.Zero || byteCount <= 0)
            {
                return string.Empty;
            }

            var bytes = new byte[byteCount];
            Marshal.Copy(pointer, bytes, 0, byteCount);
            return Encoding.UTF8.GetString(bytes);
        }

        private static void FinalizeStatement(IntPtr statement)
        {
            if (statement != IntPtr.Zero)
            {
                sqlite3_finalize(statement);
            }
        }

        private static void Close(IntPtr db)
        {
            if (db != IntPtr.Zero)
            {
                sqlite3_close(db);
            }
        }

        private static void ThrowSqlite(IntPtr db, int result)
        {
            throw new InvalidOperationException(GetSqliteMessage(db, result));
        }

        private static string GetSqliteMessage(IntPtr db, int result)
        {
            return db == IntPtr.Zero ? "SQLite error " + result : PtrToStringUtf8(sqlite3_errmsg(db));
        }

        [DllImport("winsqlite3.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int sqlite3_open_v2(byte[] filename, out IntPtr db, int flags, IntPtr vfs);

        [DllImport("winsqlite3.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int sqlite3_prepare_v2(IntPtr db, byte[] sql, int nByte, out IntPtr statement, IntPtr tail);

        [DllImport("winsqlite3.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int sqlite3_step(IntPtr statement);

        [DllImport("winsqlite3.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int sqlite3_finalize(IntPtr statement);

        [DllImport("winsqlite3.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int sqlite3_close(IntPtr db);

        [DllImport("winsqlite3.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int sqlite3_bind_null(IntPtr statement, int index);

        [DllImport("winsqlite3.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int sqlite3_bind_int(IntPtr statement, int index, int value);

        [DllImport("winsqlite3.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int sqlite3_bind_text(IntPtr statement, int index, byte[] value, int bytes, IntPtr destructor);

        [DllImport("winsqlite3.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int sqlite3_column_count(IntPtr statement);

        [DllImport("winsqlite3.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int sqlite3_column_type(IntPtr statement, int column);

        [DllImport("winsqlite3.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int sqlite3_column_int(IntPtr statement, int column);

        [DllImport("winsqlite3.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr sqlite3_column_text(IntPtr statement, int column);

        [DllImport("winsqlite3.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int sqlite3_column_bytes(IntPtr statement, int column);

        [DllImport("winsqlite3.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr sqlite3_column_name(IntPtr statement, int column);

        [DllImport("winsqlite3.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr sqlite3_errmsg(IntPtr db);
    }
}
