using Npgsql;

namespace GeospatialApi.Config
{
  public static class DbConnectionConfig
  {
    private const string ConnectionString =
      "Server=127.0.0.1;Port=5432;Database=GeoDatabase;User Id=postgres;Password=password;";

    public static IServiceCollection AddNpgSqlDataSource(this IServiceCollection services)
    {
      var dataSourceBuilder = new NpgsqlDataSourceBuilder(ConnectionString);
      dataSourceBuilder.UseNetTopologySuite();
      var dataSource = dataSourceBuilder.Build();

      services.AddSingleton(dataSource);

      return services;
    }
  }
}
