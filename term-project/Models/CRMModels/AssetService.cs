using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CRMModels
{
    [Table("ASSET_SERVICE")]
    public class AssetService : BaseModel
    {
        [PrimaryKey("asset_service_id", false)]
        public Guid AssetServiceId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("rate")]
        public float? Rate { get; set; }
    }
}
