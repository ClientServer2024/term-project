using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace term_project.Models.CareModels
{
    [Table("PAY_HISTORY")]
    public class PayHistory : BaseModel
    {
        [PrimaryKey("pay_history_id", false)]
        public Guid PayHistoryId { get; set; }

        [Column("employee_id")]
        public Guid EmployeeId { get; set; }

        [Column("pay_raise_date")]
        public DateTime PayRaiseDate { get; set; }

        [Column("previous_salary_rate")]
        public float PreviousSalaryRate { get; set; }

        [Column("new_salary_rate")]
        public float NewSalaryRate { get; set; }
    }
}
