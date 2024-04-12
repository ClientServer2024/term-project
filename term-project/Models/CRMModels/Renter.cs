using Postgrest.Attributes;
using Postgrest.Models;
using System.Text.Json.Serialization;

namespace term_project.Models.CRMModels
{
    [Table("RENTER")]
    public class Renter : BaseModel
    {
        [PrimaryKey("renter_id", false)]
        [JsonIgnore] // Ignore this property during JSON serialization
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
