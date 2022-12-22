using SchoolDbProject.ClassLib;
using SchoolDbProject.Data;

namespace SchoolDbProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DbAccess db = new DbAccess();

            db.Run();
        }
    }
}

