using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetAdvantix.Helpers;
using AspNetAdvantix.Model;
using AspNetAdvantix.ViewModel;
using SAPbobsCOM;

namespace AspNetAdvantix.Controllers
{
    [Route ("api/controller")]
    [ApiController]

    public class ODLNController : ControllerBase
    {
        [HttpPost ("createODLN")]
        public ActionResult<ResultResponse> createODLN ([FromBody]ODLNViewModel model)
        {
            Documents oOdln = (SAPbobsCOM.Documents)DIApi._oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDeliveryNotes);

            // header
            oOdln.CardCode = model.CardCode;
            oOdln.Comments = model.Comments;

            // row data
            foreach (var odlnc in model.odlnContent)
            {
                oOdln.Lines.ItemCode = odlnc.ItemCode;
                oOdln.Lines.Quantity = odlnc.Quantity;
                oOdln.Lines.BaseEntry = odlnc.BaseEntry;
                oOdln.Lines.BaseLine = odlnc.BaseLine;
                oOdln.Lines.BaseType = odlnc.BaseType;
                oOdln.Lines.Add();
            }

            int lRetCode = oOdln.Add();
            if (lRetCode == 0)
            {
                return new ResultResponse
                {
                    Result = "Success",
                    Message = DIApi._oCompany.GetLastErrorDescription(),
                    ResultData = ""
                };
            }
            else
            {
                return new ResultResponse
                {
                    Result = "Failed",
                    Message = DIApi._oCompany.GetLastErrorDescription(),
                    ResultData = ""
                };
            }
        }
    }
}
