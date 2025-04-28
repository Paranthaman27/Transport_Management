using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Transport_Management.Models.Entity
{
    [Table("mstCompany")]
    public class mstCompany
    {
        [Key]
        public int mstCompanyId { get; set; }
        public int? companyId { get; set; }

        [Required(AllowEmptyStrings = false), MaxLength(100)]
        public string companyGSTNo { get; set; }
        [Required(AllowEmptyStrings = false), MaxLength(100)]
        public string companyName { get; set; }
        public string? addressLine1 { get; set; }
        public string? addressLine2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public int? pinCode  { get; set; }
        public int? createdBy { get; set; }
        public DateTime createdDate { get; set; } = DateTime.Now;
        public int? updatedBy { get; set; }
        public DateTime? updatedDate { get; set; }
        public int? deletdBy { get; set; }
        public DateTime? deletedDate { get; set; }
        public bool isActive { get; set; } = true;
        public bool isGST { get; set; }=true;
    }
}
