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

    public class OWTRController : ControllerBase
    {
        [HttpPost ("createOWTR")]
        public ActionResult<ResultResponse> createOWTR ([FromBody]OWTRViewModel model)
        {
            StockTransfer oOwtr = (SAPbobsCOM.StockTransfer)DIApi._oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer);

            //header
            oOwtr.Series = model.Series;
            oOwtr.CardCode = model.CardCode;
            oOwtr.UserFields.Fields.Item("U_IT_TYPE").Value = model.ITType;
            oOwtr.FromWarehouse = model.FromWarehouse;
            oOwtr.ToWarehouse = model.ToWarehouse;
            oOwtr.Comments = model.Comments;

            // row data
            foreach (var owtrc in model.owtrContent)
            {
                oOwtr.Lines.ItemCode = owtrc.ItemCode;
                oOwtr.Lines.Quantity = owtrc.Quantity;
                oOwtr.Lines.Add();
            }

            int lRetCode = oOwtr.Add();
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