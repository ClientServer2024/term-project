using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("EMPLOYEE_QUALIFICATION")]
    public class EmployeeQualification : BaseModel
    {
        [PrimaryKey("employee_qualification_id", false)]
        public Guid EmployeeQualificationId { get; set; }

        [Column("qualification_id")]
        public Guid QualificationId { get; set; }

        [Column("employee_id")]
        public Guid EmployeeId { get; set; }
    }
}
