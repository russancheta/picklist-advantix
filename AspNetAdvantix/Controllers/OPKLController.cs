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
        public Picklist(AdvantixContext context)
        {
            _context = context;
        }
        [HttpGet("getPicklist")]
        public async Task<ActionResult<IEnumerable<
    }
}