using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("SERVICE")]
    public class Service : BaseModel
    {
        [PrimaryKey("service_id", false)]
        public Guid ServiceId { get; set; }

        [Column("service_name")]
        public string ServiceName { get; set; }

        [Column("rate")]
        public float? Rate { get; set; }
    }
}
