using System;

namespace VS_CUWSISMED
{
    public static class EmployeeRoles
    {
        public const string Administrator = "Administrator";
        public const string Reception = "Rejestracja";

        public static string Normalize(string role)
        {
            if (string.Equals(role, Administrator, StringComparison.OrdinalIgnoreCase))
            {
                return Administrator;
            }

            return Reception;
        }
    }

    public enum AppointmentStatus
    {
        Free,
        Reserved,
        Cancelled
    }

    public sealed class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Pesel { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Notes { get; set; }
        public int WarningCount { get; set; }
        public DateTime? BlockedUntil { get; set; }

        public string DisplayName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName).Trim();
            }
        }

        public bool IsBlocked
        {
            get
            {
                return BlockedUntil.HasValue && BlockedUntil.Value.Date >= DateTime.Today;
            }
        }
    }

    public sealed class Doctor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialization { get; set; }

        public string DisplayName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName).Trim();
            }
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", DisplayName, Specialization);
        }
    }

    public sealed class Appointment
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int? PatientId { get; set; }
        public DateTime StartAt { get; set; }
        public AppointmentStatus Status { get; set; }
        public string CancelReason { get; set; }
        public string Notes { get; set; }

        public string StatusText
        {
            get
            {
                if (Status == AppointmentStatus.Reserved)
                {
                    return "Zajęty";
                }

                if (Status == AppointmentStatus.Cancelled)
                {
                    return "Anulowany";
                }

                return "Wolny";
            }
        }
    }

    public sealed class ScheduleEntry
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public sealed class MedicalService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class PatientNote
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Text { get; set; }
    }

    public sealed class PatientWarning
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Reason { get; set; }
    }

    public sealed class Employee
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Pesel { get; set; }
        public DateTime? BirthDate { get; set; }
        public string DisplayName { get; set; }
        public string Role { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsDoctor { get; set; }
        public string Specialization { get; set; }

        public string FullName
        {
            get
            {
                string fullName = string.Format("{0} {1}", FirstName, LastName).Trim();
                return string.IsNullOrWhiteSpace(fullName) ? DisplayName : fullName;
            }
        }

        public bool IsAdministrator
        {
            get
            {
                return string.Equals(Role, EmployeeRoles.Administrator, StringComparison.OrdinalIgnoreCase);
            }
        }

        public string StatusText
        {
            get
            {
                return IsActive ? "Aktywny" : "Nieaktywny";
            }
        }
    }

    public sealed class AvailableSlot
    {
        public Doctor Doctor { get; set; }
        public DateTime StartAt { get; set; }
    }

    public sealed class PatientSearchCriteria
    {
        public string Pesel { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrWhiteSpace(Pesel)
                    && string.IsNullOrWhiteSpace(FirstName)
                    && string.IsNullOrWhiteSpace(LastName)
                    && !BirthDate.HasValue
                    && string.IsNullOrWhiteSpace(Phone)
                    && string.IsNullOrWhiteSpace(Email);
            }
        }
    }

    public sealed class RegistrationResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }
        public Employee Employee { get; private set; }

        public static RegistrationResult Ok(Employee employee)
        {
            return new RegistrationResult
            {
                Success = true,
                Message = "Konto pracownika zostało utworzone.",
                Employee = employee
            };
        }

        public static RegistrationResult Fail(string message)
        {
            return new RegistrationResult
            {
                Success = false,
                Message = message
            };
        }
    }
}
