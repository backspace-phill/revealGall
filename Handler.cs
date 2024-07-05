using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
namespace revealGall;
public class Handler
{
    public static IResult HandleRootRoute()
    {
        var htmlFiles = Directory.GetFiles(".", "*.html", SearchOption.AllDirectories);
        return htmlFiles.Length == 0
            ? Results.Extensions.Html("<h1>No HTML files found</h1>")
            : Results.Extensions.Html($"""
    <h1>Hello Root!</h1>
    <br>
    {htmlFiles.Select((content) => $"<a href={content}>{content}</a><iframe src={content} ></iframe>").Aggregate((a, b) => a + "<br>" + b)}
    """);
    }

    public static IResult HandleIndividualRoute(string route)
    {
        try
        {
            if (!route.EndsWith(".html"))
            {
                return Results.File(File.ReadAllBytes(route));
            }
            // Console.WriteLine(route);
            var htmlFileOfRoute = File.ReadAllText(route);
            var rootElement = new HtmlParser().ParseDocument(htmlFileOfRoute);
            var body = rootElement.Body;
            var linkTags = rootElement.QuerySelectorAll("link");
            var scriptTags = rootElement.QuerySelectorAll("script");
            var imageTags = rootElement.QuerySelectorAll("img");

            if (body is null)
            {
                return Results.NotFound("Body is Null");
            }
            RewriteScriptTags(rootElement, body, scriptTags, route);
            RewriteLinkTags(rootElement, body, linkTags, route);
            RewriteImageTags(rootElement, body, imageTags, route);
            return rootElement.FirstElementChild is null
                ? Results.NotFound("First Element Child is Null")
                : Results.Extensions.Html(rootElement.FirstElementChild.OuterHtml);
        }
        catch (Exception)
        {
            // Console.WriteLine(e.Message);
            return Results.NotFound("File Not Found");
        }
    }

    private static void RewriteLinkTags(IHtmlDocument rootElement, IHtmlElement body, IHtmlCollection<IElement> linkTags, string route)
    {
        foreach (var linkTag in linkTags)
        {
            var href = linkTag.GetAttribute("href");
            var stylesheet = linkTag.GetAttribute("rel");
            if (href is null || stylesheet != "stylesheet")
            {
                continue;
            }
            try
            {
                var dir = Path.GetDirectoryName(route);
                var path = Path.Combine(dir ?? ".", href);
                var textOfReferencedFile = File.ReadAllText(path);
                var replacementScriptTag = rootElement.CreateElement("style");
                replacementScriptTag.InnerHtml = textOfReferencedFile;
                body.InsertBefore(replacementScriptTag);
                linkTag.RemoveFromParent();
            }
            catch (System.Exception)
            {
                continue;
            }
        }
    }
    private static void RewriteScriptTags(IHtmlDocument rootElement, IHtmlElement body, IHtmlCollection<IElement> scriptTags, string route)
    {
        foreach (var scriptTag in scriptTags)
        {
            var source = scriptTag.GetAttribute("src");
            if (source is null)
            {
                continue;
            }

            try
            {
                var path = Path.Combine(Path.GetDirectoryName(route) ?? ".", source);
                var textOfReferencedFile = File.ReadAllText(path);
                var replacementScriptTag = rootElement.CreateElement("script");
                replacementScriptTag.InnerHtml = textOfReferencedFile;
                body.InsertBefore(replacementScriptTag);
                scriptTag.RemoveFromParent();
            }
            catch (System.Exception)
            {
                continue;
            }

        }
    }

    private static void RewriteImageTags(IHtmlDocument rootElement, IHtmlElement body, IHtmlCollection<IElement> scriptTags, string route)
    {
        foreach (var scriptTag in scriptTags)
        {
            var source = scriptTag.GetAttribute("src");
            if (source is null)
            {
                continue;
            }

            try
            {
                var path = Path.Combine(Path.GetDirectoryName(route) ?? ".", source);
                var bytesOfReferencedFile = File.ReadAllBytes(path);
                var image = Path.GetExtension(path).Replace(".", "");
                scriptTag.SetAttribute("src", $"data:image/{image};base64,{Convert.ToBase64String(bytesOfReferencedFile)}");
            }
            catch (Exception)
            {
                continue;
            }
        }
    }
}