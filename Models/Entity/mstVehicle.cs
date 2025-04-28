using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Transport_Management.Models.Entity
{
    [Table("mstVehicle")]
    public class mstVehicle
    {
        [Key]
        public int mstVehicleId { get; set; }
        public int vehicleRegId { get; set; }
        [Required(AllowEmptyStrings = false), MaxLength(11)]
        public string vehicleRegNo { get; set; }
        public string vehicleName { get; set; }
        public string vehiclType { get; set; }
        public int weightPassingKg { get; set; }
        public int vehicleLength { get; set; }
        public int vehicleBreadth { get; set; }
        public string? chassisNo { get; set; }
        public string? engineNo { get; set; }
        public DateTime? purchaseDate { get; set; }
        public DateTime? fcDate { get; set; }
        public DateTime? insuranceDate { get; set; }
        public DateTime? dueDate { get; set; }
        public int? createdBy { get; set; }
        public DateTime createdDate { get; set; } = DateTime.Now;
        public int? updatedBy { get; set; }
        public DateTime? updatedDate { get; set; }
        public int? deletdBy { get; set; }
        public DateTime? deletedDate { get; set; }
        public bool isActive { get; set; } = true;
        public bool isDue { get; set; } = true;
    }
}
