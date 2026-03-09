namespace CUW_SISMED.Models
{
    public class Appointment
    {
        public int ID { get; set; }
        public string PWZ { get; set; }
        public int PID { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Status { get; set; }
        public string CancelReason { get; set; }
    }
}