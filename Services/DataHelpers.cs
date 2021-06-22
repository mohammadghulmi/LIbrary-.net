using Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Services
{
    public class DataHelpers
    {
        public static IEnumerable<string> HumanizeHours(IEnumerable<BranchHours> branchHours)
        {
            var hours = new List<string>();
            foreach(var Time in branchHours)
            {
                var day = Enum.GetName(typeof(DayOfWeek), Time.DayOfWeek);
                var OpenTime = TimeSpan.FromHours(Time.OpenTime).ToString("hh':'mm");
                var CLoseTime = TimeSpan.FromHours(Time.CloseTime).ToString("hh':'mm");
                var Entry = $"{day} {OpenTime} to {CLoseTime}";
                hours.Add(Entry);
            }
            return hours;
        }
    }
}
