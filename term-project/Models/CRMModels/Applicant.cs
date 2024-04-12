using Postgrest.Attributes;
using Postgrest.Models;

namespace term_project.Models.CRMModels
{
  [Table("APPLICANT")]
  public class Applicant : BaseModel
  {
    [PrimaryKey("applicant_id", false)]
    public Guid ApplicantId { get; set; }

    [Column("first_name")]
    public string FirstName { get; set; }

    [Column("last_name")]
    public string LastName { get; set; }

    [Column("current_employer")]
    public string CurrentEmployer { get; set; }

    [Column("income")]
    public double Income { get; set; }

    [Column("reference_info")]
    public string ReferenceInfo { get; set; }

    [Column("sharing_people_info")]
    public string SharingPeopleInfo { get; set; }

    [Column("status")]
    public string Status { get; set; }

    [Column("email")]
    public string Email { get; set; }
    }
}