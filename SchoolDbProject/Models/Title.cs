using System;
using System.Collections.Generic;

namespace SchoolDbProject.Models
{
    public partial class Title
    {
        public Title()
        {
            Employees = new HashSet<Employee>();
        }

        public int Id { get; set; }
        public string TitleName { get; set; } = null!;
        public decimal? Salary { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
