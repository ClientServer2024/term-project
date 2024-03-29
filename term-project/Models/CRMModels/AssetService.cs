using Postgrest.Attributes;
using Postgrest.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace term_project.Models.CRMModels
{
  [Table("ASSET_SERVICE")]
  public class AssetService : BaseModel
  {
    [PrimaryKey("asset_service_id")]
    public Guid AssetServiceId { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("rate")]
    public float Rate { get; set; }
  }
}
