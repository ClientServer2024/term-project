using Postgrest.Attributes;
using Postgrest.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace term_project.Models.CRMModels
{
  [Table("APPLIANCE")]
  public class Appliance : BaseModel
  {
    [PrimaryKey("appliance_id")]
    public Guid ApplianceId { get; set; }

    [Column("asset_id")]
    public Guid AssetId { get; set; }

    [Column("make")]
    public string Make { get; set; }

    [Column("model")]
    public string Model { get; set; }
  }
}
