using GeospatialApi.Context;
using Microsoft.AspNetCore.Mvc;

namespace GeospatialApi.Controllers
{
  [ApiController]
  [Route("api/[controller]/[action]")]
  public class OsmController : ControllerBase
  {
    private readonly OsmContext _context;

    public OsmController(OsmContext context)
    {
      _context = context;
    }

    [HttpGet(Name = "RailroadsInRange")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
    public async Task<IActionResult> Get(
      [FromQuery] double latitude = 48.148598,
      [FromQuery] double longitude = 17.107748,
      [FromQuery] double radiusInMeters = 10000)
    {
      var result = await _context.GetRoadsInRange(latitude, longitude, radiusInMeters);

      return Ok(result);
    }

    [HttpGet(Name = "RailroadsInRangeGeoJson")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
    public async Task<IActionResult> GetGeoJson(
      [FromQuery] double latitude = 48.148598,
      [FromQuery] double longitude = 17.107748,
      [FromQuery] double radiusInMeters = 10000)
    {
      var result = await _context.GetRoadsInRangeGeoJson(latitude, longitude, radiusInMeters);

      return Ok(result);
    }
  }
}