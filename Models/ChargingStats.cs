using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMCharger.Models
{
    struct ChargingStats
    {        
		public string CurrentAction { get; set; }
		public string CurrentTarget { get; set; }
		public int ChargeCount { get; set; }

		public void Clear()
		{
			this.CurrentAction = "Idle";
			this.CurrentTarget = "";
			this.ChargeCount = 0;
		}
    }
}
