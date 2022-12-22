using System;
using System.Collections.Generic;

namespace SchoolDbProject.Models
{
    public partial class RelationShip
    {
        public int? FkStudentId { get; set; }
        public int? FkCourseId { get; set; }
        public int? FkGradeId { get; set; }
        public DateTime? SetDate { get; set; }
        public int? FkGradedByTeacherId { get; set; }

        public virtual Course? FkCourse { get; set; }
        public virtual Grade? FkGrade { get; set; }
        public virtual Employee? FkGradedByTeacher { get; set; }
        public virtual Student? FkStudent { get; set; }
    }
}
