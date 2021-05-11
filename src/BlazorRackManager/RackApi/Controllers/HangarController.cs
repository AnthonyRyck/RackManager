using AccessData;
using RackCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackApi.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class HangarController : Controller
	{
		private SqlContext SqlContext;


		public HangarController(SqlContext sqlContext)
		{
			SqlContext = sqlContext;
		}


        [Authorize(Policy = "RequestAdmin")]
        [HttpGet("rackempty")]
        public async Task<IEnumerable<Rack>> GetRacksEmpty()
        {
			var racksEmpty = await SqlContext.GetRackEmpty();
			return racksEmpty;
        }

		[Authorize(Policy = "RequestAdmin")]
		[HttpGet("rackoqp")]
		public async Task<IEnumerable<Rack>> GetRacksOccupes()
		{
			var racksFull = await SqlContext.GetRackFull();
			return racksFull;
		}

	}
}
