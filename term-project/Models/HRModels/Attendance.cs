using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("ATTENDANCE")]
    public class Attendance : BaseModel
    {
        [PrimaryKey("attendance_id")]
        public Guid AttendanceId { get; set; }

        [Column("employee_id")]
        public Guid EmployeeId { get; set; }

        [Column("clock_in_time")]
        public DateTime ClockInTime { get; set; }

        [Column("clock_out_time")]
        public DateTime ClockOutTime { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("status")]
        public string Status { get; set; }

    }
}
