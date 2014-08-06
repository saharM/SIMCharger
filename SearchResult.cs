using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMCharger
{
    public class SearchResult
    {
        public string MobileNumber { get; set; }
        public string ActivationDate { get; set; }
        public string LastChargeDate { get; set; }
        public string SimSerial { get; set; }
        public string RegFullNameOfOwner { get; set; }        
        public string Data { get; set; }
        public string Result { get; set; }
        public override string ToString()
        {
            return string.Format("Mobile={0}, ActivationDate={1}, LastChargeDate={2}",
                MobileNumber, ActivationDate, LastChargeDate);
        }
    }
}
