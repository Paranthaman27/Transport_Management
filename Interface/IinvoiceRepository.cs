using Transport_Management.Models.DTO;
using Transport_Management.Models.Entity;

namespace Transport_Management.Interface
{
    public interface IinvoiceRepository
    {
        ApiResponseDTO checkInvoiceExistById(int invoiceId);
        ApiResponseDTO getInvoiceList();
        ApiResponseDTO addInvoicedetails(mstInvoice invoice);
        ApiResponseDTO editInvoicedetails(mstInvoice invoice);
        ApiResponseDTO deleteInvoicedetails(mstInvoice invoice);
        ApiResponseDTO getNotBilledInvoiceList();
        ApiResponseDTO generateInvoice(string rentalIds);
        ApiResponseDTO getFilteredInvoices(DateTime fromDate, DateTime toDate, int? companyId);


    }
}
