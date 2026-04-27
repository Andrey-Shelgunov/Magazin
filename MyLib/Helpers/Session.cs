using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.Helpers
{
    
        public static class Session
    {
        public static Staff CurrentUser { get; set; }

        public static string CurrentRole
        {
            get
            {
                if (CurrentUser == null) return "Unknown";
                if (CurrentUser.RoleId == 1) return "Admin";
                if (CurrentUser.RoleId == 2) return "Cashier";
                if (CurrentUser.RoleId == 3) return "Warehouse";
                return "Unknown";
            }
        }

        public static bool IsAdmin
        {
            get { return CurrentUser != null && CurrentUser.RoleId == 1; }
        }

        public static bool IsCashier
        {
            get { return CurrentUser != null && CurrentUser.RoleId == 2; }
        }

        public static bool IsWarehouse
        {
            get { return CurrentUser != null && CurrentUser.RoleId == 3; }
        }

        public static void Clear()
        {
            CurrentUser = null;
        }
    }
}