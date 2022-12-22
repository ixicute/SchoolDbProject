using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SchoolDbProject.Data;
using SchoolDbProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SchoolDbProject.ClassLib
{
    internal class DbAccess
    {
        //This is the context used to  communicate with the database.
        private SchoolDbContext schoolDb = new SchoolDbContext();
        
        //Connection string. Edit "data source" to match server name and initial catalog to match database name.
        private SqlConnection sqlcon = new SqlConnection(@"Data Source = ALDOR007; Initial Catalog=SchoolDb; Integrated Security= true");

        /// <summary>
        /// Sets title for the application and runs Main-Menu of the program.
        /// </summary>
        public void Run()
        {
            Console.Title = "School Database - 2022";
            MainMenu();            
        }

        /// <summary>
        /// Configuration for the menu options. Edit this method to change menu-options.
        /// </summary>
        private void MainMenu()
        {
            //This string will hold the message on the top of the screen.
            string prompt = "Connected to School database - Successful\n" +
                            "(Use arrows to cycle through options and press enter to select it.)";

            //This string array holds all the menu options. Edit this to change the content of the menu.
            List<string> options = new List<string>() 
            { "Hämta personal", //Done
              "Hämta alla elever", //Done
              "Hämta klass-info", //Done
              "Hämta betyg från senaste månaden", 
              "Visa snittbetyg för alla kurser (Med högsta & lägsta betyget",
              "Lägg till ny elev",
              "Lägg till ny personal",
              "Avsluta"
            };

            Menu mainMenu = new Menu(prompt, options);

            //Runs the method and saves the returned value to be used in the switch-case menu below.
            int selectedIndex = mainMenu.PrintMenu();

            //Menu for selection.
            switch (selectedIndex)
            {
                case 0:
                    EmployeesSql();
                    break;
                case 1:
                    ViewStudentsEntity();
                    break;
                case 2:
                    StudentsByClassEntity();
                    break;
                case 3:
                    GetLatestGrade();                    
                    break;
                case 4:
                    GetCourseGradeAverageSql();
                    break;
                case 5:
                    InsertStudentSql();
                    break;
                case 6:
                    InsertEmployeeEntity();
                    break;
                case 7:
                    Exit();
                    break;
            }

            //Just in case, program exits if menu is skipped.
            Environment.Exit(0);
        }

        /// <summary>
        /// Prints a menu with options to print out all or specific employee-title from the school.
        /// </summary>
        private void EmployeesSql()
        {
            string prompt = "Välj titeln på personalen du vill hämta: ";

            //String of options for the menu
            List<string> options = new List<string>()
            { 
                "Visa All Personal",
                "Teacher",
                "Principal",
                "Janitor",
                "Tillbaka"
            };

            Menu EmpMenu = new Menu(prompt, options);

            //Menu returns an int based on the chosen option
            int selectedIndex = EmpMenu.PrintMenu();

            string sqlCommand = "SELECT Employee.FirstName, Employee.LastName, Title.TitleName FROM Employee " +
                             "Join Title ON Title.Id = FK_TitleId ";

            //The returned int is used to pick the 'where'-clause for the sql command.
            switch (selectedIndex)
            {
                //case 0 shows all personal by default, hence why its empty.
                case 0:
                    break;
                case 1:
                    sqlCommand += "WHERE Title.TitleName = 'Teacher'";
                    break;
                case 2:
                    sqlCommand += "WHERE Title.TitleName = 'Principal'";
                    break;
                case 3:
                    sqlCommand += "WHERE Title.TitleName = 'Janitor'";
                    break;
                case 4:
                    MainMenu();
                    break;
            }

            SqlDataAdapter sqlData = new SqlDataAdapter(sqlCommand, sqlcon);

            DataTable dataTable = new DataTable();

            sqlData.Fill(dataTable);

            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("|{0, -15} | {1, -15} | {2, -15}|", "Förnamn", "Efternamn", "Titel");
            Console.WriteLine(" {0}", new string('-', 51));
            Console.ResetColor();
            foreach (DataRow dr in dataTable.Rows)
            {
                //Formatted strings as tables
                Console.WriteLine("|{0, -15} | {1, -15} | {2, -15}|", dr["FirstName"], dr["LastName"], dr["TitleName"]);
            }
            Console.WriteLine(" {0}", new string('-', 51));
            Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
            Console.ReadKey();
            EmployeesSql();
        }

        /// <summary>
        /// Prints all of the available students with menu to sort them.
        /// </summary>
        private void ViewStudentsEntity()
        {
            //Gets all students from the database.
            var userChoice = from s in schoolDb.Students select s;

            //Copies students over to IQueryable variable.
            IQueryable<Student> sorted = userChoice;
            string sortPrompt = "Välj hur elevernas data ska sorteras: ";
            List<string> sortByOption = new List<string>()
            { "Förnamn Stigande (A först)",
              "Förnamn Fallande (Ö först)",
              "Efternamn Stigande (A först)",
              "Efternamn Fallande (Ö först)",
              "Tillbaka"
            };

            int sortIndex;

            Menu studentSortMenu = new Menu(sortPrompt, sortByOption);

            //Saves the int value returned by the menu based on user choice
            sortIndex = studentSortMenu.PrintMenu();

            switch (sortIndex)
            {
                case 0:
                    sorted = userChoice.OrderBy(x => x.FirstName);
                    break;
                case 1:
                    sorted = userChoice.OrderByDescending(x => x.FirstName);
                    break;
                case 2:
                    sorted = userChoice.OrderBy(x => x.LastName);
                    break;
                case 3:
                    sorted = userChoice.OrderByDescending(x => x.LastName);
                    break;
                case 4:
                    MainMenu();
                    break;
            }

            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("|{0, -15} | {1, -15} | {2, -15}|", "Förnamn", "Efternamn", "Personnummer");            
            Console.WriteLine(" {0}", new string('-', 51));
            Console.ResetColor();

            foreach (Student student in sorted)
            {
                Console.WriteLine("|{0, -15} | {1, -15} | {2, -15}|", student.FirstName, student.LastName, student.SsNumber);                
            }
            Console.WriteLine(" {0}", new string('-', 51));
            Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
            Console.ReadKey();
            ViewStudentsEntity();
        }

        /// <summary>
        /// Prints a menu with all available classes and prints out students in that chosen class.
        /// </summary>
        private void StudentsByClassEntity()
        {
            //save the name of the class that will be used to get students.
            string classChoice = ClassPicker();

            //get students based on the chosen class.
            var sortStudents = from c in schoolDb.Classes
                               join s in schoolDb.Students on c.Id equals s.FkClassId
                               where c.ClassName == classChoice
                               select new
                               {
                                   FirstN = s.FirstName,
                                   LastN = s.LastName,
                                   ClassN = c.ClassName
                               };
            
            var temp = sortStudents;

            string sortPrompt = "Välj hur elevernas data ska sorteras: ";
            List<string> sortByOption = new List<string>()
            { "Förnamn Stigande (A först)",
              "Förnamn Fallande (Ö först)",
              "Efternamn Stigande (A först)",
              "Efternamn Fallande (Ö först)",
              "Tillbaka"
            };

            int sortIndex;

            Menu studentSortMenu = new Menu(sortPrompt, sortByOption);
            
            sortIndex = studentSortMenu.PrintMenu();
            switch (sortIndex)
            {
                case 0:
                    temp = sortStudents.OrderBy(x => x.FirstN);                    
                    break;
                case 1:
                    temp = sortStudents.OrderByDescending(x => x.FirstN);
                    break;
                case 2:
                    temp = sortStudents.OrderBy(x => x.LastN);
                    break;
                case 3:
                    temp = sortStudents.OrderByDescending(x => x.LastN);                    
                    break;
                case 4:
                    StudentsByClassEntity();
                    break;
            }

            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("|{0, -15} | {1, -15} | {2, -6}|", "Förnamn", "Efternamn", "Klass");
            Console.WriteLine(" {0}", new string('-', 42));
            Console.ResetColor();

            foreach (var item in temp)
            {
                Console.WriteLine("|{0, -15} | {1, -15} | {2, -6}|", item.FirstN, item.LastN, item.ClassN);
            }
            Console.WriteLine(" {0}", new string('-', 42));
            Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
            Console.ReadKey();
            MainMenu();
        }

        /// <summary>
        /// Method to pick a class. returns a string of the class name.
        /// </summary>
        /// <returns></returns>
        private string ClassPicker()
        {
            List<string> classesList = new List<string>();
            string chosenClass = "";
            string pickClassPrompt = "Välj vilken klass du vill visa: ";

            var options = from c in schoolDb.Classes select c;

            foreach (Class item in options)
            {
                classesList.Add(item.ClassName);
            }
            classesList.Add("Tillbaka");

            Menu classesMenu = new Menu(pickClassPrompt, classesList);

            int userChoice = classesMenu.PrintMenu();

            if (userChoice == (classesList.Count -1))
            {
                MainMenu();
            }
            
            for (int i = 0; i < classesList.Count; i++)
            {
                if (userChoice == i)
                {
                    chosenClass = classesList[i].ToString();
                }
            }

            return chosenClass;
        }

        /// <summary>
        /// Prints out all of the grades set in past month with student name, course name and grade.
        /// </summary>
        private void GetLatestGrade()
        {
            string sqlCommand = "SELECT Concat(Student.FirstName, ' ', Student.LastName) AS Student_Name, Course.CourseName, " +
                                "Grade.GradeLevel, SetDate FROM RelationShip " +
                                "Join Student ON Student.Id = FK_StudentId " +
                                "JOIN Course ON Course.Id = FK_CourseId " +
                                "JOIN Grade ON Grade.Id = FK_GradeId " +
                                "WHERE SetDate >= CURRENT_TIMESTAMP -30" +
                                "ORDER BY SetDate";

            SqlDataAdapter sqlData = new SqlDataAdapter(sqlCommand, sqlcon);

            DataTable dataTable = new DataTable();

            sqlData.Fill(dataTable);

            Console.Clear();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("| {0, -25} | {1, -22} | {2, -11}| {3, -13} |",
                              "Student", "Kurs", "Betyg", "Datum");
            Console.WriteLine(" {0}", new string('-', 81));
            Console.ResetColor();
            foreach (DataRow dr in dataTable.Rows)
            {
                DateTime temp1 = (DateTime)dr["SetDate"];
                                
                string tempString = DateOnly.FromDateTime(temp1).ToString();

                //Formatted strings as tables
                Console.WriteLine("| {0,-25} | {1,-22} | {2,-6} | {3,-13} |",
                                  dr["Student_Name"], dr["CourseName"], dr["GradeLevel"], tempString);
            }
            Console.WriteLine(" {0}", new string('-', 81));
            Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
            Console.ReadKey();
            MainMenu();
        }

        /// <summary>
        /// Prints the average grade for each course.
        /// </summary>
        private void GetCourseGradeAverageSql()
        {
            string sqlCommand = "SELECT Course.CourseName, AVG(Grade.Id) AS Average, MIN(Grade.Id) AS Highest_Grade, " +
                                "MAX(Grade.Id) AS Lowest_Grade FROM RelationShip " +
                                "Join Grade on Grade.Id = FK_GradeId " +
                                "Join Course on Course.Id = FK_CourseId " +
                                "GROUP BY Course.CourseName "+
                                "ORDER BY Average";
            
            SqlDataAdapter sqlData = new SqlDataAdapter(sqlCommand, sqlcon);

            DataTable dataTable = new DataTable();

            sqlData.Fill(dataTable);

            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0, -15} {1}", " ", "Betygen skrivs ut som tal där 1 = A och 6 = F \n");
            Console.ResetColor();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("|{0, -23} | {1, -15} | {2, -15}| {3, -13} |",
                              "Kurs", "Snittbetyg", "Högsta betyg", "Lägsta betyg");

            Console.WriteLine(" {0}", new string('-', 75));
            Console.ResetColor();
            foreach (DataRow dr in dataTable.Rows)
            {
                //Formatted strings as tables
                Console.WriteLine("|{0, -23} | {1, -15} | {2, -15} | {3, -12} |",
                                  dr["CourseName"], dr["Average"], dr["Highest_Grade"], dr["Lowest_Grade"]);
            }
            Console.WriteLine(" {0}", new string('-', 75));
            Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
            Console.ReadKey();
            MainMenu();
        }

        /// <summary>
        /// Method to add new students into the database using ADO.NET
        /// </summary>
        private void InsertStudentSql()
        {
            Console.Clear();
            List<string> classList = new List<string>();
            int success = 0;
            string firstName = "";
            string lastName = "";
            string ss_Number = "";
            int classChosen = 0;
            string sqlCommand = "INSERT INTO Student(FirstName, LastName, SS_Number, FK_ClassId) ";
            Console.WriteLine("Fyll i följande fält om nya eleven: (skriv \"exit\" för att avbryta)");

            while (true)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Förnamn: ");
                Console.ResetColor();

                firstName = Console.ReadLine();
                if (firstName.ToLower() == "exit")
                {
                    Console.Clear();
                    Console.WriteLine("Återvänder till huvudmenyn...");
                    Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                    Console.ReadKey();
                    MainMenu();
                }
                else if (!string.IsNullOrEmpty(firstName) && firstName.Length <= 30)
                {
                    break;
                }
                
                else
                {
                    Console.WriteLine("Du skrev fel värde. Försök igen!");
                    Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                    Console.ReadKey();
                }
            }
            while (true)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Efternamn: ");
                Console.ResetColor();

                lastName = Console.ReadLine();
                if (lastName.ToLower() == "exit")
                {
                    Console.Clear();
                    Console.WriteLine("Återvänder till huvudmenyn...");
                    Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                    Console.ReadKey();
                    MainMenu();
                }
                else if (!string.IsNullOrEmpty(lastName) && lastName.Length <= 30)
                {
                    break;
                }
                
                else
                {
                    Console.WriteLine("Du skrev fel värde. Försök igen!");
                    Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                    Console.ReadKey();
                }
            }
            while (true)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Personnummer: ");
                Console.ResetColor();

                ss_Number = Console.ReadLine();
                if (ss_Number.ToLower() == "exit")
                {
                    Console.Clear();
                    Console.WriteLine("Återvänder till huvudmenyn...");
                    Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                    Console.ReadKey();
                    MainMenu();
                }

                else if (!string.IsNullOrEmpty(ss_Number) && ss_Number.Length <= 15)
                {
                    break;
                }
                
                else
                {
                    Console.WriteLine("Du skrev fel värde. Återvänder till huvudmenyn...");
                    Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                    Console.ReadKey();
                    MainMenu();
                }
            }

            var classes = from c in schoolDb.Classes select c;
            foreach (var c in classes)
            {
                classList.Add(c.ClassName);
            }
            
            while (true)
            {
                Console.WriteLine("skriv talet på klassen som eleven ska hamna i: ");
                for (int i = 0; i < classList.Count; i++)
                {
                    Console.WriteLine("{0}- {1}", i + 1, classList[i]);
                }

                bool temp = int.TryParse(Console.ReadLine(), out classChosen);
                if (classChosen > 0 && classChosen <= classList.Count)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Du skrev fel värde. Försök igen!");
                }
            }
                        
            sqlCommand += $"VALUES (\'{firstName.Trim()}\', \'{lastName.Trim()}\', \'{ss_Number.Trim()}\', {classChosen})";
            SqlDataAdapter sqlData = new SqlDataAdapter(sqlCommand, sqlcon);
            try
            {
                sqlcon.Open();
                SqlCommand cmd = new SqlCommand(sqlCommand, sqlcon);
                success = cmd.ExecuteNonQuery();
                sqlcon.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }            

            if (success <= 1)
            {
                Console.WriteLine("Studenten har lagts till i databasen!");
                Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                Console.ReadKey();
                MainMenu();
            }

            else
            {
                Console.WriteLine("Inga nya studenter har lagts till.");
                Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Method to add new employees with a menu to pick the title. using Entity Framework
        /// </summary>
        private void InsertEmployeeEntity()
        {
            Console.Clear();
            bool success = false;
            List<string> titleOption = new List<string>();
            string firstName = "";
            string lastName = "";
            int chosenTitle = 0;
            string pickTitlePrompt = "Välj vilken titel nya personalen ska få: ";
            var titles = from c in schoolDb.Titles select c;

            foreach (Title title in titles)
            {
                titleOption.Add(title.TitleName);
            }
            titleOption.Add("Tillbaka");

            Console.WriteLine("Fyll i följande fält för nya personalen: (skriv \"exit\" för att avbryta) ");
            while (true)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Förnamn: ");
                Console.ResetColor();

                firstName = Console.ReadLine().Trim();
                if (firstName.ToLower() == "exit")
                {
                    Console.Clear();
                    Console.WriteLine("Återvänder till huvudmenyn...");
                    Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                    Console.ReadKey();
                    MainMenu();
                }
                else if (!string.IsNullOrEmpty(firstName) && firstName.Length <= 30)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Du skrev fel värde. Försök igen!");
                    Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                    Console.ReadKey();
                }
                
            }

            while (true)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Efternamn: ");
                Console.ResetColor();

                lastName = Console.ReadLine().Trim();
                if (lastName.ToLower() == "exit")
                {
                    Console.Clear();
                    Console.WriteLine("Återvänder till huvudmenyn...");
                    Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                    Console.ReadKey();
                    MainMenu();
                }
                else if (!string.IsNullOrEmpty(lastName) && lastName.Length <= 30)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Du skrev fel värde. Försök igen!");
                    Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                    Console.ReadKey();
                }
            }

            Menu classesMenu = new Menu(pickTitlePrompt, titleOption);
            chosenTitle = classesMenu.PrintMenu();

            if (chosenTitle == (titleOption.Count - 1))
            {
                InsertEmployeeEntity();
            }

            Employee emp = new Employee();
            emp.FirstName = firstName;
            emp.LastName = lastName;
            emp.FkTitleId = chosenTitle+1;
            try
            {
                schoolDb.Employees.Add(emp);
                schoolDb.SaveChanges();
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (success == true)
            {
                Console.WriteLine("En ny personal har skapats!");
                Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Något gick fel. Försök igen senare.");
                Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                Console.ReadKey();
            }
            MainMenu();
        }

        /// <summary>
        /// Method that exits the applikation with code 0.
        /// </summary>
        private void Exit()
        {
            Console.Clear();
            Console.WriteLine("Avslutar applikationen...");
            Thread.Sleep(2000);
            Environment.Exit(0);
        }
    }
}