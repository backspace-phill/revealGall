using System.Text.Json.Serialization;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using revealGall;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

var rootApi = app.MapGroup("/");

rootApi.MapGet("/", Handler.HandleRootRoute);
rootApi.MapGet("/{*route}", Handler.HandleIndividualRoute);



app.Run();

[JsonSerializable(typeof(string))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}

