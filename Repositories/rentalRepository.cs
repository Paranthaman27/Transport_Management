using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transport_Management.Helpers.DbContexts;
using Transport_Management.Models.Entity;
using Transport_Management.Interface;
using Transport_Management.Models.DTO;
using System.ComponentModel.Design;

namespace Transport_Management.Repositories
{

    public class rentalRepository : IrentalRepository
    {
        private readonly dbContext _dbContext;
        private IApiResponseRepository _apiResponseRepository;

        public rentalRepository(dbContext dbContext, IApiResponseRepository apiResponseRepository)
        {
            _dbContext = dbContext;
            _apiResponseRepository = apiResponseRepository;
        }
        public ApiResponseDTO checkRentalExistById(int mstRentalId)
        {
            var RentalDetails = _dbContext.mstRentalEntry.Where(a => a.mstRentalId == mstRentalId).FirstOrDefault();
            if (RentalDetails != null)
            {
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Rental Details Exist", data = RentalDetails });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Rental not Found" });
        }
        public ApiResponseDTO getRentalList()
        {
            List<mstRentalEntry> RentalDetailsList = _dbContext.mstRentalEntry.Where(a => a.isActive == true).ToList();
            if (RentalDetailsList != null)
            {
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Data Fetched Successfully", data = RentalDetailsList });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Rental details not Found" });
        }
        public ApiResponseDTO addRentaldetails(mstRentalEntry Rental)
        {
            ApiResponseDTO resultRentalDetails = checkRentalExistById(Rental.mstRentalId);
            mstRentalEntry RentalData = resultRentalDetails.data;
            if (resultRentalDetails.success != true)
            {
                _dbContext.mstRentalEntry.Add(Rental);
                _dbContext.SaveChanges();
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Rental Details Successfully Regisered" });
            }
            else if (resultRentalDetails.success == true && RentalData.isActive == false)
            {
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Rental Details Already Exist. State is isActive False" });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Rental Details already Exist" });
        }
        public ApiResponseDTO editRentaldetails(mstRentalEntry Rental)
        {
            ApiResponseDTO resultRentalDetails = checkRentalExistById(Rental.mstRentalId);
            mstRentalEntry RentalData = resultRentalDetails.data;
            if (resultRentalDetails.success == true && RentalData.isActive == true)
            {
                RentalData = Rental;
                _dbContext.mstRentalEntry.Update(RentalData);
                _dbContext.SaveChanges();
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Rental Details Updated Successfully " });
            }
            else if (resultRentalDetails.success == true && RentalData.isActive == false)
            {
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Rental Details are Deleted" });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Something Went wrong" });
        }
        public ApiResponseDTO deleteRentaldetails(mstRentalEntry Rental)
        {
            ApiResponseDTO resultRentalDetails = checkRentalExistById(Rental.mstRentalId);
            mstRentalEntry RentalData = resultRentalDetails.data;
            if (resultRentalDetails.success == true && RentalData.isActive == true)
            {
                RentalData.isActive = false;
                _dbContext.mstRentalEntry.Update(RentalData);
                _dbContext.SaveChanges();
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Rental Details Deleted Successfully " });
            }
            else if (resultRentalDetails.success == true && RentalData.isActive == false)
            {
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Rental Details Already Deleted" });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Something Went wrong" });
        }

        public ApiResponseDTO getFilteredRentals(DateTime fromDate, DateTime toDate, int? companyId, int? vehicleId)
        {
            List<mstRentalEntry> filteredRentals = new List<mstRentalEntry>();
            if (companyId != 0 && companyId != null) { 
                 filteredRentals = _dbContext.mstRentalEntry.Where(r => (r.rentalDate >= fromDate) && (r.rentalDate <= toDate) && (r.mstCompanyId == companyId) && (vehicleId == null || r.mstVehicleId == vehicleId) && r.isActive == true).ToList();
            }
            else
            {
                filteredRentals = _dbContext.mstRentalEntry.Where(r => (r.rentalDate >= fromDate) && (r.rentalDate <= toDate)   && (vehicleId == null || r.mstVehicleId == vehicleId) && r.isActive == true).ToList();
            }

                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Filtered Rentals Retrieved Successfully.", data = filteredRentals });
           
        }

    }
}