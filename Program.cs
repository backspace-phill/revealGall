using System.Net;
using System.Text.Json.Serialization;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using revealGall;

var builder = WebApplication.CreateSlimBuilder(args);

var port = 8080;
var ip = IPAddress.Any;
var directory = Directory.GetCurrentDirectory();


builder.Environment.ContentRootPath = directory;
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(ip, port);
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});
#if !DEBUG
builder.Logging.SetMinimumLevel(LogLevel.Critical);
Console.WriteLine($"IP: {ip}, Port: {port}");
Console.WriteLine($"http://{ip}:{port}");
#endif
var app = builder.Build();

var rootApi = app.MapGroup("/");

rootApi.MapGet("/", Handler.HandleRootRoute);
rootApi.MapGet("/{*route}", Handler.HandleIndividualRoute);



app.Run();

[JsonSerializable(typeof(string))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}

