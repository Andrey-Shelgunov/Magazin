using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib
{
    public class Staff
    {
        public int StaffId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }

        public string FullName
        {
            get { return $"{FirstName} {LastName}"; }
        }

        public Staff(int id, string firstName, string lastName, int roleId, bool isActive = true)
        {
            StaffId = id;
            FirstName = firstName;
            LastName = lastName;
            RoleId = roleId;
            IsActive = isActive;
        }
    }
}
