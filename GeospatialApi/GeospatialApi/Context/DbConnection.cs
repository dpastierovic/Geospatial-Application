using NetTopologySuite.Geometries;
using Npgsql;

namespace GeospatialApi.Context
{
  public class DbConnection
  {
    private readonly NpgsqlDataSource _dataSource;

    public DbConnection(NpgsqlDataSource dataSource)
    {
      _dataSource = dataSource;
    }

    public async Task<IEnumerable<string>> GetRoadsInRange(double latitude, double longitude, double distance)
    {
      await using var connection = _dataSource.CreateConnection();
      connection.Open();
      await using var command = new NpgsqlCommand(
        $"SELECT ST_Transform(way, 4326) as geometry FROM planet_osm_roads WHERE ST_Distance(way, " +
        $"ST_Transform('SRID=4326;POINT({Math.Round(longitude, 4)} {Math.Round(latitude, 4)})'::geometry, 3857)) < {distance} AND railway = 'rail';",
        connection);

      var reader = await command.ExecuteReaderAsync();
      var result = new List<string>();
      while (await reader.ReadAsync())
      {
        result.Add(reader.GetFieldValue<LineString>(0).ToString());
      }
      return result.ToArray();
    }
  }
}
