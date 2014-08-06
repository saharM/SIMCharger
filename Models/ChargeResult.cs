using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMCharger.Models
{
    public class ChargeResult
    {
        public string MobileNumber { get; set; }
        public string Data { get; set; }
        public string Result { get; set; }
        public override string ToString()
        {
            return string.Format("Mobile={0}, Data={1}, Result={2}",
                MobileNumber, Data, Result);
        }
    }
}
