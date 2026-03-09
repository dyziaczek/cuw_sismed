namespace CUW_SISMED.Models
{
    public class Schedule
    {
        public int ID { get; set; }
        public string PWZ { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}