using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transport_Management.Helpers.DbContexts;
using Transport_Management.Models.Entity;
using Transport_Management.Interface;
using Transport_Management.Models.DTO;

namespace Transport_Management.Repositories
{

    public class vehicleRepository : IvehicleRepository
    {
        private readonly dbContext _dbContext;
        private IApiResponseRepository _apiResponseRepository;

        public vehicleRepository(dbContext dbContext, IApiResponseRepository apiResponseRepository)
        {
            _dbContext = dbContext;
            _apiResponseRepository = apiResponseRepository;
        }
        public ApiResponseDTO checkVehicleExistByRegId(string vehicleRegNo)
        {
             var vehicleDetails = _dbContext.mstVehicle.Where(a => a.vehicleRegNo == vehicleRegNo).FirstOrDefault();
            if (vehicleDetails != null)
            {
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Vehicle Details Exist", data = vehicleDetails });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Vehicle not Found" });
        }
        public ApiResponseDTO getVehicleList()
        {
            List<mstVehicle> vehicleDetailsList = _dbContext.mstVehicle.ToList();
            if (vehicleDetailsList != null)
            {
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Data Fetched Successfully", data = vehicleDetailsList });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Vehicle details not Found" });
        }
        public ApiResponseDTO addVehicledetails(mstVehicle vehicle)
        {
            ApiResponseDTO resultVehicleDetails = checkVehicleExistByRegId(vehicle.vehicleRegNo);
            mstVehicle vehicleData = resultVehicleDetails.data;
            if (resultVehicleDetails.success != true)
            {
                _dbContext.mstVehicle.Add(vehicle);
                _dbContext.SaveChanges();
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Vehicle Details Successfully Regisered" });
            }
            else if (resultVehicleDetails.success == true && vehicleData.isActive == false)
            {
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Vehicle Details Already Exist. State is isActive False" });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Vehicle Details already Exist" });
        }
        public ApiResponseDTO editVehicledetails(mstVehicle vehicle)
        {
            ApiResponseDTO resultVehicleDetails = checkVehicleExistByRegId(vehicle.vehicleRegNo);
            mstVehicle vehicleData = resultVehicleDetails.data;
            if (resultVehicleDetails.success == true && vehicleData.isActive == true)
            {
                vehicleData.chassisNo = vehicle.chassisNo;
                vehicleData.engineNo = vehicle.engineNo;
                vehicleData.vehicleRegNo = vehicle.vehicleRegNo;
                vehicleData.vehicleLength = vehicle.vehicleLength;
                vehicleData.engineNo = vehicle.engineNo;
                vehicleData.dueDate = vehicle.dueDate;
                vehicleData.isDue = vehicle.isDue;
                vehicleData.isActive = vehicle.isActive;
                vehicleData.dueDate = vehicle.dueDate;
                vehicleData.fcDate = vehicle.fcDate;
                vehicleData.insuranceDate = vehicle.insuranceDate;
                vehicleData.purchaseDate = vehicle.purchaseDate;
                vehicleData.weightPassingKg = vehicle.weightPassingKg;
                vehicleData.vehicleBreadth = vehicle.vehicleBreadth;
                vehicleData.vehicleRegId = vehicle.vehicleRegId;
                vehicleData.updatedDate =DateTime.Now;
                vehicleData.updatedBy = vehicle.createdBy;
                _dbContext.mstVehicle.Update(vehicleData);
                _dbContext.SaveChanges();
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Vehicle Details Updated Successfully " });
            }
            else if (resultVehicleDetails.success == true && vehicleData.isActive == false)
            {
                vehicleData.isActive = true;
                _dbContext.mstVehicle.Update(vehicleData);
                _dbContext.SaveChanges();
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Vehicle Details made to Active" });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Something Went wrong" });
        }
        public ApiResponseDTO deleteVehicledetails(mstVehicle vehicle)
        {
            ApiResponseDTO resultVehicleDetails = checkVehicleExistByRegId(vehicle.vehicleRegNo);
            mstVehicle vehicleData = resultVehicleDetails.data;
            if (resultVehicleDetails.success == true && vehicleData.isActive == true)
            {
                vehicleData.isActive = false;
                _dbContext.mstVehicle.Update(vehicleData);
                _dbContext.SaveChanges();
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Vehicle Details Deleted Successfully " });
            }
            else if (resultVehicleDetails.success == true && vehicleData.isActive == false)
            {
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Vehicle Details Already Deleted" });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Something Went wrong" });
        }

    }
}
