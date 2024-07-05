using System.Text.Json.Serialization;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Scripting;
using Microsoft.AspNetCore.Mvc;



var builder = WebApplication.CreateSlimBuilder(args);


builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

var rootApi = app.MapGroup("/");

rootApi.MapGet("/", HandleRootRoute);
rootApi.MapGet("/{*route}", HandleIndividualRoute);

IResult HandleRootRoute()
{
    var htmlFiles = Directory.GetFiles(".", "*.html", SearchOption.AllDirectories);
    if (htmlFiles.Length == 0)
    {
        return Results.Extensions.Html("<h1>No HTML files found</h1>");
    }
    return Results.Extensions.Html($"""
    <h1>Hello Root!</h1>
    <br>
    {htmlFiles.Select((content) => $"<iframe src={content} ></iframe>").Aggregate((a, b) => a + "<br>" + b)}
    """);
}

IResult HandleIndividualRoute(string route)
{
    try
    {
        var htmlFileOfRoute = File.ReadAllText(route);
        var rootElement = new HtmlParser().ParseDocument(htmlFileOfRoute);
        var body = rootElement.Body;
        var linkTags = rootElement.QuerySelectorAll("link");

        if(body is null) {
            return Results.NotFound("Body is Null");
        }
        
        foreach (var linkTag in linkTags)
        {
            var href = linkTag.GetAttribute("href");
            var replacementScriptTag = rootElement.CreateElement("script");
            replacementScriptTag.InnerHtml = "console.log('Hello from a script tag')";
            body.InsertBefore(replacementScriptTag);
        }
        if(rootElement.FirstElementChild is null) {
            return Results.NotFound("First Element Child is Null");
        }
        return Results.Extensions.Html(rootElement.FirstElementChild.OuterHtml);
    }
    catch (Exception)
    {
        return Results.NotFound("File Not Found");
    }
}

app.Run();

[JsonSerializable(typeof(string))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}

