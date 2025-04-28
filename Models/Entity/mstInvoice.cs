using System.ComponentModel.DataAnnotations;

namespace Transport_Management.Models.Entity
{
    public class mstInvoice
    {
        [Key]
        public int mstInvoiceId { get; set; }
        public int billNo { get; set; }
        public int mstCompanyId { get; set; }
        public string mstRentalId { get; set; }
        public int SubTotal { get; set; }
        public int sgstAmount { get; set; }
        public int cgstAmount { get; set; }
        public int totalAmount { get; set; }
        public DateTime invoicedDate { get; set; }
        public int invoiceMonth { get; set; }
        public int invoiceYear { get; set; }
        public int? createdBy { get; set; }
        public DateTime createdDate { get; set; } = DateTime.Now;
        public int? updatedBy { get; set; }
        public DateTime? updatedDate { get; set; }
        public int? deletedBy { get; set; }
        public DateTime? deletedDate { get; set; }
        public bool isActive { get; set; } = true;
        public bool isPaid { get; set; } = false;
    }
}
