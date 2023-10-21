using Karpinski_XY.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDatabase(builder.Configuration)
    .AddIdentity()
    .AddJWTAuthentication(builder.Services.GetApplicationSettings(builder.Configuration))
    .AddSmtpSettings(builder.Configuration)
    .AddAutoMapperConfiguration()
    .AddLogging()
    .AddSerilog()
    .AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Karpinski", Version="v1"});
        c.CustomSchemaIds(Karpinski_XY.Infrastructure.Extensions.ServiceCollectionExtensions.SchemaSuffixStrategy);
    })
    .AddApplicationServices();

builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint()
    .UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Karpinski");
        c.RoutePrefix = String.Empty;
    });
}

app.UseRouting();

app.UseCors(options => options
.AllowAnyOrigin()
.AllowAnyMethod()
.AllowAnyHeader());

app.UseHttpMethodOverride();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
    RequestPath = new PathString("/Resources")
});

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.ApplyMigrations();
app.Run();