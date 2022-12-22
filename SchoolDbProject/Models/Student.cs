using System;
using System.Collections.Generic;

namespace SchoolDbProject.Models
{
    public partial class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string SsNumber { get; set; } = null!;
        public int FkClassId { get; set; }

        public virtual Class FkClass { get; set; } = null!;
    }
}
