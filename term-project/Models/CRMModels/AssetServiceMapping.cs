using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CRMModels
{
    [Table("ASSET_SERVICE_MAPPING")]
    public class AssetServiceMapping : BaseModel
    {
        [PrimaryKey("asset_service_id", false)]
        public Guid AssetServiceId { get; set; }

        [Column("asset_id")]
        public Guid AssetId { get; set; }
    }
}
