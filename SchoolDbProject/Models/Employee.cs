using System;
using System.Collections.Generic;

namespace SchoolDbProject.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Courses = new HashSet<Course>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int FkTitleId { get; set; }
        public DateTime StartDate { get; set; }
        public decimal? Salary { get; set; }
        public int? FkDepartment { get; set; }
        public string? Password { get; set; }

        public virtual Department? FkDepartmentNavigation { get; set; }
        public virtual Title FkTitle { get; set; } = null!;
        public virtual ICollection<Course> Courses { get; set; }
    }
}
