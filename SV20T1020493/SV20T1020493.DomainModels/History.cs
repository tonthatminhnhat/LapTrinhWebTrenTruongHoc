using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020493.DomainModels
{
    public class History
    {
        public int TrashBinID { get; set; }
        public DateTime TimeChange { get; set; } 
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; } = "";
        public string TableName { get; set; } = "";
        public string Work { get; set; } = "";
        public string Data { get; set; } = "";
        public string OldData { get; set; } = "";
    }
}
