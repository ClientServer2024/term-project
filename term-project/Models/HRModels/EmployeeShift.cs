using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("EMPLOYEE_SHIFT")]
    public class EmployeeShift : BaseModel
    {
        [PrimaryKey("employee_shift_id")]
        public Guid EmployeeShiftId { get; set; }

        [Column("employee_id")]
        public Guid EmployeeId { get; set; }

        [Column("shift_id")]
        public Guid ShiftId { get; set; }
    }
}
