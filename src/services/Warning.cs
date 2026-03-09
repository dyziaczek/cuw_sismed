using System;
using CUW_SISMED.Models;

namespace CUW_SISMED.Services
{
    public class Warning
    {
        public void AddWarning(Patient patient)
        {
            patient.Warnings++;

            if (patient.Warnings >= 3)
            {
                patient.BlockedUntil = DateTime.Now.AddDays(30);
            }
        }
    }
}