using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAPbobsCOM;

namespace AspNetAdvantix.Helpers
{
    public static class DIApi
    {
        public static Company _oCompany;
        public static bool _connect;
        public static bool Connect()
        {
            Company oCompany = new Company();
            oCompany.LicenseServer = "10.0.100.31";
            oCompany.language = BoSuppLangs.ln_English;
            oCompany.UseTrusted = false;

            oCompany.DbServerType = BoDataServerTypes.dst_MSSQL2012;

            oCompany.Server = "10.0.100.30";
            oCompany.DbUserName = "sa";
            oCompany.DbPassword = "B1Admin";
            oCompany.CompanyDB = "TESTDATABASE";
            oCompany.UserName = "manager";
            oCompany.Password = "asdf";

            int con = oCompany.Connect();
            if (con == 0)
            {
                _oCompany = oCompany;
                _connect = true;
                Console.WriteLine("SAP Connected");
                return true;
            }
            else
            {
                _connect = false;
                return false;
            }
        }
    }
}