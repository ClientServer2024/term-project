using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.HRModels
{
    [Table("MANAGER")]
    public class Manager : BaseModel
    {
        [PrimaryKey("manager_id")]
        public Guid ManagerId { get; set; }

        [Column("employee_id")]
        public Guid EmployeeId { get; set; }

        [Column("manager_type")]
        public string ManagerType { get; set; }

    }
}
