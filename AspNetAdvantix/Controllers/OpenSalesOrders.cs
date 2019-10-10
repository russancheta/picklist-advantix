using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetAdvantix.Helpers;
using AspNetAdvantix.Model;
using AspNetAdvantix.ViewModel;
using SAPbobsCOM;

namespace AspNetAdvantix.Controllers
{
    [Route("api/controller")]
    [ApiController]

    public class OpenSO : ControllerBase
    {
        private readonly AdvantixContext _context;
        private readonly PicklistContext _picklistcontext;
        public OpenSO(AdvantixContext context, PicklistContext picklistcontext)
        {
            _context = context;
            _picklistcontext = picklistcontext;
        }
        [HttpGet("getOpenSalesOrder")]
        public async Task<ActionResult<IEnumerable<OpenSalesOrder>>> getOpenSalesOrder()
        {
            var openSOQuery = @"
                select
                    a.DocNum,
                    a.DocDate,
                    case
                        when a.U_SO_TYPE = 'C'
                        then 'Consignment'
                        when a.U_SO_TYPE = 'O'
                        then 'Outright'
                    end 'SOType',
                    a.CardCode,
                    a.CardName,
                    a.U_ITR_BRANCH 'WhseBranch',
                    b.ItemCode,
                    b.Dscription,
                    b.Quantity,
                    b.Quantity [QtyToPost],
                    b.WhsCode,
                    case
                        when a.U_SO_TYPE = 'C'
                        then (select z.WhsName from OWHS z where z.WhsCode = a.U_ITR_BRANCH)
                        when a.U_SO_TYPE = 'O'
                        then 'ADVANTIX MARKETING WHSE'--(select z.WhsName from OWHS z where z.WhsCode = b.WhsCode)
                    end 'Branch',
                    b.DocEntry,
                    b.LineNum,
                    b.ObjType
                from
                    ORDR a
                    inner join RDR1 b on a.DocEntry = b.DocEntry
                where
                    a.DocStatus = 'O'
                    and a.CANCELED = 'N'
                    and b.LineStatus = 'O'";
            var openSO = await _context.OpenSalesOrder.FromSql(openSOQuery).ToListAsync();
            return openSO;
        }

        [HttpPost("createOPKL")]
        public ActionResult<ResultResponse> createOPKL([FromBody]List<OpenSalesOrder> list)
        {
            // List<OpenSalesOrder> sortedList = list.OrderBy(si => si.DocNum).ToList();

            // var container = new List<OpenSalesOrder>();
            // for (int i = 0; i < sortedList.Count; i++)
            // {
            //         int comparisonIndex 
            // }
            OPKLViewModel opklVM = new OPKLViewModel();
            List<OPKLContentViewModel> opklContentList = new List<OPKLContentViewModel>();
            foreach (var obj in list)
            {
                opklContentList.Add(new OPKLContentViewModel()
                {
                    OrderEntry = obj.DocEntry,
                    OrderLine = obj.LineNum,
                    BaseObjectType = obj.ObjType,
                    RelQtty = obj.QtyToPost
                });

            }
            opklVM.opklContent = opklContentList;
            var result = postPickList(opklVM);
            return result;
        }

        private ResultResponse postPickList(OPKLViewModel model)
        {
            SAPbobsCOM.PickLists oOpkl = (SAPbobsCOM.PickLists)DIApi._oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPickLists);

            // header
            // oOpkl.PickDate = model.PickDate;
            // oOpkl.Remarks = model.Remarks;

            // rows 
            foreach (var opklc in model.opklContent)
            {
                oOpkl.Lines.OrderEntry = opklc.OrderEntry;
                oOpkl.Lines.OrderRowID = opklc.OrderLine;
                oOpkl.Lines.BaseObjectType = opklc.BaseObjectType;
                oOpkl.Lines.ReleasedQuantity = Convert.ToDouble(opklc.RelQtty);
                oOpkl.Lines.Add();
            }

            int lRetCode = oOpkl.Add();
            if (lRetCode == 0)
            {
                return new ResultResponse
                {
                    Result = "Success",
                    Message = "Transaction successfuly posted",
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