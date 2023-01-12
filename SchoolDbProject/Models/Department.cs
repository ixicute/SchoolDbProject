using System;
using System.Collections.Generic;

namespace SchoolDbProject.Models
{
    public partial class Department
    {
        public Department()
        {
            Employees = new HashSet<Employee>();
        }

        public int Id { get; set; }
        public string DepartmentName { get; set; } = null!;

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
