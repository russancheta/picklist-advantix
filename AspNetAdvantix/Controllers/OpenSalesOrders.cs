using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetAdvantix.Helpers;
using AspNetAdvantix.Model;
using AspNetAdvantix.Models;
using AspNetAdvantix.ViewModel;
using SAPbobsCOM;

namespace AspNetAdvantix.Controllers
{
    [Route("api/controller")]
    [ApiController]

    public class OpenSO : ControllerBase
    {
        private readonly AdvantixContext _context;
        private readonly PickListDbContext _pickListDbContext;

        public OpenSO(AdvantixContext context, PickListDbContext pickListDbContext)
        {
            _context = context;
            _pickListDbContext = pickListDbContext;
        }

        [HttpGet("getOpenSalesOrder")]
        public async Task<ActionResult<IEnumerable<OpenSalesOrder>>> getOpenSalesOrder()
        {
            var openSOQuery = @"
                select
					'SO' 'Type',
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
                    (select z.Block from CRD1 z where z.CardCode = a.CardCode and z.Address = a.U_ITR_BRANCH) 'BPName',
                    a.U_ITR_BRANCH 'WhseBranch',
                    b.ItemCode,
                    b.Dscription,
                    b.OpenQty,
                    cast(0 as decimal(19,8)) 'QtyToPost',
					(select z.OnHand from OITW z where z.ItemCode = b.ItemCode and z.WhsCode = '01') 'InStock',
                    (select z.OnHand - z.IsCommited from OITW z where z.ItemCode = b.ItemCode and z.WhsCode = '01') 'Available',
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
                    and b.LineStatus = 'O'
                    and b.OpenQty > 0
                    
                union all
                
                select
					'ITR' 'Type',
                    a.DocNum,
                    a.DocDate,
                    'Consignment' 'SOType',
                    a.CardCode,
                    a.CardName,
                    (select z.Block from CRD1 z where z.CardCode = a.CardCode and z.Address = a.U_ITR_BRANCH) 'BPName',
                    a.U_ITR_BRANCH 'WhseBranch',
                    b.ItemCode,
                    b.Dscription,
                    b.OpenQty,
                    cast(0 as decimal(19,8)) 'QtyToPost',
					(select z.OnHand from OITW z where z.ItemCode = b.ItemCode and z.WhsCode = '01') 'InStock',
                    (select z.OnHand - z.IsCommited from OITW z where z.ItemCode = b.ItemCode and z.WhsCode = '01') 'Available',
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
                    OWTQ a
                    inner join WTQ1 b on a.DocEntry = b.DocEntry
                where
                    a.DocStatus = 'O'
                    and a.Series = 129
                    and a.CANCELED = 'N'
                    and b.LineStatus = 'O'
                    and b.OpenQty > 0";
            var openSO = await _context.OpenSalesOrder.FromSql(openSOQuery).ToListAsync();
            return openSO;
        }

