using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CRMModels
{
    [Table("ASSET")]
    public class Asset : BaseModel
    {
        [PrimaryKey("asset_id", false)]
        public Guid AssetId { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("rent_history_id")]
        public Guid RentHistoryId { get; set; }

        [Column("application_count")]
        public long? ApplicationCount { get; set; }
    }
}
