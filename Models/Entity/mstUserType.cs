using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Transport_Management.Models.Entity
{
    [Table("mstUserType")]
    public class mstUserType
    {
        [Key]
        public int userTypeId { get; set; }
        public string userRole { get; set; }
        public int? createdBy { get; set; }
        public DateTime createdDate { get; set; } = DateTime.Now;
        public int? updatedBy { get; set; }
        public DateTime? updatedDate { get; set; }
        public int? deletdBy { get; set; }
        public DateTime? deletedDate { get; set; }
        public bool isActive { get; set; } = true;
        public bool isDefault { get; set; }
    }
}
