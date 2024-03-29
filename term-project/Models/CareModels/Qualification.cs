using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("QUALIFICATION")]
    public class Qualification : BaseModel
    {
        [PrimaryKey("qualification_id", false)]
        public Guid QualificationId { get; set; }

        [Column("qualification_name")]
        public string QualificationName { get; set; }
    }
}
