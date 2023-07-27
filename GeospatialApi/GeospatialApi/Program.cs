using Microsoft.AspNetCore.Http.Json;
using NetTopologySuite.IO.Converters;
using System.Text.Json.Serialization;
using System.Text.Json;
using GeospatialApi.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure PostGis connection
builder.Services.AddNpgsqlDataSource();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Json converters for geoJson handling
builder.Services.Configure<JsonOptions>(options =>
  {
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.Converters.Add(new GeoJsonConverterFactory());
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
  }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(options => options.AllowAnyOrigin());
app.UseAuthorization();

app.MapControllers();

app.Run();