        [HttpGet("getUpdatedSalesOrder")]
        public async Task<ActionResult<IEnumerable<PickedSalesOrder>>> getUpdatedSalesOrder()
        {
            var updatedQuery = @"
                select
					'SO' 'Type',
                    a.DocNum,
                    a.CardCode,
                    a.CardName,
                    (select z.Block from CRD1 z where z.CardCode = a.CardCode and z.Address = a.U_ITR_BRANCH) 'BPName',
                    case
                        when a.U_SO_TYPE = 'C'
                        then 'Consignment'
                        when a.U_SO_TYPE = 'O'
                        then 'Outright'
                    end 'SOType',
                    a.U_ITR_BRANCH 'WhseBranch',
                    b.ItemCode,
                    b.U_SO_PickedQty 'PickedQty',
                    cast(0 as decimal(19,8)) 'QtyToPost',
                    b.U_SO_PLNo 'PLNo',
                    DATEADD(dd, DATEDIFF(dd, 0, b.U_SO_PLDate), 0) 'PLDate',
                    b.U_SO_PLRemarks 'PLRemarks',
                    b.U_SO_PLUser 'PLUser',
                    b.Dscription,
                    (select z.OnHand from OITW z where z.ItemCode = b.ItemCode and z.WhsCode = '01') 'InStock',
                    b.WhsCode,
                    b.DocEntry,
                    b.LineNum,
                    b.ObjType
                from
                    ORDR a
                    inner join RDR1 b on a.DocEntry = b.DocEntry
                where
                    isnull(b.U_SO_PLNo, '') <> ''
                    and a.U_SO_Released = 'N'
                    and b.LineStatus = 'O'
                    
                union all
                
                select
					'ITR' 'Type',
                    a.DocNum,
                    a.CardCode,
                    a.CardName,
                    (select z.Block from CRD1 z where z.CardCode = a.CardCode and z.Address = a.U_ITR_BRANCH) 'BPName',
                    'Consignment' 'SOType',
                    a.U_ITR_BRANCH 'WhseBranch',
                    b.ItemCode,
                    b.U_SO_PickedQty 'PickedQty',
                    cast(0 as decimal(19,8)) 'QtyToPost',
                    b.U_SO_PLNo 'PLNo',
                    DATEADD(dd, DATEDIFF(dd, 0, b.U_SO_PLDate), 0) 'PLDate',
                    b.U_SO_PLRemarks 'PLRemarks',
                    b.U_SO_PLUser 'PLUser',
                    b.Dscription,
                    (select z.OnHand from OITW z where z.ItemCode = b.ItemCode and z.WhsCode = '01') 'InStock',
                    b.WhsCode,
                    b.DocEntry,
                    b.LineNum,
                    b.ObjType
                from
                    OWTQ a
                    inner join WTQ1 b on a.DocEntry = b.DocEntry
                where
                    isnull(b.U_SO_PLNo, '') <> ''
                    and a.U_SO_Released = 'N'
                    and b.LineStatus = 'O'";
            var updatedSO = await _context.PickedSalesOrder.FromSql(updatedQuery).ToListAsync();
            return updatedSO;
        }

        [HttpGet("getPicklistNo")]
        public async Task<ActionResult<PickListNo>> getPicklistNo()
        {
            var pickListNo = @"SELECT T0.Code, T0.U_PLNo 'PickListNum' FROM [@PICKLIST_SERIES] T0";
            var plNo = await _context.PickListNo.FromSql(pickListNo).FirstOrDefaultAsync();
            return plNo;
        }

        [HttpGet("getSalesOrderMonitoring")]
        public async Task<ActionResult<IEnumerable<SalesOrderMonitoring>>> getSalesOrderMonitoring()
        {
            var soMonitoringQuery = @"
                select
					'SO' 'Type',
                    a.DocEntry,
                    a.DocNum,
                    a.DocDate,
					DATEDIFF(d, a.DocDate, getdate()) 'DaysDue',
                    case
                    when a.U_SO_TYPE = 'C'
                        then 'Consignment'
                        when a.U_SO_TYPE = 'O'
                        then 'Outright'
                    end 'SOType',
                    (select z.Block from CRD1 z where z.CardCode = a.CardCode and z.Address = a.U_ITR_BRANCH) 'BPName'
                from
                    ORDR a
                where
                    a.DocStatus = 'O'

				union all

                select
					'ITR' 'Type',
                    a.DocEntry,
                    a.DocNum,
                    a.DocDate,
					DATEDIFF(d, a.DocDate, getdate()) 'DaysDue',
                    'Consignment' 'SOType',
                    (select z.Block from CRD1 z where z.CardCode = a.CardCode and z.Address = a.U_ITR_BRANCH) 'BPName'
                from
                    OWTQ a
                where
                    a.DocStatus = 'O'
					and a.Series = 129";
            var soMonitoring = await _context.SalesOrderMonitoring.FromSql(soMonitoringQuery).ToListAsync();
            return soMonitoring;
        }

