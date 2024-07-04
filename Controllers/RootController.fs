namespace revealGall.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open System.IO

[<ApiController>]
[<Route("")>]
type RootController(logger: ILogger<RootController>) =
    inherit ControllerBase()

    [<HttpGet>]
    [<Route("/")>]
    member _.GetRoot() =
        let mutable response = new ContentResult()
        let files = Directory.GetFiles "."
        let fileAndContents = files |> Seq.map (fun file -> file, System.IO.File.ReadAllText(file))
        response.ContentType <- "text/html"
        response.Content <- "<h1>Hello World</h1>" + "RootController\n" + String.Join("\n", fileAndContents |> Seq.map (fun (file, content) -> file + " " + content + "\n"))
        response

    [<HttpGet>]
    [<Route("/{*path}")>]
    member _.GetPath(path: string) =
        let mutable response = new ContentResult()
        response.ContentType <- "text/html"
        response.Content <- "<h1>Hello World</h1>" + "RootController" + path
        response
