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
using Figgle;
using System.Globalization;

namespace SchoolDbProject.ClassLib
{
    internal class DbAccess
    {
        private User loggedinUser;

        //This is the context used to  communicate with the database.
        private SchoolDbContext schoolDb = new SchoolDbContext();
        
        //Connection string. Edit "data source" to match server name and initial catalog to match database name.
        private SqlConnection sqlcon = new SqlConnection(@"Data Source = ALDOR007; Initial Catalog=SchoolDb; Integrated Security= true");

        //Holds all users added through the InitiateUser method.
        private List<User> userList = new List<User>();

        /// <summary>
        /// Initiate users and adds them to a list of USER.
        /// Id, Username, password and boolean for if admin or not.
        /// </summary>
        public void InitiateUser()
        {
            var loginData =
                (
                    from u in schoolDb.Employees
                    select new
                    {
                        ID = u.Id,
                        Username = u.FirstName + " " + u.LastName,
                        Password = u.Password,
                        isAdmin = u.FkTitleId
                    }
                ).ToList();

            //Janitor (title = 3) won't be added to the user list because they should not have access.
            // Following titles are referenced: 1 = teacher, 2 = principle, 4 = admin
            foreach (var user in loginData)
            {
                if (user.isAdmin <= 2)
                {
                    userList.Add(new User(user.ID, user.Username, user.Password, false));
                }

                else if (user.isAdmin == 4)
                {
                    userList.Add(new Admin(user.ID, user.Username, user.Password, true));
                }
            }
        } //Added for LAB-4

        /// <summary>
        /// Runs Main-Menu of the program after login-check.
        /// </summary>
        public void Run()
        {            
            bool isLoginValid = LogIn();

            if (isLoginValid == true)
            {
                MainMenu();
            }
            
        }

        /// <summary>
        /// Configuration for the menu options. Edit this method to change menu-options.
        /// </summary>
        private void MainMenu()
        {
            Console.Clear();
            //This string will hold the message on the top of the screen.
            string prompt = $"Inloggad som [{loggedinUser.UserName}]\n";

            if (loggedinUser.IsAdmin == true)
            {
                //This string array holds all the menu options. Edit this to change the content of the menu.
                List<string> options = new List<string>()
                {
                    "Sök upp student med ID", //Done
                    "Lägg till ny elev", //Done
                    "Lägg till ny personal", //Done
                    "Visa avdelningsstatus",
                    "Visa avdelningarnas månatlig kostnad",
                    "Visa avdelningarnas snittkostnad",
                    "Visa personal",
                    "Logga ut",
                    "Avsluta"
                };

                Menu mainMenu = new Menu(prompt, options);

                //Runs the method and saves the returned value to be used in the switch-case menu below.
                int selectedIndex = mainMenu.PrintMenu();

                //Menu for selection.
                switch (selectedIndex)
                {
                    case 0:                        
                        ViewStudentByIdEntity();
                        break;
                    case 1:                        
                        InsertStudentSql();
                        break;
                    case 2:                        
                        InsertEmployeeEntity();
                        break;
                    case 3:                        
                        DepartmentsEntity();
                        break;
                    case 4:                        
                        DptTotalMonthlyCostSql();
                        break;
                    case 5:                        
                        DptAverageCostSql();
                        break;
                    case 6:
                        EmployeesSql();
                        break;
                    case 7:
                        Console.Clear();
                        Console.WriteLine("Du loggas ut...");
                        Thread.Sleep(1000);
                        Console.Clear();
                        Run();
                        break;
                    case 8:
                        Exit();
                        break;
                }
            }

            else if(loggedinUser.IsAdmin == false)
            {
                //This string array holds all the menu options. Edit this to change the content of the menu.
                List<string> options = new List<string>()
                {
                    "Hämta alla elever", //Done
                    "Hämta klass-info", //Done
                    "Hämta betyg från senaste månaden",
                    "Visa snittbetyg för alla kurser",
                    "Sätt betyg",
                    "Visa aktiva kurser",
                    "Sök upp student med ID",
                    "Logga ut",
                    "Avsluta"
                };

                Menu mainMenu = new Menu(prompt, options);

                //Runs the method and saves the returned value to be used in the switch-case menu below.
                int selectedIndex = mainMenu.PrintMenu();

                //Menu for selection.
                switch (selectedIndex)
                {
                    case 0:                        
                        ViewStudentsEntity();
                        break;
                    case 1:                        
                        StudentsByClassEntity();
                        break;
                    case 2:                        
                        GetLatestGrade();
                        break;
                    case 3:                        
                        GetCourseGradeAverageSql();
                        break;
                    case 4:                        
                        SetGradeSql();
                        break;
                    case 5:                        
                        ActiveCoursesEntity();
                        break;
                    case 6:                        
                        ViewStudentByIdEntity();
                        break;
                    case 7:
                        Console.Clear();
                        Console.WriteLine("Du loggas ut...");
                        Thread.Sleep(1000);
                        Console.Clear();
                        Run();
                        break;
                    case 8:
                        Exit();
                        break;
                }
            }

            //Program exits in case it somehow skips menus.
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
                "Admin",
                "Tillbaka"
            };

