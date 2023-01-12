using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolDbProject.ClassLib
{
    internal class Admin : User
    {
        public Admin(int ID, string name, string password, bool isAdmin) : base(ID, name, password, isAdmin)
        {

        }
    }
}
