using Microsoft.AspNetCore.Mvc;
using Transport_Management.Interface;
using Transport_Management.Models.DTO;
using Transport_Management.Models.Entity;
using Transport_Management.Models;
using static Transport_Management.Models.clsViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Transport_Management.Helpers.Filters;
using Transport_Management.Repositories;

namespace Transport_Management.Controllers
{
    [SessionAuthorize]
    [RoleAuthorize(clsCommon.sSuperAdmin, clsCommon.sAdmin,clsCommon.sDriver)]
    public class RentalController : Controller
    {
        private IApiResponseRepository _apiResponseRepository;
        private IrentalRepository _rentalRepose;
        private IcompanyRepository _companyRepose;
        private IvehicleRepository _vehicleRepose;
        public RentalController(IApiResponseRepository apiResponseRepository, IrentalRepository rentalRepose, IcompanyRepository companyRepose, IvehicleRepository vehicleRepose)
        {
            _apiResponseRepository = apiResponseRepository;
            _rentalRepose = rentalRepose;
            _companyRepose = companyRepose;
            _vehicleRepose = vehicleRepose;
        }
        public IActionResult Rental()
        {
            List<mstCompany> companyList = (_companyRepose.getcompanyList()).data;
            List<mstVehicle> vehicleList = (_vehicleRepose.getVehicleList()).data;
            clsRentalViewModel viewModel = new clsRentalViewModel();
            viewModel.lstDrpCompany = companyList;
            viewModel.lstDrpVehicle = vehicleList;
            return View(viewModel);
        }
        public IActionResult _Rental()
        {
            return PartialView();
        }


        [HttpGet]
        public IActionResult GetRentalList()
        {
            var Rentals = _rentalRepose.getRentalList();
            clsRentalViewModel viewModel = new clsRentalViewModel();
            viewModel.lstrentalEntry = Rentals.data;
         //   return PartialView("_Rental", viewModel);
            return Json(Rentals);
        }

        [HttpGet]
        public IActionResult GetRentalById(int id)
        {
            var Rental = _rentalRepose.checkRentalExistById(id);
            return Json(Rental);
        }

        [HttpPost]
        public IActionResult SaveRental(mstRentalEntry Rental)
        {
            if (Rental.mstRentalId == 0)
                return Json(_rentalRepose.addRentaldetails(Rental));
            else
                return Json(_rentalRepose.editRentaldetails(Rental));
        }

        [HttpDelete]
        public IActionResult DeleteRental(int id)
        {
            var Rental = new mstRentalEntry { mstRentalId = id };
            return Json(_rentalRepose.deleteRentaldetails(Rental));
        }


        [HttpGet]
        public IActionResult GetFilteredRentals(DateTime fromDate, DateTime toDate, int? companyId, int? vehicleId)
        {
            try
            {
                var filteredRentals = _rentalRepose.getFilteredRentals(fromDate, toDate, companyId, vehicleId);
                clsRentalViewModel viewModel = new clsRentalViewModel();
                viewModel.lstrentalEntry = filteredRentals.data;
                //return PartialView("_Rental", viewModel);
                return Json(filteredRentals);
               // return PartialView("_Rental", filteredRentals.data);
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(_apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "An error occurred while filtering rentals." }));
            }
        }
    }
}
