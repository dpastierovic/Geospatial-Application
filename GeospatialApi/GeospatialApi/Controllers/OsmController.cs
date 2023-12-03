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

    [HttpGet(Name = "ShopsHeatMap")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetShopsHeatMap(
      [FromQuery] double latPoint1 = 48.148598,
      [FromQuery] double lonPoint1 = 17.107748,
      [FromQuery] double latPoint2 = 49.148598,
      [FromQuery] double lonPoint2 = 18.107748,
      [FromQuery] double gridSquareInDeg = 0.05)
    {
      var result = await _context.GetShopsGridHeatMap(latPoint1, lonPoint1, latPoint2, lonPoint2, gridSquareInDeg);

      return Ok(result);
    }
  }
}