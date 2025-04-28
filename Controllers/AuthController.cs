using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transport_Management.Helpers.DbContexts;
using Transport_Management.Interface;
using Transport_Management.Models;
using Transport_Management.Models.DTO;
using Transport_Management.Models.Entity;
using Transport_Management.Repositories;
using System.Collections.Generic;
using System;
using System.Text;
using static Transport_Management.Models.clsCommon;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Transport_Management.Controllers
{
    public class AuthController : Controller
    {
        private IauthRepository _authRepose;

        private IApiResponseRepository _apiResponseRepository;
        public AuthController(IApiResponseRepository apiResponseRepository, IauthRepository authRepository)
        {
            _authRepose = authRepository;
            _apiResponseRepository = apiResponseRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            HttpContext.Session.Clear();
            ViewBag.SessionId = HttpContext.Session?.GetString("userSessionId");
            return View();
        }
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
        [HttpPost]
        public async Task<IActionResult> Register([Bind] mstUser user)
        {
            ApiResponseDTO registerResult = _authRepose.registerNewUser(user);
            if (registerResult.success == true)
            {
                

                return RedirectToAction("Login", "Auth");
            }
            ViewBag.vbRegisterWError = registerResult.message;
            return View(user);
        }


        [HttpGet]
        public ApiResponseDTO loginUser(userDTO user)
        {
            if (!ModelState.IsValid)
            {
                return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Something Went Wrong" });
                //return BadRequest();
            }
            else
            {
                ApiResponseDTO loginResult = _authRepose.loginUser(user);
                if (loginResult.success)
                {
                    string jwtdata = loginResult.data.jwtToken.ToString();
                    mstUser userdata = loginResult.data.userDatas;
                    HttpContext.Session.SetString("JWTToken", jwtdata);
                    HttpContext.Session.SetString("userSessionId", Convert.ToString(userdata.mstUserId));
                    HttpContext.Session.SetString("userName", userdata.userName);
                    HttpContext.Session.SetString("userEmail", userdata.userEmail);
                    string Role = getUserType(userdata.userTypeId);
                    HttpContext.Session.SetString("Role", Role);
                   
                   // return RedirectToAction("Index", "Home");
                }
                ViewBag.vbLoginError = loginResult.message;
                return loginResult;
               // return _apiResponseRepository.FailureResponse(new ApiResponseDTO { message = "Something Went Wrong" });
                // return View();
            }
        }
    }
}
