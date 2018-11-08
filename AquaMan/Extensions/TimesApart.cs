using AquaMan.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AquaMan.Extensions
{
    public class TimesApart : ValidationAttribute
    {
        double _distanceApart;
        public TimesApart(double distanceApart)
        {
            _distanceApart = distanceApart;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime?[] feedingTimes = ((FeedingTimes)validationContext.ObjectInstance).Feedings.Where(p => p != null).ToArray();

            if (feedingTimes.Count() > 0)
            {
                DateTime? tempTime = feedingTimes[0];
                double delta;
                for (int x = 1; x < feedingTimes.Count(); x++)
                {
                    delta = Math.Abs((GetCurrentDayTime(tempTime) - GetCurrentDayTime(feedingTimes[x])).TotalHours);
                    if (delta < _distanceApart)
                        return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            return $"Feeding times must be at least {_distanceApart.ToString()} appart.";
        }

        private DateTime GetCurrentDayTime(DateTime? dt)
        {   
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, dt.Value.Hour, dt.Value.Minute, dt.Value.Second);
        }
    }
}
