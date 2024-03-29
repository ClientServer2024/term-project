using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("INVOICE")]
    public class Invoice : BaseModel
    {
        [PrimaryKey("invoice_id", false)]
        public Guid InvoiceId { get; set; }

        [Column("renter_id")]
        public Guid RenterId { get; set; }

        [Column("amount_due")]
        public float AmountDue { get; set; }

        [Column("services_charges")]
        public float ServicesCharges { get; set; }

        [Column("taxes")]
        public float Taxes { get; set; }

        [Column("due_date")]
        public DateTime DueDate { get; set; }

        [Column("service_schedule_id")]
        public Guid ServiceScheduleId { get; set; }

        [Column("invoice_type")]
        public string InvoiceType { get; set; }

        [Column("asset_service_id")]
        public Guid AssetServiceId { get; set; }

        [Column("asset_id")]
        public Guid AssetId { get; set; }
    }
}
