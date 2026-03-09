using System;
using CUW_SISMED.Models;

namespace CUW_SISMED.Services
{
    public class Reservation
    {
        public Appointment CreateAppointment(string pwz, int pid, DateTime date, TimeSpan time)
        {
            return new Appointment
            {
                PWZ = pwz,
                PID = pid,
                Date = date,
                Time = time,
                Status = "Reserved"
            };
        }

        public void CancelAppointment(Appointment appointment, string reason)
        {
            appointment.Status = "Cancelled";
            appointment.CancelReason = reason;
        }
    }
}