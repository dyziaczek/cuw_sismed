using System;
using System.Collections.Generic;

namespace VS_CUWSISMED
{
    internal sealed class ClinicSeedData
    {
        public List<Patient> Patients { get; private set; }
        public List<Doctor> Doctors { get; private set; }
        public List<ScheduleEntry> Schedules { get; private set; }
        public List<Appointment> Appointments { get; private set; }
        public List<Employee> Employees { get; private set; }
        public List<MedicalService> Services { get; private set; }
        public List<PatientNote> PatientNotes { get; private set; }
        public List<PatientWarning> PatientWarnings { get; private set; }
        public List<SismedDocument> Documents { get; private set; }

        public ClinicSeedData()
        {
            Patients = new List<Patient>();
            Doctors = new List<Doctor>();
            Schedules = new List<ScheduleEntry>();
            Appointments = new List<Appointment>();
            Employees = new List<Employee>();
            Services = new List<MedicalService>();
            PatientNotes = new List<PatientNote>();
            PatientWarnings = new List<PatientWarning>();
            Documents = new List<SismedDocument>();
        }
    }

    internal static class SampleData
    {
        public static ClinicSeedData Create()
        {
            var data = new ClinicSeedData();

            data.Employees.Add(CreateEmployee(
                1,
                "admin",
                "Administrator",
                "SISMED",
                "75010112345",
                new DateTime(1975, 1, 1),
                EmployeeRoles.Administrator,
                "admin",
                false,
                null));

            data.Employees.Add(CreateEmployee(
                2,
                "rejestrator",
                "Rejestrator",
                "SISMED",
                "85050554321",
                new DateTime(1985, 5, 5),
                EmployeeRoles.Reception,
                "admin",
                false,
                null));

            data.Patients.Add(new Patient
            {
                Id = 1,
                FirstName = "Anna",
                LastName = "Kowalska",
                Pesel = "82010112345",
                BirthDate = new DateTime(1982, 1, 1),
                Phone = "501222333",
                Email = "anna.kowalska@example.com",
                Address = "Warszawa, 01-001, Marszałkowska, 10, 5",
                Notes = string.Empty,
                WarningCount = 0
            });

            data.Patients.Add(new Patient
            {
                Id = 2,
                FirstName = "Jan",
                LastName = "Kowalski",
                Pesel = "90050554321",
                BirthDate = new DateTime(1990, 5, 5),
                Phone = "502333444",
                Email = "jan.kowalski@example.com",
                Address = "Warszawa, 01-002, Puławska, 20, 8",
                Notes = string.Empty,
                WarningCount = 0
            });

            data.Doctors.Add(new Doctor
            {
                Id = 1,
                FirstName = "Maria",
                LastName = "Zielinska",
                Specialization = "Internista"
            });

            data.Services.Add(new MedicalService
            {
                Id = 1,
                Name = "Konsultacja internistyczna",
                Specialization = "Internista",
                IsActive = true
            });

            data.Services.Add(new MedicalService
            {
                Id = 2,
                Name = "Konsultacja kardiologiczna",
                Specialization = "Kardiolog",
                IsActive = true
            });

            data.Services.Add(new MedicalService
            {
                Id = 3,
                Name = "Konsultacja dermatologiczna",
                Specialization = "Dermatolog",
                IsActive = true
            });

            data.Doctors.Add(new Doctor
            {
                Id = 2,
                FirstName = "Piotr",
                LastName = "Wisniewski",
                Specialization = "Kardiolog"
            });

            data.Doctors.Add(new Doctor
            {
                Id = 3,
                FirstName = "Ewa",
                LastName = "Lewandowska",
                Specialization = "Dermatolog"
            });

            data.Employees.Add(CreateEmployee(
                3,
                "mzielinska",
                "Maria",
                "Zielinska",
                "79030311111",
                new DateTime(1979, 3, 3),
                EmployeeRoles.Reception,
                "demo",
                true,
                "Internista"));

            data.Employees.Add(CreateEmployee(
                4,
                "pwisniewski",
                "Piotr",
                "Wisniewski",
                "81040422222",
                new DateTime(1981, 4, 4),
                EmployeeRoles.Reception,
                "demo",
                true,
                "Kardiolog"));

            data.Employees.Add(CreateEmployee(
                5,
                "elewandowska",
                "Ewa",
                "Lewandowska",
                "83050533333",
                new DateTime(1983, 5, 5),
                EmployeeRoles.Reception,
                "demo",
                true,
                "Dermatolog"));

            int scheduleId = 1;
            foreach (var doctor in data.Doctors)
            {
                for (int day = (int)DayOfWeek.Monday; day <= (int)DayOfWeek.Saturday; day++)
                {
                    data.Schedules.Add(new ScheduleEntry
                    {
                        Id = scheduleId++,
                        DoctorId = doctor.Id,
                        DayOfWeek = (DayOfWeek)day,
                        StartTime = TimeSpan.FromHours(7),
                        EndTime = TimeSpan.FromHours(18)
                    });
                }
            }

            data.Appointments.Add(new Appointment
            {
                Id = 1,
                DoctorId = 1,
                PatientId = 1,
                StartAt = DateTime.Today.AddDays(1).AddHours(9),
                Status = AppointmentStatus.Reserved,
                Notes = "Pierwsza wizyta"
            });

            data.Appointments.Add(new Appointment
            {
                Id = 2,
                DoctorId = 2,
                PatientId = 2,
                StartAt = DateTime.Today.AddDays(1).AddHours(10).AddMinutes(15),
                Status = AppointmentStatus.Reserved,
                Notes = "Kontrola"
            });

            data.Documents.Add(new SismedDocument
            {
                Id = 1,
                Title = "Procedura rejestracji pacjenta",
                Category = "Recepcja",
                Content = "Zweryfikuj dane pacjenta, numer PESEL oraz aktualny numer telefonu przed umówieniem wizyty.",
                Author = "Administrator SISMED",
                CreatedAt = DateTime.Now.AddDays(-7),
                UpdatedAt = DateTime.Now.AddDays(-7),
                Status = DocumentStatus.Active,
                LastEditedBy = "Administrator SISMED"
            });

            data.Documents.Add(new SismedDocument
            {
                Id = 2,
                Title = "Zasady obsługi blokad rezerwacji",
                Category = "Wizyty",
                Content = "Po trzech ostrzeżeniach pacjent otrzymuje czasową blokadę rezerwacji zgodnie z zasadami placówki.",
                Author = "Administrator SISMED",
                CreatedAt = DateTime.Now.AddDays(-3),
                UpdatedAt = DateTime.Now.AddDays(-3),
                Status = DocumentStatus.Active,
                LastEditedBy = "Administrator SISMED"
            });

            return data;
        }

        private static Employee CreateEmployee(
            int id,
            string login,
            string firstName,
            string lastName,
            string pesel,
            DateTime birthDate,
            string role,
            string password,
            bool isDoctor,
            string specialization)
        {
            string salt;
            string hash = PasswordHasher.CreateHash(password, out salt);

            return new Employee
            {
                Id = id,
                Login = login,
                FirstName = firstName,
                LastName = lastName,
                Pesel = pesel,
                BirthDate = birthDate,
                DisplayName = string.Format("{0} {1}", firstName, lastName).Trim(),
                Role = EmployeeRoles.Normalize(role),
                PasswordHash = hash,
                PasswordSalt = salt,
                CreatedAt = DateTime.Now,
                IsActive = true,
                IsDoctor = isDoctor,
                Specialization = specialization
            };
        }
    }
}
