using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("SERVICE_QUALIFICATION")]
    public class ServiceQualification: BaseModel
    {
        [PrimaryKey("service_id", true)]
        public Guid ServiceId { get; set; }
        
        [PrimaryKey("qualification_id", true)]
        public Guid QualificationId { get; set; }
    }
}
