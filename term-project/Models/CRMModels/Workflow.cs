using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("WORKFLOW")]
    public class Workflow : BaseModel
    {
        [PrimaryKey("workflow_id", false)]
        public Guid WorkflowId { get; set; }

        [Column("maintenance_request_id")]
        public Guid MaintenanceRequestId { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("assigned_to")]
        public Guid AssignedTo { get; set; }
    }
}
