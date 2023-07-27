using GeospatialApi.Context;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace GeospatialApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class OsmController : ControllerBase
  {
    private readonly NpgsqlDataSource _dataSource;

    public OsmController(NpgsqlDataSource dataSource)
    {
      _dataSource = dataSource;
    }

    [HttpGet(Name = "RoadsInRange")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
    public async Task<IActionResult> Get(
      [FromQuery] double latitude = 48.148598,
      [FromQuery] double longitude = 17.107748,
      [FromQuery] double radiusInMeters = 100)
    {
      var context = new DbConnection(_dataSource);
      var result = await context.GetRoadsInRange(latitude, longitude, radiusInMeters);
      
      return Ok(result);
    }
  }
}
