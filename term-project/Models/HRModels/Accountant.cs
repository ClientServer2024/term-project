using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("ACCOUNTANT")]
    public class Accountant : BaseModel
    {
        [PrimaryKey("accountant_id")]
        public Guid AccountantId { get; set; }

        [Column("employee_id")]
        public Guid EmployeeId { get; set; }

        [Column("certification_number")]
        public string CertificationNumber { get; set; }

        [Column("specialization")]
        public string Specialization { get; set; }

    }
}
