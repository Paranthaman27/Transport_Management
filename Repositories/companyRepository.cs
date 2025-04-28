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

    public class companyRepository : IcompanyRepository
    {
        private readonly dbContext _dbContext;
        private IApiResponseRepository _apiResponseRepository;

        public companyRepository(dbContext dbContext, IApiResponseRepository apiResponseRepository)
        {
            _dbContext = dbContext;
            _apiResponseRepository = apiResponseRepository;
        }
        public ApiResponseDTO checkCompanyExistById(int mstCompanyId)
        {
            var companyDetails = _dbContext.mstCompany.Where(a => a.mstCompanyId == mstCompanyId).FirstOrDefault();
            if (companyDetails != null)
            {
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Company Details Exist", data = companyDetails });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Company not Found" });
        }
        public ApiResponseDTO getcompanyList()
        {
            List<mstCompany> companyDetailsList = _dbContext.mstCompany.Where(a => a.isActive == true).ToList();
            if (companyDetailsList != null)
            {
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Data Fetched Successfully", data = companyDetailsList });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Company details not Found" });
        }
        public ApiResponseDTO addcompanydetails(mstCompany company)
        {
            ApiResponseDTO resultcompanyDetails = checkCompanyExistById(company.mstCompanyId);
            mstCompany companyData = resultcompanyDetails.data;
            if (resultcompanyDetails.success != true)
            {
                _dbContext.mstCompany.Add(company);
                _dbContext.SaveChanges();
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Company Details Successfully Regisered" });
            }
            else if (resultcompanyDetails.success == true && companyData.isActive == false)
            {
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Company Details Already Exist. State is isActive False" });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Company Details already Exist" });
        }
        public ApiResponseDTO editcompanydetails(mstCompany company)
        {
            ApiResponseDTO resultcompanyDetails = checkCompanyExistById(company.mstCompanyId);
            mstCompany companyData = resultcompanyDetails.data;
            if (resultcompanyDetails.success == true && companyData.isActive == true)
            {
                companyData = company;
                _dbContext.mstCompany.Update(companyData);
                _dbContext.SaveChanges();
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Company Details Updated Successfully " });
            }
            else if (resultcompanyDetails.success == true && companyData.isActive == false)
            {
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Company Details are Deleted" });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Something Went wrong" });
        }
        public ApiResponseDTO deletecompanydetails(mstCompany company)
        {
            ApiResponseDTO resultcompanyDetails = checkCompanyExistById(company.mstCompanyId);
            mstCompany companyData = resultcompanyDetails.data;
            if (resultcompanyDetails.success == true && companyData.isActive == true)
            {
                companyData.isActive = false;
                _dbContext.mstCompany.Update(companyData);
                _dbContext.SaveChanges();
                return _apiResponseRepository.SuccessResponse(new ApiResponseDTO { message = "Company Details Deleted Successfully " });
            }
            else if (resultcompanyDetails.success == true && companyData.isActive == false)
            {
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Company Details Already Deleted" });
            }
            return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Something Went wrong" });
        }

    }
}