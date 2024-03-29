using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("RENT_HISTORY")]
    public class RentHistory : BaseModel
    {
        [PrimaryKey("rent_history_id", false)]
        public Guid RentHistoryId { get; set; }

        [Column("asset_id")]
        public Guid AssetId { get; set; }

        [Column("amount")]
        public double Amount { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }
    }
}
