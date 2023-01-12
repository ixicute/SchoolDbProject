using System;
using System.Collections.Generic;

namespace SchoolDbProject.Models
{
    public partial class Course
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = null!;
        public int FkEmployeeId { get; set; }
        public string? ActiveStatus { get; set; }

        public virtual Employee FkEmployee { get; set; } = null!;
    }
}
