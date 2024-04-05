using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("EMPLOYEE_QUALIFICATION")]
    public class EmployeeQualification : BaseModel
    {
        [PrimaryKey("qualification_id", true)]
        public Guid QualificationId { get; set; }

        [PrimaryKey("employee_id", true)]
        public Guid EmployeeId { get; set; }
    }
}
