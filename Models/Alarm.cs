using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinalEscolar.Models
{
    public class Alarm
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Day { get; set; }
        public string Time { get; set; }
        public string Song { get; set; }
        public int IntervalInSeconds { get; set; }
    }
}
