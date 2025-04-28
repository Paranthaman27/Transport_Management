using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transport_Management.Helpers.DbContexts;
using Transport_Management.Interface;
using Transport_Management.Models.DTO;
using Transport_Management.Models.Entity;
using Transport_Management.Repositories;
using System;
using Microsoft.AspNetCore.Session;
using System.Text;
using Transport_Management.Helpers.Filters;
using Transport_Management.Models;

namespace Transport_Management.Controllers
{
    [SessionAuthorize]
    [RoleAuthorize(clsCommon.sSuperAdmin, clsCommon.sAdmin,clsCommon.sDriver)]
    public class VehicleController : Controller
    {
        private IApiResponseRepository _apiResponseRepository;
        private IvehicleRepository _vehicleRepose;
        public VehicleController (IApiResponseRepository apiResponseRepository, IvehicleRepository vehicleRepose)
        {
            _apiResponseRepository = apiResponseRepository;
            _vehicleRepose = vehicleRepose;
        }
        public IActionResult Vehicle()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetVehicleList()
        {
            var vehicles = _vehicleRepose.getVehicleList();
            return Json(vehicles);
        }

        [HttpGet]
        public IActionResult GetVehicleById(string id)
        {
            var vehicle = _vehicleRepose.checkVehicleExistByRegId(id);
            return Json(vehicle);
        }

        [HttpPost]
        public IActionResult SaveVehicle(mstVehicle vehicle)
        {
            if (vehicle.vehicleRegNo==null)
                return Json(_vehicleRepose.addVehicledetails(vehicle));
            else
                return Json(_vehicleRepose.editVehicledetails(vehicle));
        }

        [HttpDelete]
        public IActionResult DeleteVehicle(string vehicleRegNo)
        {
            var vehicle = new mstVehicle { vehicleRegNo = vehicleRegNo };
            return Json(_vehicleRepose.deleteVehicledetails(vehicle));
        }
    }
}
