using Microsoft.AspNetCore.Mvc;
using Transport_Management.Helpers.Filters;
using Transport_Management.Interface;
using Transport_Management.Models;
using Transport_Management.Models.Entity;
using System.Collections.Generic;
using static Transport_Management.Models.clsViewModel;

namespace Transport_Management.Controllers
{
    [SessionAuthorize]
    [RoleAuthorize(clsCommon.sSuperAdmin, clsCommon.sAdmin)]
    public class InvoiceController : Controller
    {
        private IApiResponseRepository _apiResponseRepository;
        private IinvoiceRepository _invoiceRepose;
        private IcompanyRepository _companyRepose;
        private IvehicleRepository _vehicleRepose;
        private IrentalRepository _rentalRepose;
        public InvoiceController(IApiResponseRepository apiResponseRepository, IinvoiceRepository invoiceRepose, IcompanyRepository companyRepose, IvehicleRepository vehicleRepose, IrentalRepository rentalRepose)
        {
            _apiResponseRepository = apiResponseRepository;
            _invoiceRepose = invoiceRepose;
            _companyRepose = companyRepose;
            _vehicleRepose = vehicleRepose;
            _rentalRepose = rentalRepose;
        }
        public IActionResult Invoice()
        {
            List<mstCompany> companyList = (_companyRepose.getcompanyList()).data;
            List<mstVehicle> vehicleList = (_vehicleRepose.getVehicleList()).data;
            List<mstRentalEntry> invoicerentalList = (_invoiceRepose.getNotBilledInvoiceList()).data;
			//List<mstRentalEntry> rentalList = invoicerentalList.Where(a=> a.isGST = true).ToList();
			//List<mstRentalEntry> rentalList = (_rentalRepose.getRentalList()).data;
            clsInvoiceViewModel viewModel = new clsInvoiceViewModel();
            viewModel.lstDrpCompany = companyList;
            viewModel.lstDrpVehicle = vehicleList;
            viewModel.rentalList = invoicerentalList;
            return View(viewModel);
        }
        public IActionResult GetInvoiceList()
        {
            var Invoices = _invoiceRepose.getInvoiceList();
            return Json(Invoices);
        }

        [HttpGet]
        public IActionResult GetInvoiceById(int id)
        {
            var Invoice = _invoiceRepose.checkInvoiceExistById(id);
            return Json(Invoice);
        }
        [HttpGet]
        public IActionResult getFilteredInvoices(DateTime fromDate, DateTime toDate, int? companyId)
        {
            var Invoice = _invoiceRepose.getFilteredInvoices( fromDate,toDate,companyId);
            return Json(Invoice);
        }

        [HttpPost]
        public IActionResult SaveInvoice(mstInvoice Invoice)
        {
            if (Invoice.mstInvoiceId == 0)
                return Json(_invoiceRepose.addInvoicedetails(Invoice));
            else
                return Json(_invoiceRepose.editInvoicedetails(Invoice));
        }

        [HttpDelete]
        public IActionResult DeleteInvoice(int id)
        {
            var Invoice = new mstInvoice { mstInvoiceId = id };
            return Json(_invoiceRepose.deleteInvoicedetails(Invoice));
        }

        [HttpPost]
        public IActionResult GenerateInvoice(string rentalId)
        {
            if (rentalId != null)
                return Json(_invoiceRepose.generateInvoice(rentalId));
            else
                return BadRequest();
        }
    }
}
