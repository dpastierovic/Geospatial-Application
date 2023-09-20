using Npgsql;

namespace GeospatialApi.Config
{
  public static class DbConnectionConfig
  {
    public static IServiceCollection AddNpgSqlDataSource(this IServiceCollection services)
    {
      var dataSource = CreateNpgSqlDataSource();
      services.AddSingleton(dataSource);
      return services;
    }

    public static NpgsqlDataSource CreateNpgSqlDataSource()
    {
      var dataSourceBuilder = new NpgsqlDataSourceBuilder("Server=127.0.0.1;Port=5432;Database=GeoDatabase;User Id=postgres;Password=password;");
      dataSourceBuilder.UseNetTopologySuite();
      return dataSourceBuilder.Build();
    }
  }
}
