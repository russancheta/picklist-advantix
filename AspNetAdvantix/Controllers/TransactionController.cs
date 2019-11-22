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
    [Route("api/controller")]
    [ApiController]

    public class TransactionController : ControllerBase
    {
        private readonly AdvantixContext _context;

        public TransactionController(AdvantixContext context)
        {
            _context = context;
        }

        [HttpPost("postTransaction")]
        public ActionResult<ResultResponse> postTransaction([FromBody]List<OpenSalesOrder> listModel, string token)
        {
            var response = new ResultResponse();
            List<OpenSalesOrder> sortedList = listModel.OrderBy(si => si.DocNum).ToList();

            List<OpenSalesOrder> container = new List<OpenSalesOrder>();
            for (int i = 0; i < sortedList.Count; i++)
            {
                int comparisonIndex = 0;
                if (i < sortedList.Count - 1)
                {
                    comparisonIndex = i + 1;
                }
                else
                {
                    comparisonIndex = i;
                }
                container.Add(sortedList[i]);
                Console.WriteLine("Comparison Index: " + comparisonIndex);
                if (sortedList[i].DocNum != sortedList[comparisonIndex].DocNum)
                {
                    if (container[0].SOType == "Consignment")
                    {
                        response = postIT(container, token);
                        if (response.Result == "Failed")
                        {
                            return response;
                        }
                    }
                    else
                    {
                        response = postDR(container, token);
                        if (response.Result == "Failed")
                        {
                            return response;
                        }
                    }
                    container = new List<OpenSalesOrder>();
                }

                if (i + 1 == sortedList.Count)
                {
                    if (container[0].SOType == "Consignment")
                    {
                        response = postIT(container, token);
                    }
                    else
                    {
                        response = postDR(container, token);
                    }
                    container = new List<OpenSalesOrder>();
                }
            }

            return response;
        }

        private ResultResponse postIT(List<OpenSalesOrder> list, string token)
        {
            ResultResponse response = new ResultResponse();
            Console.WriteLine("POST IT");
            Company oCompany = DIApi.companyLoggedIn.Find(c => c.Token == token).SAPCompany;
            if (list[0].CardName.Contains("AUTOMATIC"))
            {
                foreach (var obj in list)
                {
                    for (int i = 0; i < obj.QtyToPost; i++)
                    {
                        StockTransfer oStockTransfer = (SAPbobsCOM.StockTransfer)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer);

                        // header
                        oStockTransfer.DocDate = list[0].DocDate.AddDays(1);
                        Console.WriteLine(list[0].DocDate);
                        oStockTransfer.CardCode = list[0].CardCode;
                        oStockTransfer.FromWarehouse = "01";
                        oStockTransfer.ToWarehouse = list[0].WhseBranch;
                        oStockTransfer.UserFields.Fields.Item("U_IT_TYPE").Value = "SALES";
                        oStockTransfer.UserFields.Fields.Item("U_AR_SO").Value = list[0].DocNum.ToString();
                        oStockTransfer.UserFields.Fields.Item("U_AR_PONO").Value = list[0].PONo;
                        oStockTransfer.UserFields.Fields.Item("U_ITR_BRANCH").Value = list[0].WhseBranch;
                        if (obj.SOType == "Consignment")
                        {
                            oStockTransfer.UserFields.Fields.Item("U_SO_TYPE").Value = "C";
                        }
                        else
                        {
                            oStockTransfer.UserFields.Fields.Item("U_SO_TYPE").Value = "O";
                        }
                        oStockTransfer.UserFields.Fields.Item("U_DelStat").Value = list[0].DelStat;
                        oStockTransfer.Comments = $"Based on Sales Order {list[0].DocNum}";

                        // rows
                        oStockTransfer.Lines.ItemCode = obj.ItemCode;
                        oStockTransfer.Lines.Quantity = 1;
                        if (obj.UseBaseUnits == "Y")
                        {
                            oStockTransfer.Lines.UseBaseUnits = SAPbobsCOM.BoYesNoEnum.tYES;
                        }
                        else
                        {
                            oStockTransfer.Lines.UseBaseUnits = SAPbobsCOM.BoYesNoEnum.tNO;
                        }
                        oStockTransfer.Lines.Add();

                        int RetCode = oStockTransfer.Add();

                        if (RetCode == 0)
                        {
                            var updateRelease = @"update a set a.U_SO_Released = 'Y' from ORDR a where a.DocEntry = {0}";
                            var updatedReleased = _context.Database.ExecuteSqlCommand(updateRelease, list[0].DocEntry);
                            if (updatedReleased != 0)
                            {
                                response.Result = "Success";
                                response.Message = "";
                                response.ResultData = "";
                            }
                            else
                            {
                                response.Result = "Failed";
                                response.Message = oCompany.GetLastErrorDescription();
                                response.ResultData = "";
                            }
                        }
                        else
                        {
                            response.Result = "Failed";
                            response.Message = oCompany.GetLastErrorDescription();
                            response.ResultData = "";
                        }
                    }
                }

                // close Sales Order upon posting of Inventory Transfer
                Documents oSalesOrder = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
                oSalesOrder.GetByKey(list[0].DocEntry);
                int iRetCode = oSalesOrder.Close();
            }
            else
            {
                StockTransfer oStockTransfer = (SAPbobsCOM.StockTransfer)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer);
                oStockTransfer.CardCode = list[0].CardCode;
                oStockTransfer.FromWarehouse = "01";
                oStockTransfer.ToWarehouse = list[0].WhseBranch;
                oStockTransfer.UserFields.Fields.Item("U_IT_TYPE").Value = "SALES";
                oStockTransfer.UserFields.Fields.Item("U_AR_SO").Value = list[0].DocNum.ToString();
                oStockTransfer.Comments = $"Based on Inv. Transfer Req. {list[0].DocNum}";
                foreach (var obj in list)
                {
                    oStockTransfer.Lines.ItemCode = obj.ItemCode;
                    oStockTransfer.Lines.Quantity = Convert.ToDouble(obj.QtyToPost);
                    oStockTransfer.Lines.BaseEntry = obj.DocEntry;
                    oStockTransfer.Lines.BaseLine = obj.LineNum;
                    oStockTransfer.Lines.BaseType = SAPbobsCOM.InvBaseDocTypeEnum.InventoryTransferRequest;
                    oStockTransfer.Lines.Add();
                }
                int RetCode = oStockTransfer.Add();
                if (RetCode == 0)
                {
                    var updateRelease = @"update a set a.U_SO_Released = 'Y' from ORDR a where a.DocEntry = {0}";
                    var updatedRelease = _context.Database.ExecuteSqlCommand(updateRelease, list[0].DocEntry);
                    if (updatedRelease != 0)
                    {
                        response.Result = "Success";
                        response.Message = "";
                        response.ResultData = "";
                    }
                    else
                    {
                        response.Result = "Failed";
                        response.Message = "";
                        response.ResultData = "";
                    }
                }
                else
                {
                    response.Result = "Failed";
                    response.Message = oCompany.GetLastErrorDescription();
                    response.ResultData = "";
                }
            }
            return response;
        }

        private ResultResponse postDR(List<OpenSalesOrder> list, string token)
        {
            ResultResponse response = new ResultResponse();
            Console.WriteLine("POST DR");
            Company oCompany = DIApi.companyLoggedIn.Find(c => c.Token == token).SAPCompany;
            Documents oDeliveryNotes = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDeliveryNotes);
            oDeliveryNotes.CardCode = list[0].CardCode;
            oDeliveryNotes.UserFields.Fields.Item("U_DelStat").Value = "In-Transit";
            oDeliveryNotes.Comments = $"Based on Sales Order {list[0].DocNum}";
            foreach (var obj in list)
            {
                oDeliveryNotes.Lines.ItemCode = obj.ItemCode;
                oDeliveryNotes.Lines.Quantity = Convert.ToDouble(obj.QtyToPost);
                oDeliveryNotes.Lines.BaseEntry = obj.DocEntry;
                oDeliveryNotes.Lines.BaseLine = obj.LineNum;
                oDeliveryNotes.Lines.BaseType = Convert.ToInt32(obj.ObjType);
                oDeliveryNotes.Lines.Add();
            }
            int RetCode = oDeliveryNotes.Add();
            if (RetCode == 0)
            {
                var updateRelease = @"update a set a.U_SO_Released = 'Y' from ORDR a where a.DocEntry = {0}";
                var updatedRelease = _context.Database.ExecuteSqlCommand(updateRelease, list[0].DocEntry);
                if (updatedRelease != 0)
                {
                    response.Result = "Success";
                    response.Message = oCompany.GetLastErrorDescription();
                    response.ResultData = "";
                }
                else
                {
                    response.Result = "Failed";
                    response.Message = "";
                    response.ResultData = "";
                }
            }
            else
            {
                response.Result = "Failed";
                response.Message = oCompany.GetLastErrorDescription();
                response.ResultData = "";
            }
            return response;
        }

        [HttpPut("closeSO")]
        public ActionResult<ResultResponse> closeSO(List<ForClosingViewModel> list, string token)
        {
            ResultResponse response = new ResultResponse();
            Company oCompany = DIApi.companyLoggedIn.Find(c => c.Token == token).SAPCompany;
            foreach (var obj in list)
            {
                if (obj.Type == "SO")
                {
                    Console.WriteLine("CLOSE SO");
                    Documents oSalesOrder = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
                    oSalesOrder.GetByKey(obj.DocEntry);
                    int iRetCode = oSalesOrder.Close();
                    if (iRetCode != 0)
                    {
                        return new ResultResponse
                        {
                            Result = "Failed",
                            Message = oCompany.GetLastErrorDescription()
                        };
                    }
                }
                else
                {
                    Console.WriteLine("CLOSE ITR");
                    StockTransfer oInvTransferReq = (SAPbobsCOM.StockTransfer)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest);
                    oInvTransferReq.GetByKey(obj.DocEntry);
                    int iRetCode = oInvTransferReq.Close();
                    if (iRetCode != 0)
                    {
                        return new ResultResponse
                        {
                            Result = "Failed",
                            Message = oCompany.GetLastErrorDescription()
                        };
                    }
                }
            }
            return new ResultResponse
            {
                Result = "Success",
                Message = "Document successfully closed"
            };
        }
    }
}