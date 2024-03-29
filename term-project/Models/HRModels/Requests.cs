using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("REQUESTS")]
    public class Request : BaseModel
    {
        [PrimaryKey("request_id")]
        public Guid RequestId { get; set; }

        [Column("employee_id")]
        public Guid EmployeeId { get; set; }

        [Column("request_type")]
        public string RequestType { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("status")]
        public string Status { get; set; }
    }
}
