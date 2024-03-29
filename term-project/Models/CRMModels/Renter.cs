using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CareModels
{
    [Table("RENTER")]
    public class Renter : BaseModel
    {
        [PrimaryKey("renter_id", false)]
        public Guid RenterId { get; set; }

        [Column("applicant_id")]
        public Guid ApplicantId { get; set; }

        [Column("emergency_contacts")]
        public string EmergencyContacts { get; set; }

        [Column("family_doctor")]
        public string FamilyDoctor { get; set; }

        [Column("status")]
        public string Status { get; set; }
    }
}
