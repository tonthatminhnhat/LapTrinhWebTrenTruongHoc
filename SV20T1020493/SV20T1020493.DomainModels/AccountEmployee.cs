using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020493.DomainModels
{
    public class AccountEmployee
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string RoleNames { get; set; } = "";
    }
}
