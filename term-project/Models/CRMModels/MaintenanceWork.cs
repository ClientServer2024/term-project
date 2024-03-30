using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CRMModels
{
    [Table("MAINTENANCE_WORK")]
    public class MaintenanceWork : BaseModel
    {
        [PrimaryKey("work_id", false)]
        public Guid WorkId { get; set; }

        [Column("maintenance_request_id")]
        public Guid MaintenanceRequestId { get; set; }

        [Column("employee_id")]
        public Guid EmployeeId { get; set; }

        [Column("report")]
        public string Report { get; set; }
    }
}
