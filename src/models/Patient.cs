namespace CUW_SISMED.Models
{
    public class Patient
    {
        public int PID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PESEL { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string Address { get; set; }
        public int Warnings { get; set; }
        public DateTime? BlockedUntil { get; set; }
    }
}