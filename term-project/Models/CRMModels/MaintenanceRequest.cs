using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CRMModels
{
    [Table("MAINTENANCE_REQUEST")]
    public class MaintenanceRequest : BaseModel
    {
        [PrimaryKey("maintenance_request_id", false)]
        public Guid MaintenanceRequestId { get; set; }

        [Column("asset_id")]
        public Guid AssetId { get; set; }

        [Column("renter_id")]
        public Guid RenterId { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("due_date")]
        public DateTime DueDate { get; set; }
    }
}
