using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolDbProject.ClassLib
{
    internal class User
    {
        protected int ID;
        protected bool isAdmin;
        protected string username;
        protected string password;

        public User(int ID, string name, string password, bool isAdmin)
        {
            this.ID = ID;
            this.username = name;
            this.password = password;
            this.isAdmin = isAdmin;
        }
        public int Id
        {
            get { return ID; }
        }
        public bool IsAdmin
        {
            get { return isAdmin; }
        }
        public string Password
        {
            get { return password; }
        }
        public string UserName
        {
            get { return username; }
        }
    }
}
