using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib
{
    public class UserCredential
    {
        public int CredentialId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int StaffId { get; set; }

        public UserCredential(int id, string login, string password, int staffId)
        {
            CredentialId = id;
            Login = login;
            Password = password;
            StaffId = staffId;
        }
    }
}
