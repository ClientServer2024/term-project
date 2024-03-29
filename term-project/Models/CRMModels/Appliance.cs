using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("APPLIANCE")]
    public class Appliance : BaseModel
    {
        [PrimaryKey("appliance_id", false)]
        public Guid ApplianceId { get; set; }

        [Column("asset_id")]
        public Guid AssetId { get; set; }

        [Column("make")]
        public string Make { get; set; }

        [Column("model")]
        public string Model { get; set; }
    }
}