            Menu EmpMenu = new Menu(prompt, options);

            //Menu returns an int based on the chosen option
            int selectedIndex = EmpMenu.PrintMenu();

            string sqlCommand = "SELECT Employee.FirstName, Employee.LastName, Title.TitleName, " +
                                "DATEDIFF(year, StartDate, GETDATE()) AS YearsWorked, Employee.Salary, Employee.StartDate" +
                                " FROM Employee " +
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
                    sqlCommand += "Where Title.TitleName = 'Admin'";
                    break;
                case 5:
                    MainMenu();
                    break;
            }

            SqlDataAdapter sqlData = new SqlDataAdapter(sqlCommand, sqlcon);

            DataTable dataTable = new DataTable();

            sqlData.Fill(dataTable);

            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n\t\t\t\t\tLista över alla anställda\n");
            Console.ResetColor();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("|{0, -15} | {1, -15} | {2, -15} | {3, -9} | {4, -12} | {5, -17}|", 
                              "Förnamn", "Efternamn", "Titel", "Lön", "År I Arbete", "Anställningsdatum");
            Console.WriteLine(" {0}", new string('-', 99));
            Console.ResetColor();

            foreach (DataRow dr in dataTable.Rows)
            {
                //Formatted strings as tables
                Console.WriteLine("|{0, -15} | {1, -15} | {2, -15} | {3, -9} | {4, -12} | {5, -17}|",
                                  dr["FirstName"], dr["LastName"], dr["TitleName"], dr["Salary"], dr["YearsWorked"],
                                  Convert.ToDateTime(dr["StartDate"].ToString()).ToString("yyyy-MM-dd"));
            }

            Console.WriteLine(" {0}", new string('-', 99));
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
            Console.WriteLine("|{0, -4} | {1, -15} | {2, -15} | {3, -15}|","ID", "Förnamn", "Efternamn", "Personnummer");            
            Console.WriteLine(" {0}", new string('-', 58));
            Console.ResetColor();

            foreach (Student student in sorted)
            {
                Console.WriteLine("|{0, -4} | {1, -15} | {2, -15} | {3, -15}|",student.Id, student.FirstName, student.LastName, student.SsNumber);
            }
            Console.WriteLine(" {0}", new string('-', 58));
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
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("skriv talet på klassen som eleven ska hamna i: ");
                Console.ResetColor();

                for (int i = 0; i < classList.Count; i++)
                {
                    Console.WriteLine("[{0}]. {1}", i + 1, classList[i]);
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
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Studenten har lagts till i databasen!");
                Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                Console.ReadKey();
                MainMenu();
            }

            else
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Inga nya studenter har lagts till.");
                Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                Console.ReadKey();
                MainMenu();
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
            List<string> deptOptions = new List<string>();
            string pickTitlePrompt = "Välj vilken titel nya personalen ska få: ";
            string pickDeptPrompt = "Välj avdelning för personalen att jobba i: ";
            var titles = from c in schoolDb.Titles select c;
            var depts = from d in schoolDb.Departments select d;
            int chosenTitle = 0;
            int chosenDept = 0;
            string firstName = "";
            string lastName = "";
            decimal salary = 0M;            

