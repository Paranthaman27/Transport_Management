using Transport_Management.Models.DTO;
using Transport_Management.Models.Entity;

namespace Transport_Management.Interface
{
    public interface IrentalRepository
    {
        ApiResponseDTO checkRentalExistById(int RentalId);
        ApiResponseDTO getRentalList();
        ApiResponseDTO addRentaldetails(mstRentalEntry Rental);
        ApiResponseDTO editRentaldetails(mstRentalEntry Rental);
        ApiResponseDTO deleteRentaldetails(mstRentalEntry Rental);
        ApiResponseDTO getFilteredRentals(DateTime fromDate, DateTime toDate, int? companyId, int? vehicleId);
    }
}
