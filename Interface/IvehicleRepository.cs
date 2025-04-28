using Transport_Management.Models.DTO;
using Transport_Management.Models.Entity;

namespace Transport_Management.Interface
{
    public interface IvehicleRepository
    {
        ApiResponseDTO addVehicledetails(mstVehicle vehicle);
        ApiResponseDTO editVehicledetails(mstVehicle vehicle);
        ApiResponseDTO deleteVehicledetails(mstVehicle vehicle);
        ApiResponseDTO checkVehicleExistByRegId(string vehicleRegId);
        ApiResponseDTO getVehicleList();
    }
}
