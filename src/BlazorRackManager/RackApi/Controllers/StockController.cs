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
	public class StockController : RackBaseController
	{

		public StockController(SqlContext sqlContext) : base(sqlContext)
		{

		}

		[Authorize(Policy = "MemberRequest")]
		[HttpPost("produitracks")]
		public async Task<List<StockView>> GetRacksProduit([FromBody] string referenceProduit)
		{
			try
			{
				LogInfo("Acces à GetRacksProduit");
				var racksProduit = await ContextSql.GetStocks(referenceProduit);
				return racksProduit;
			}
			catch (Exception ex)
			{
				LogError(ex, "Erreur GetRackInfo");
				throw;
			}
		}

		[Authorize(Policy = "MemberRequest")]
		[HttpGet("rackstock")]
		public async Task<List<StockView>> GetRackStock()
		{
			try
			{
				LogInfo("Acces à GetRackStock");
				var racksStock = await ContextSql.GetStocks();
				return racksStock;
			}
			catch (Exception ex)
			{
				LogError(ex, "Erreur GetRackInfo");
				throw;
			}
		}

	}
}
