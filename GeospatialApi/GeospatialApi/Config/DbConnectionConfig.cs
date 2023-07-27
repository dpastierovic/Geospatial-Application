using Npgsql;

namespace GeospatialApi.Config
{
  public static class DbConnectionConfig
  {
    public static IServiceCollection AddNpgsqlDataSource(this IServiceCollection services)
    {
      var dataSourceBuilder = new NpgsqlDataSourceBuilder("Server=127.0.0.1;Port=5432;Database=GeoDatabase;User Id=postgres;Password=password;");
      dataSourceBuilder.UseNetTopologySuite();
      var dataSource = dataSourceBuilder.Build();
      services.AddSingleton(dataSource);
      return services;
    }
  }
}
