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

    public class PickList : ControllerBase
    {
        private readonly AdvantixContext _context;
        public PickList(AdvantixContext context)
        {
            _context = context;
        }
        [HttpGet("getPicklist")]
        public async Task<ActionResult<IEnumerable<Picklist>>> getPicklist()
        {
            var pickListQuery = @"
                select
                    A.AbsEntry,
                    D.CardName,
                    C.ShipDate,
					C.ItemCode,
                    C.Dscription,
					B.RelQtty,
                    B.PickQtty,
                    C.Quantity 'OrderedQty',
					A.Remarks,
                    case
                        when D.U_SO_TYPE = 'O'
                        then 'Outright'
                        else 'Consignment'
                    end 'SOType',
                    case
                        when d.U_SO_TYPE = 'C'
                        then (select z.WhsName from OWHS z where z.WhsCode = d.U_ITR_BRANCH)
                        when d.U_SO_TYPE = 'O'
                        then 'ADVANTIX MARKETING WHSE'--(select z.WhsName from OWHS z where z.WhsCode = b.WhsCode)
                    end 'Branch'
                from
                    OPKL A
                    inner join PKL1 B on A.AbsEntry = B.AbsEntry
                    inner join RDR1 C on B.OrderEntry = C.DocEntry and B.OrderLine = C.LineNum and B.BaseObject = C.ObjType
                    inner join ORDR D on C.DocEntry = D.DocEntry
                where
                    A.Status = 'R'";
            var pickedDocs = await _context.Picklist.FromSql(pickListQuery).ToListAsync();
            return pickedDocs;
        }
    }
}