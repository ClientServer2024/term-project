using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CRMModels
{
    [Table("OCCUPANCY_HISTORY")]
    public class OccupancyHistory : BaseModel
    {
        [PrimaryKey("occupancy_history_id", false)]
        public Guid OccupancyHistoryId { get; set; }

        [Column("asset_id")]
        public Guid AssetId { get; set; }

        [Column("renter_id")]
        public Guid RenterId { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("vehicle_associated")]
        public string VehicleAssociated { get; set; }
    }
}
