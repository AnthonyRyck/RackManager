using AccessData;
using RackCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RackCore.EntityView;

namespace RackApi.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class HangarController : RackBaseController
	{
		public HangarController(SqlContext sqlContext) : base(sqlContext)
		{
		}

		[Authorize(Policy = "MemberRequest")]
        [HttpGet("rackempty")]
        public async Task<IEnumerable<Rack>> GetRacksEmpty()
        {
			try
			{
				LogInfo("GetRacksEmpty");
				var racksEmpty = await ContextSql.GetRackEmpty();
				return racksEmpty;
			}
			catch (Exception ex)
			{
				LogError(ex, "Erreur sur GetRacksEmpty");
				throw;
			}
        }

		[Authorize(Policy = "MemberRequest")]
		[HttpGet("rackoqp")]
		public async Task<IEnumerable<Rack>> GetRacksOccupes()
		{
			try
			{
				LogInfo("GetRacksOccupes");
				var racksFull = await ContextSql.GetRackFull();
				return racksFull;
			}
			catch (Exception ex)
			{
				LogError(ex, "Erreur sur GetRackOccupes");
				throw;
			}
		}

		[Authorize(Policy = "MemberRequest")]
		[HttpPost("rackinfo")]
		public async Task<HangarView> GetRackInfo([FromBody] int idRack)
		{
			try
			{
				LogInfo("GetRackInfo");
				var rackInfo = await ContextSql.GetHangar(idRack);
				return rackInfo;
			}
			catch (Exception ex)
			{
				LogError(ex, "Erreur GetRackInfo");
				throw;
			}
		}
	}
}
