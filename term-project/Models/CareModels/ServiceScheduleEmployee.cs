using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("SERVICE_SCHEDULE_EMPLOYEE")]
    public class ServiceScheduleEmployee : BaseModel
    {
        [PrimaryKey("service_schedule_employee_id", false)]
        public Guid ServiceScheduleEmployeeId { get; set; }

        [Column("service_schedule_id")]
        public Guid ServiceScheduleId { get; set; }

        [Column("employee_id")]
        public Guid EmployeeId { get; set; }
    }
}
