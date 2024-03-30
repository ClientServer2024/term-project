using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("EMPLOYEE")]
    public class Employee : BaseModel
    {
        [PrimaryKey("employee_id", false)]
        public Guid EmployeeId { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("address")]
        public string Address { get; set; }

        [Column("emergency_contact")]
        public string EmergencyContact { get; set; }

        [Column("job_title")]
        public string JobTitle { get; set; }

        [Column("employment_type")]
        public string EmploymentType { get; set; }

        [Column("salary_rate")]
        public float? SalaryRate { get; set; }

        [Column("manager_id")]
        public long? ManagerId { get; set; }
    }
}
