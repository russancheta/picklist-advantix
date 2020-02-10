using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAPbobsCOM;

namespace AspNetAdvantix.Helpers
{
    public static class DIApi
    {
        public static List<OnlineCompany> companyLoggedIn = new List<OnlineCompany>();
        public static Company _oCompany;
        public static bool _connect;
        public static SAPbobsCOM.Company LoginSAP(string address, string dbName, string userName, string password)
        {
            Company oCompany = new Company();
            oCompany.LicenseServer = "10.0.100.32";
            oCompany.language = BoSuppLangs.ln_English;
            oCompany.UseTrusted = false;

            oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2012;

            oCompany.Server = "10.0.100.30";
            oCompany.DbUserName = "sa";
            oCompany.DbPassword = "B1Admin";
            oCompany.CompanyDB = dbName;
            oCompany.UserName = userName;
            oCompany.Password = password;
            return oCompany;
        }
    }
}