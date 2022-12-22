using System;
using System.Collections.Generic;

namespace SchoolDbProject.Models
{
    public partial class Grade
    {
        public int Id { get; set; }
        public string GradeLevel { get; set; } = null!;
    }
}
