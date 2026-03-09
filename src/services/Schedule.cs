using System;
using System.Collections.Generic;

namespace CUW_SISMED.Services
{
    public class Schedule
    {
        public List<TimeSpan> GenerateSlots(TimeSpan start, TimeSpan end)
        {
            List<TimeSpan> slots = new List<TimeSpan>();

            TimeSpan current = start;

            while (current < end)
            {
                slots.Add(current);
                current = current.Add(TimeSpan.FromMinutes(15));
            }

            return slots;
        }
    }
}