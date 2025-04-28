using System;
using Transport_Management.Models.DTO;
using Transport_Management.Models.Entity;

namespace Transport_Management.Interface
{
    public interface IcompanyRepository
    {
        ApiResponseDTO checkCompanyExistById(int companyId);
        ApiResponseDTO getcompanyList();
        ApiResponseDTO addcompanydetails(mstCompany company);
        ApiResponseDTO editcompanydetails(mstCompany company);
        ApiResponseDTO deletecompanydetails(mstCompany company);


    }
}

