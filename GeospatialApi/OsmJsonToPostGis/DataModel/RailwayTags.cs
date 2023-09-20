namespace OsmJsonToPostGis.DataModel
{
  internal class RailwayTags
  {
    public long Id { get; set; }

    public string? Electrified { get; set; }

    public string? Usage { get; set; }

    public string ToSql()
    {

      if (string.IsNullOrEmpty(Electrified)) Electrified = string.Empty;
      if (string.IsNullOrEmpty(Usage)) Usage = string.Empty;
      return $"({Id}, '{Electrified}', '{Usage}')";
    }
  }
}
