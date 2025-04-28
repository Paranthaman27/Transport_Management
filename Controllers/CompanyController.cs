using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Transport_Management.Helpers.Filters;
using Transport_Management.Interface;
using Transport_Management.Models.DTO;
using Transport_Management.Models;
using Transport_Management.Models.Entity;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Transport_Management.Controllers
{
    [SessionAuthorize]
    [RoleAuthorize(clsCommon.sSuperAdmin,clsCommon.sAdmin)]
    public class CompanyController : Controller
    {
        private IApiResponseRepository _apiResponseRepository;
        private IcompanyRepository _companyRepose;
        public CompanyController(IApiResponseRepository apiResponseRepository, IcompanyRepository companyRepose)
        {
            _apiResponseRepository = apiResponseRepository;
            _companyRepose = companyRepose;
        }
        public IActionResult Company()
        {
            var companies = _companyRepose.getcompanyList();
            List<mstCompany> companyList = companies.data;

            clsCompanyViewModel viewModel = new clsCompanyViewModel();
            viewModel.activetCompanyList = companyList.Where(a=> a.isActive=true).ToList();
            viewModel.inActiveCompanyList = companyList.Where(a=> a.isActive=false).ToList();
            return View(viewModel);
        }
        [HttpGet]
        public IActionResult GetCompanyList()
        {
            var companies = _companyRepose.getcompanyList();
            return Json(companies);
        }

        [HttpGet]
        public IActionResult GetCompanyById(int id)
        {
            var company = _companyRepose.checkCompanyExistById(id);
            return Json(company);
        }

        [HttpPost]
        public IActionResult SaveCompany(mstCompany company)
        {
            if (company.mstCompanyId == 0)
                return Json(_companyRepose.addcompanydetails(company));
            else
                return Json(_companyRepose.editcompanydetails(company));
        }

        [HttpDelete]
        public IActionResult DeleteCompany(int id)
        {
            var company = new mstCompany { mstCompanyId = id };
            return Json(_companyRepose.deletecompanydetails(company));
        }

    }
}