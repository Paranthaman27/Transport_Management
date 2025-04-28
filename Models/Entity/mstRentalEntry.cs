using System.ComponentModel.DataAnnotations;

namespace Transport_Management.Models.Entity
{
    public class mstRentalEntry
    {
        [Key]
        public int mstRentalId { get; set; }
        public DateTime rentalDate { get; set; }
        public int mstVehicleId { get; set; }
        public int mstUserId { get; set; }
        public int? mstCompanyId { get; set; }
        public string rentalDescription { get; set; }
        public string partyName { get; set; }
        public int rentalAmount { get; set; }
        public int? createdBy { get; set; }
        public DateTime createdDate { get; set; } = DateTime.Now;
        public int? updatedBy { get; set; }
        public DateTime? updatedDate { get; set; }
        public int? deletedBy { get; set; }
        public DateTime? deletedDate { get; set; }
        public bool isActive { get; set; } = true;
        public bool isGST { get; set; }
        public bool isBilled { get; set; } = false;
        public int? mstInvoiceID { get; set; }
        public int? billNo { get; set; }
        public bool isPaid { get; set; } = false;

    }
}
