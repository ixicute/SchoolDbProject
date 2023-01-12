using SchoolDbProject.ClassLib;
using SchoolDbProject.Data;

namespace SchoolDbProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "School Database - 2022";
            DbAccess db = new DbAccess();            
            db.InitiateUser();
            db.Run();
        }
    }
}

