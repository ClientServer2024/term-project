using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("SERVICE_SCHEDULE")]
    public class ServiceSchedule : BaseModel
    {
        [PrimaryKey("service_schedule_id", false)]
        public Guid ServiceScheduleId { get; set; }

        [Column("service_id")]
        public Guid ServiceId { get; set; }

        [Column("schedule")]
        public DateTimeOffset Schedule { get; set; }
    }
}