        [HttpGet("getSalesOrderMonitoringDetails")]
        public async Task<ActionResult<IEnumerable<SalesOrderMonitoringDetails>>> getSalesOrderMonitoringDetails(int docnum)
        {
            var soMonitQuery = @"
                select
                    a.U_SO_DocNum 'DocNum',
                    a.U_SO_PLNo 'PLNo',
                    a.U_SO_ItemCode 'ItemCode',
                    a.U_SO_Qty 'Qty',
                    a.U_SO_Remarks 'Remarks',
                    a.U_SO_Date 'Date',
                    a.U_SO_UserName 'UserName'
                from
                    [@PICK_LIST_H] a
                where
                    a.U_SO_DocNum = {0}";
            var soMonitDetails = await _context.SalesOrderMonitoringDetails.FromSql(soMonitQuery, docnum).ToListAsync();
            return soMonitDetails;
        }

        [HttpPut("updateSalesOrder")]
        public ActionResult<ResultResponse> updateSalesOrder(List<UpdateSalesOrder> listUpdateSalesOrder, string userCode)
        {
            int pickNo = 0;
            int updateRows = 0;

            foreach (var obj in listUpdateSalesOrder)
            {
                string updateQuery = @"";
                switch (obj.ObjType)
                {
                    case "17":
                        updateQuery = @"
                            update 
                                a 
                            set 
                                a.U_SO_PickedQty = {2}, 
                                a.U_SO_PLRemarks = {3}, 
                                a.U_SO_PLNo = {4}, 
                                a.U_SO_PLDate = getdate(), 
                                a.U_SO_Released = 'Y' 
                            from 
                                RDR1 a 
                            where 
                                a.DocEntry = {0} 
                                and a.LineNum = {1}";
                        break;
                    case "1250000001":
                        updateQuery = @"
                            update 
                                a 
                            set 
                                a.U_SO_PickedQty = {2}, 
                                a.U_SO_PLRemarks = {3}, 
                                a.U_SO_PLNo = {4}, 
                                a.U_SO_PLDate = getdate(), 
                                a.U_SO_Released = 'Y' 
                            from 
                                WTQ1 a 
                            where 
                                a.DocEntry = {0} 
                                and a.LineNum = {1}";
                        break;
                }
                Console.WriteLine(updateQuery);
                var update = _context.Database.ExecuteSqlCommand(updateQuery, obj.DocEntry, obj.LineNum, obj.PickedQty, obj.Remarks, obj.PLNo);
                pickNo = obj.PLNo;
                updateRows += update;
                var docEntry = _pickListDbContext.PickListH.Last();
                int iDocEntry = 0;
                if (docEntry == null)
                {
                    iDocEntry = 1;
                }
                else
                {
                    iDocEntry = docEntry.DocEntry + 1;
                }
                Console.WriteLine(docEntry);
                var PickListH = new PickListH

                {
                    DocEntry = iDocEntry,
                    USoDocNum = obj.DocEntry,
                    USoPlno = obj.PLNo,
                    USoItemCode = obj.ItemCode,
                    USoQty = obj.PickedQty,
                    USoRemarks = obj.Remarks,
                    USoDate = DateTime.Now,
                    USoUserName = userCode,
                    UObjType = obj.ObjType

                };

                Console.WriteLine(PickListH.DocEntry);
                Console.WriteLine(PickListH.USoDocNum);

                _pickListDbContext.Add(PickListH);
                _pickListDbContext.SaveChanges();
            }

            var updateNo = @"update T0 set T0.U_PLNo = {0} FROM [@PICKLIST_SERIES] T0";
            var updatedNo = _context.Database.ExecuteSqlCommand(updateNo, pickNo + 1);
            if (updateRows == listUpdateSalesOrder.Count && listUpdateSalesOrder.Count != 0)
            {
                return new ResultResponse
                {
                    Result = "Success",
                    Message = "Successfuly updated Document and Line",
                    ResultData = ""
                };
            }
            else
            {
                return new ResultResponse
                {
                    Result = "Failed",
                    Message = "Failed to update Document and Line",
                    ResultData = ""
                };
            }

        }

        /* [HttpPost("createOPKL")]
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
        } */
    }
}