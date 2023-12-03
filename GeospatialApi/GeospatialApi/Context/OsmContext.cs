using NetTopologySuite.Geometries;
using Npgsql;

namespace GeospatialApi.Context
{
  public class OsmContext
  {
    private readonly NpgsqlDataSource _dataSource;

    public OsmContext(NpgsqlDataSource dataSource)
    {
      _dataSource = dataSource;
    }

    public async Task<IEnumerable<string>> GetRoadsInRange(double latitude, double longitude, double distance)
    {
      await using var connection = _dataSource.CreateConnection();
      connection.Open();

      await using var command = new NpgsqlCommand(@$"
        SELECT * 
        FROM slovakia_railways 
        WHERE ST_DWithin(geom, ST_MakePoint({Math.Round(longitude, 4)}, {Math.Round(latitude, 4)})::geography, {distance});",
        connection);

      var reader = await command.ExecuteReaderAsync();
      var column = (await reader.GetColumnSchemaAsync()).FirstOrDefault(c => c.ColumnName == "geom");

      if (column?.ColumnOrdinal == null)
      {
        return Enumerable.Empty<string>();
      }

      var result = new List<string>();
      while (await reader.ReadAsync())
      {
        result.Add(reader.GetFieldValue<Geometry>(column.ColumnOrdinal.Value).ToString());
      }
      return result.ToArray();
    }

    public async Task<IEnumerable<string>> GetRoadsInRangeGeoJson(double latitude, double longitude, double distance)
    {
      await using var connection = _dataSource.CreateConnection();
      connection.Open();

      await using var command = new NpgsqlCommand(@$"
        SELECT ST_AsGeoJSON(t.*) as geojson FROM
        (SELECT * 
        FROM slovakia_railways 
        WHERE ST_DWithin(geom, ST_MakePoint({Math.Round(longitude, 4)}, {Math.Round(latitude, 4)})::geography, {distance})) as t;",
        connection);

      var reader = await command.ExecuteReaderAsync();
      var column = (await reader.GetColumnSchemaAsync()).FirstOrDefault(c => c.ColumnName == "geojson");

      if (column?.ColumnOrdinal == null)
      {
        return Enumerable.Empty<string>();
      }

      var result = new List<string>();
      while (await reader.ReadAsync())
      {
        result.Add(reader.GetFieldValue<string>(column.ColumnOrdinal.Value));
      }
      return result.ToArray();
    }

    public async Task<string> GetShopsGridHeatMap(double latitude1, double longitude1, double latitude2, double longitude2, double gridWidth)
    {
      await using var connection = _dataSource.CreateConnection();
      connection.Open();

      await using var command = new NpgsqlCommand(@$"
        SELECT json_build_object('type', 'FeatureCollection', 'features', json_agg(ST_AsGeoJSON(t.*)::json)) as geojson
          FROM
            (SELECT COUNT(*), squares.geom
              FROM (SELECT ST_Transform(way, 4326) as geom
                    FROM planet_osm_point WHERE 
                    ST_Contains(ST_MakeEnvelope({longitude1}, {latitude1}, {longitude2}, {latitude2}, 4326), ST_Transform(ST_SetSRID(way, 3857), 4326))
                    AND shop <> 'null') as pts
              INNER JOIN
              ST_SquareGrid({gridWidth}, geom) AS squares
              ON ST_Intersects(pts.geom, squares.geom)
            GROUP BY squares.geom) as t",
        connection);

      var reader = await command.ExecuteReaderAsync();

      var column = (await reader.GetColumnSchemaAsync()).FirstOrDefault(c => c.ColumnName == "geojson");
      if (column?.ColumnOrdinal == null)
      {
        return string.Empty;
      }

      await reader.ReadAsync();
      return reader.GetFieldValue<string>(column.ColumnOrdinal.Value);
    }
  }
}