            foreach (Title title in titles)
            {
                titleOption.Add(title.TitleName);
            }
            titleOption.Add("Avbryt");

            foreach (Department dept in depts)
            {
                deptOptions.Add(dept.DepartmentName);
            }

            deptOptions.Add("Avbryt");

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Fyll i följande fält för nya personalen: (skriv \"exit\" för att avbryta) ");
            Console.ResetColor();

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
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Återvänder till huvudmenyn...");
                    Console.ResetColor();
                    Thread.Sleep(1000);
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

            while (true)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Lön: (t.ex 25000) ");
                Console.ResetColor();

                decimal.TryParse(Console.ReadLine(), out salary);
                
                if (salary >= 0)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Du skrev fel värde. Försök igen!");                    
                }
            }

            Menu titlesMenu = new Menu(pickTitlePrompt, titleOption);
            chosenTitle = titlesMenu.PrintMenu();

            if (chosenTitle == (titleOption.Count - 1))
            {
                MainMenu();
            }

            Menu deptMenu = new Menu(pickDeptPrompt, deptOptions);
            chosenDept = deptMenu.PrintMenu();

            if (chosenDept == (deptOptions.Count - 1))
            {
                MainMenu();
            }

            Employee emp = new Employee();
            emp.FirstName = firstName;
            emp.LastName = lastName;
            emp.FkTitleId = chosenTitle+1;
            emp.Salary = salary;
            emp.FkDepartment = chosenDept+1;
            emp.StartDate = DateTime.Now;
            

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
        } //Updated LAB-4

        /// <summary>
        /// Sets the grade of a student
        /// </summary>
        public void SetGradeSql()
        {
            int studentId = 0;
            int courseId = 0;
            int gradeId = 0;
            int success = 0;
            List<int> idList = new List<int>();

            string sqlStudents = "SELECT Student.Id, Student.FirstName, Student.LastName, Student.SS_Number FROM Student ";

            string sqlNoGradeCourses = "SELECT Course.Id, Course.CourseName FROM RelationShip " +
                                       "JOIN Course ON FK_CourseId = Course.Id " +
                                       "JOIN Student ON FK_StudentId = Student.Id ";

            string sqlGrade = "SELECT Grade.Id, Grade.GradeLevel FROM Grade";

            //Getting student data to choose student based on ID - START
            SqlDataAdapter sqlData = new SqlDataAdapter(sqlStudents, sqlcon);

            DataTable dataTable = new DataTable();

            sqlData.Fill(dataTable);

            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n\t\tLista över alla studenter\n");
            Console.ResetColor();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("|{0, -4} | {1, -15} | {2, -15}| {3, -15} |",
                              "ID", "Förnamn", "Efternamn", "Personnummer");

            Console.WriteLine(" {0}", new string('-', 60));
            Console.ResetColor();
            foreach (DataRow dr in dataTable.Rows)
            {
                //Formatted strings as tables
                Console.WriteLine("|{0, -4} | {1, -15} | {2, -15} | {3, -15} |",
                                  dr["Id"], dr["FirstName"], dr["LastName"], dr["SS_Number"]);                
            }
            Console.WriteLine(" {0}", new string('-', 60));

            while (true)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Välj student ID: ");
                Console.ResetColor();

                int.TryParse(Console.ReadLine(), out studentId);

                if (studentId > 0 && studentId <= dataTable.Rows.Count)
                {
                    sqlNoGradeCourses += $"WHERE Student.Id = {studentId} AND FK_GradeId IS NULL";
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Du skrev fel värde. Försök igen!");
                    Console.ResetColor();
                }
            }
            //Getting student data to choose student based on ID - END


            //Getting Courses data where student has no grades yet - START
            SqlDataAdapter sqlCourses = new SqlDataAdapter(sqlNoGradeCourses, sqlcon);            

            DataTable noGradeCourses = new DataTable();

            sqlCourses.Fill(noGradeCourses);

            //If any courses with null value is found for the specified student
            if (noGradeCourses.Rows.Count != 0)
            {
                while (true)
                {
                    Console.Clear();
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n\t\tFöljande kurser saknar betyg:\n");
                    Console.ResetColor();

                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("|{0, -5} | {1, -25}|",
                                      "ID", "Course Name");

                    Console.WriteLine(" {0}", new string('-', 60));
                    Console.ResetColor();

                    idList.Clear();

                    foreach (DataRow dr in noGradeCourses.Rows)
                    {                        
                        string temp = dr["Id"].ToString();
                        idList.Add(int.Parse(temp));

                        Console.WriteLine("|{0, -5} | {1, -25}|",
                                          dr["Id"], dr["CourseName"]);
                    }
                    Console.WriteLine(" {0}", new string('-', 60));

                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Välj kursens ID: ");
                    Console.ResetColor();

                    int.TryParse(Console.ReadLine(), out courseId);

                    if (courseId > 0 && idList.Contains(courseId))
                    {
                        break;
                    }
                    else
                    {
                        Console.Clear();
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Du skrev fel värde. Försök igen!");
                        Console.ResetColor();
                        Console.ReadKey();
                    }
                }
            }

            else
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Studenten har inga pågående kurser som saknar betyg.");
                Console.ResetColor();
                Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                Console.ReadKey();
                MainMenu();
            }
            //Getting Courses data where student has no grades yet - END


            //Pick grade based on grade id - START
            SqlDataAdapter gradeChoice = new SqlDataAdapter(sqlGrade, sqlcon);

            DataTable gradesTable = new DataTable();

            gradeChoice.Fill(gradesTable);

            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n\t\tBetygssystem 2023:\n");
            Console.ResetColor();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("|{0, -4} | {1, -5} |",
                              "ID", "Grade");

            Console.WriteLine(" {0}", new string('-', 13));
            Console.ResetColor();
            foreach (DataRow dr in gradesTable.Rows)
            {
                //Formatted strings as tables
                Console.WriteLine("|{0, -4} | {1, -5} |", dr["Id"], dr["GradeLevel"].ToString().Trim());
            }
            Console.WriteLine(" {0}", new string('-', 13));

            while (true)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Välj betyg ID som du vill sätta: ");
                Console.ResetColor();

                int.TryParse(Console.ReadLine(), out gradeId);

                if (gradeId > 0 && gradeId <= gradesTable.Rows.Count)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Du skrev fel värde. Försök igen!");
                }
            }
            //Pick grade based on grade id - END

            string updatecmd = "UPDATE RelationShip " +
                               $"SET FK_GradeId = {gradeId}, SetDate = GETDATE(), FK_GradedByTeacherId = {1} " +
                               $"WHERE FK_StudentId = {studentId} AND FK_CourseId = {courseId}";

            try
            {
                sqlcon.Open();
                SqlCommand cmd = new SqlCommand(updatecmd, sqlcon);
                success = cmd.ExecuteNonQuery();
                sqlcon.Close();                
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (success >= 1)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\t\tBetyget är nu satt!\n");
                Console.ResetColor();
                Thread.Sleep(2000);
            }
            else if (success == 0)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\t\tMisslyckades.. Försök igen senare.\n");
                Console.ResetColor();
                Thread.Sleep(2000);
            }
        } //Added for Lab-4 (Transaction in code)

        /// <summary>
        /// Shows all departments available in the database.
        /// </summary>
        public void DepartmentsEntity()
        {
            var dep = 
                (
                    from d in schoolDb.Departments
                    select new
                    {
                        id = d.Id,
                        name = d.DepartmentName
                    }
                ).ToList();

            var em = 
                (
                   from e in schoolDb.Employees
                   select new
                   {
                        dep = e.FkDepartment,
                        id = e.Id
                   }
                ).ToList();

            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nLista över alla avdelningar och antal anställda\n");
            Console.ResetColor();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("|{0, -10} | {1, -15}|", "Avdelning", "Anställda");
            Console.WriteLine(" {0}", new string('-', 28));
            Console.ResetColor();

            foreach (var dept in dep)
            {
                Console.WriteLine("|{0, -10} | {1, -15}|", dept.name.Trim(), em.Count(x => x.dep == dept.id));
            }
            Console.WriteLine(" {0}", new string('-', 28));

            Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
            Console.ReadKey();
            MainMenu();
        } //Added for Lab-4

        /// <summary>
        /// Shows all courses set as "active" in the database.
        /// </summary>
        public void ActiveCoursesEntity()
        {
            var status = from c in schoolDb.Courses
                         where c.ActiveStatus == "Active"
                         select c;

            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n    Lista över alla aktiva kurser\n");
            Console.ResetColor();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("|{0, -23} | {1, -10}|",
                              "Course", "Status");

            Console.WriteLine(" {0}", new string('-', 36));
            Console.ResetColor();
            foreach (var course in status)
            {
                //Formatted strings as tables
                Console.WriteLine("|{0, -23} | {1, -10}|",
                                  course.CourseName, course.ActiveStatus.Trim());
            }
            Console.WriteLine(" {0}", new string('-', 36));

            Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
            Console.ReadKey();
            MainMenu();
        } //Added for Lab-4

        /// <summary>
        /// Shows the montly cost for each department in the database.
        /// </summary>
        public void DptTotalMonthlyCostSql()
        {
            string sqlCommand = "SELECT Department.Department_Name as \'Department Name\', SUM(Employee.Salary) as \'Monthly Cost\' FROM Employee " +
                                "JOIN Department ON FK_Department = Department.Id " +
                                "GROUP BY Department_Name";

            SqlDataAdapter sqlData = new SqlDataAdapter(sqlCommand, sqlcon);

            DataTable deptMonthCost = new DataTable();

            sqlData.Fill(deptMonthCost);

            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nLista över månatliga kostnader för avdelningar\n");
            Console.ResetColor();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("|{0, -12} | {1, -17} |",
                              "Avdelning", "Månatlig kostnad");

            Console.WriteLine(" {0}", new string('-', 33));
            Console.ResetColor();
            foreach (DataRow dr in deptMonthCost.Rows)
            {
                //Formatted strings as tables
                Console.WriteLine("|{0, -12} | {1, -17} |",
                                  dr["Department Name"].ToString().Trim(), dr["Monthly Cost"].ToString().Trim());
            }
            Console.WriteLine(" {0}", new string('-', 33));

            Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
            Console.ReadKey();
            MainMenu();
        } //Added for Lab-4

        /// <summary>
        /// Shows the average monthly cost of each department in the database.
        /// </summary>
        public void DptAverageCostSql()
        {
            string sqlCommand = "SELECT Department.Department_Name as \'Department Name\', AVG(Employee.Salary) as \'Salary Average\' FROM Employee " +
                                "JOIN Department ON FK_Department = Department.Id " +
                                "GROUP BY Department_Name";

            SqlDataAdapter sqlData = new SqlDataAdapter(sqlCommand, sqlcon);

            DataTable deptAvgCost = new DataTable();

            sqlData.Fill(deptAvgCost);            

            Console.Clear();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nLista över snittkostnader för avdelningar\n");
            Console.ResetColor();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("|{0, -12} | {1, -17} |",
                              "Avdelning", "snittkostnad");

            Console.WriteLine(" {0}", new string('-', 33));
            Console.ResetColor();
            foreach (DataRow dr in deptAvgCost.Rows)
            {
                //Formatted strings as tables
                Console.WriteLine("|{0, -12} | {1, -17} |",
                                  dr["Department Name"].ToString().Trim(), dr["Salary Average"].ToString().Trim());
            }
            Console.WriteLine(" {0}", new string('-', 33));

            Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
            Console.ReadKey();
            MainMenu();

        } //Added for Lab-4

        /// <summary>
        /// Shows a list of available students and based on ID retrives relevant data.
        /// </summary>
        public void ViewStudentByIdEntity()
        {
            int studentId = 0;
            var studs = from s in schoolDb.Students select s;
            string studName = "";
            string className = "";
            List<int> studId = new List<int>();
            DateTime datum;

            while (true)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\tLista över alla elever\n");
                Console.ResetColor();

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("|{0, -4} | {1, -15} | {2, -15}|", "ID", "Förnamn", "Efternamn");
                Console.WriteLine(" {0}", new string('-', 40));
                Console.ResetColor();

                studId.Clear();

                foreach (Student student in studs)
                {
                    studId.Add(student.Id);
                    Console.WriteLine("|{0, -4} | {1, -15} | {2, -15}|", student.Id, student.FirstName, student.LastName);
                }

                Console.WriteLine(" {0}", new string('-', 40));

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nSkriv studentens ID: ");
                Console.ResetColor();

                int.TryParse(Console.ReadLine(), out studentId); //check if student id exists

                if (studId.Contains(studentId))
                {
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nFel, försök igen...");
                    Console.ResetColor();
                    Console.WriteLine("Tryck på tangentbordet för att fortsätta");
                    Console.ReadKey();
                }
            }


            string sqlCommand = $"EXEC SP_GetStudent {studentId}";

            SqlDataAdapter sqlData = new SqlDataAdapter(sqlCommand, sqlcon);

            DataTable studentInfo = new DataTable();
            
            sqlData.Fill(studentInfo);

            foreach (DataRow dr in studentInfo.Rows)
            {
                studName = dr["Student"].ToString();
                className = dr["Class"].ToString();                
            }

            if (!string.IsNullOrEmpty(studName) || !string.IsNullOrEmpty(className))
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n\t\t\tVisar: [{studName}] - Class: [{className}]\n");
                Console.ResetColor();

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("|{0, -25} | {1, -8} | {2, -15} | {3, -23}|", "Kurs", "Betyg", "Datum", "Ansvarig Lärare");
                Console.WriteLine(" {0}", new string('-', 80));
                Console.ResetColor();

                foreach (DataRow dr in studentInfo.Rows)
                {
                    datum = (DateTime)dr["Datum"];

                    Console.WriteLine("|{0, -25} | {1, -8} | {2, -15} | {3, -23}|",
                                      dr["Course"].ToString().Trim(), dr["Grade"].ToString().Trim(), datum.ToString("yyyyMMdd"), dr["Teacher"].ToString().Trim());
                }

                Console.WriteLine(" {0}", new string('-', 80));
                Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                Console.ReadKey();
                MainMenu();
            }

            else
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Vald elev saknar något data och kan inte visas. Kontakta en Admin.");
                Console.ResetColor();
                Console.WriteLine("Tryck på tangentbordet för att fortsätta...");
                Console.ReadKey();
                MainMenu();
                
            }
        } //Added for Lab-4

        /// <summary>
        /// Login screen that checks in the list of initiated users.
        /// </summary>
        /// <returns></returns>
        public bool LogIn()
        {
            int attemptsCheck = 0;
            bool isValid = false;            

            do
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(FiggleFonts.Kban.Render("Skolan Databas"));
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("Världens bästa Applikation \nLogga in nedan! \n");
                Console.ResetColor();

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Skriv in användarnamn: ");
                Console.ResetColor();

                string uname = Console.ReadLine().ToLower();
                

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Skriv in lösenord: ");
                Console.ResetColor();

                string pass = Console.ReadLine();

                //Checks user list for matching entries to the user inputs.
                User temp = userList.Find(x => x.UserName.ToLower() == uname && x.Password == pass);

                if (userList.Contains(temp))
                {
                    loggedinUser = temp;
                    isValid = true;

                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Du loggas in...");
                    Console.ResetColor();
                    Thread.Sleep(800);

                    break;
                }

                else
                {
                    Console.Clear();
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Fel Användarnamn eller Lösenord.\n" +
                                      "Försök igen...");
                    Console.ResetColor();
                    Thread.Sleep(1000);
                    Console.Clear();

                    attemptsCheck++;
                }

            } while (attemptsCheck < 3);
            
            //If more than 3 attempts, application exits.
            if (attemptsCheck >= 3)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nDu har skrivit fel information för många gånger.\n" +
                                  "Programmet stängs.");
                Console.ResetColor();

                Environment.Exit(0);
            }

            return isValid;
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