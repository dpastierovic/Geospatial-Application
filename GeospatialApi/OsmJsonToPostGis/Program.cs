// See https://aka.ms/new-console-template for more information

using System.Runtime.CompilerServices;
using System.Text;
using GeospatialApi.Config;
using Newtonsoft.Json;
using OsmJsonToPostGis.DataModel;

using var connection = DbConnectionConfig.CreateNpgSqlDataSource();

var json = File.ReadAllText(@"Data/slovakia-railway.json");

var railwayData = JsonConvert.DeserializeObject<dynamic>(json);

var railwayObjects = new List<RailwayTags>();
foreach (var railGeometry in railwayData.elements)
{
  var tags = new RailwayTags
  {
    Id = railGeometry.id,
    Electrified = railGeometry?.tags?.electrified,
    Usage = railGeometry?.tags?.usage,
  };

  railwayObjects.Add(tags);
}

var createTableCmd = connection.CreateCommand("CREATE TABLE temp_railway_data (osm_id bigint, electrified text, usage text)");
createTableCmd.ExecuteNonQuery();
var sb = new StringBuilder("INSERT INTO temp_railway_data (osm_id, electrified, usage) VALUES ");
for (var i = 0; i < railwayObjects.Count; i++)
{
  sb.Append(railwayObjects[i].ToSql());
  if (i < railwayObjects.Count - 1)
  {
    sb.AppendLine(",");
  }
}

var insertDataCmd = connection.CreateCommand(sb.ToString());
insertDataCmd.ExecuteNonQuery();