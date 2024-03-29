using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("SHIFT")]
    public class Shift : BaseModel
    {
        [PrimaryKey("shift_id", false)]
        public Guid ShiftId { get; set; }

        [Column("shift_type")]
        public string ShiftType { get; set; }

        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime EndTime { get; set; }
    }
}
