using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("PAYROLL")]
    public class Payroll : BaseModel
    {
        [PrimaryKey("payroll_id", false)]
        public Guid PayrollId { get; set; }

        [Column("employee_id")]
        public Guid EmployeeId { get; set; }

        [Column("pay_period_start")]
        public DateTime PayPeriodStart { get; set; }

        [Column("pay_period_end")]
        public DateTime PayPeriodEnd { get; set; }

        [Column("gross_pay")]
        public float? GrossPay { get; set; }

        [Column("deductions")]
        public float? Deductions { get; set; }

        [Column("net_pay")]
        public float? NetPay { get; set; }

        [Column("tax_rate")]
        public float? TaxRate { get; set; }
    }
}
