using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("SERVICE_REGISTER")]
    public class ServiceRegister : BaseModel
    {
        [PrimaryKey("service_register_id", false)]
        public Guid ServiceRegisterId { get; set; }

        [Column("service_schedule_employee_id")]
        public Guid ServiceScheduleEmployeeId  { get; set; }
        
        [Column("renter_id")]
        public Guid RenterId { get; set; }
    }
}
