using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using AspNetAdvantix.Model;
using AspNetAdvantix.Helpers;
using AspNetAdvantix.ViewModel;
using SAPbobsCOM;

namespace AspNetAdvantix.Controllers
{
    [Route("api/controller")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private ITokenProvider _tokenProvider;
        public AuthController(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        [HttpGet("validateIsLoggedIn")]
        public ActionResult<ResultResponse> validateIsLoggedIn(string token)
        {
            var check = DIApi.companyLoggedIn.Find(c => c.Token == token);
            if (check == null)
            {
                return new ResultResponse
                {
                    Result = "Success",
                    Message = "Destroy Token",
                    ResultData = ""
                };
            }
            else
            {
                return new ResultResponse
                {
                    Result = "Failed",
                    Message = "Authorized",
                    ResultData = ""
                };
            }
        }

        [HttpPost("login")]
        public ActionResult<ResultResponse> Login([FromBody]CredentialsViewModel model)
        {
            Company oCompany = DIApi.LoginSAP(model.Address, model.DBName, model.UserName, model.Password);
            int con1 = oCompany.Connect();
            if (con1 == 0)
            {
                User user = new User();
                user.UserName = oCompany.UserName;

                int ageInMinutes = 1440;

                DateTime expiry = DateTime.UtcNow.AddMinutes(ageInMinutes);

                var token = new JsonWebToken
                {
                    access_token = _tokenProvider.CreateToken(user, expiry),
                    expires_in = ageInMinutes * 60
                };

                DIApi.companyLoggedIn.Add(new OnlineCompany()
                {
                    SAPCompany = oCompany,
                    Token = token.access_token
                });

                return new ResultResponse
                {
                    Result = "Success",
                    Message = "Successully logged in.",
                    ResultData = new
                    {
                        UserName = oCompany.UserName,
                        BranchName = oCompany.CompanyName,
                        BranchDB = oCompany.CompanyDB,
                        auth_token = token.access_token,
                        LicenseIP = oCompany.LicenseServer
                    }
                };
            }
            else
            {
                return new ResultResponse
                {
                    Result = "Failed",
                    Message = $"Unauthorized Code: {con1} - Message: {oCompany.GetLastErrorDescription()}",
                    ResultData = ""
                };
            }
        }

        [HttpGet("logout")]
        public ActionResult<ResultResponse> Logout(string token)
        {
            var element = DIApi.companyLoggedIn.Find(r => r.Token == token);
            element.SAPCompany.Disconnect();
            DIApi.companyLoggedIn.Remove(element);
            return new ResultResponse
            {
                Result = "Success",
                Message = "Successfully logged out.",
                ResultData = ""
            };
        }

        [HttpGet("getCompanyDetails")]
        public ActionResult<ResultResponse> getCompanyDetails(string dbName)
        {
            var element = DIApi.companyLoggedIn.Find(r => r.Token == dbName);
            return new ResultResponse
            {
                Result = "Success",
                Message = element.Token,
                ResultData = ""
            };
        }

        [HttpPost("branchLogin")]
        public ActionResult<ResultResponse> BranchLogin([FromBody] CredentialsViewModel credentials)
        {
            Company oCompany = DIApi.LoginSAP(credentials.Address, credentials.DBName, credentials.UserName, credentials.Password);

            int con = oCompany.Connect();
            if (con == 0)
            {
                var auth_data = new
                {
                    BranchName = oCompany.CompanyName,
                    BranchDB = oCompany.CompanyDB,
                    Username = oCompany.UserName,
                    Password = oCompany.Password
                };

                return new ResultResponse
                {
                    Result = "Success",
                    Message = "SAP Connected",
                    ResultData = auth_data
                };
            }
            else
            {
                return new ResultResponse
                {
                    Result = "Failed",
                    Message = oCompany.GetLastErrorDescription(),
                    ResultData = ""
                };
            }
        }
    }
}