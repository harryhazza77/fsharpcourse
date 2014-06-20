namespace FSharp.Http

open System.IO
open System.Net
open System.Threading
open System.Text

// ----------------------------------------------------------------------------
// Simple web server hosting static files
// ----------------------------------------------------------------------------

[<AutoOpen>]
module HttpExtensions = 

  type System.Net.HttpListener with
    member x.AsyncGetContext() = 
      Async.FromBeginEnd(x.BeginGetContext, x.EndGetContext)

  type System.Net.HttpListenerRequest with
    member request.InputString =
      use sr = new StreamReader(request.InputStream)
      sr.ReadToEnd()

  type System.Net.HttpListenerResponse with
    member response.Reply(s:string) = 
      let buffer = Encoding.UTF8.GetBytes(s)
      response.ContentLength64 <- int64 buffer.Length
      response.OutputStream.Write(buffer,0,buffer.Length)
      response.OutputStream.Close()
    member response.Reply(typ, buffer:byte[]) = 
      response.ContentLength64 <- int64 buffer.Length
      response.ContentType <- typ
      response.OutputStream.Write(buffer,0,buffer.Length)
      response.OutputStream.Close()

open FSharp.Data.Json
open FSharp.Data.Json.Extensions
open Microsoft.FSharp.Reflection

/// Simple HTTP server
type HttpServer private (url, root, controller) =

  let contentTypes = 
    dict [ ".css", "text/css"; ".html", "text/html"; ".js", "text/javascript";
           ".png", "image/png"; ".jpg", "image/jpg"; ".gif", "image/gif" ]
  let tokenSource = new CancellationTokenSource()
  
  let rec formatAnything (res:obj) =
    match res with 
    | :? string as s -> JsonValue.String(s)
    | :? int as n -> JsonValue.Number(decimal n)
    | :? decimal as d -> JsonValue.Number(d)
    | :? float as f -> JsonValue.Float(f)
    | :? bool as b -> JsonValue.Boolean(b)
    | _ when FSharpType.IsTuple(res.GetType()) ->
        let fields = FSharpValue.GetTupleFields(res)
        fields 
        |> Array.mapi (fun i v ->
          sprintf "Item%d" i, formatAnything v)
        |> Map.ofSeq
        |> JsonValue.Object
    | :? System.Collections.IEnumerable as en ->
        [| for v in en -> formatAnything v |]
        |> JsonValue.Array
    | _ -> JsonValue.Null

  let agent = MailboxProcessor<HttpListenerContext>.Start((fun inbox -> async { 
    while true do
      let! context = inbox.Receive()
      try
        match controller context with
        | Some res ->
            printfn "Controller returned: %A" res
            let response = formatAnything res
            context.Response.Reply(response.ToString())
        | None ->
            // Handle an ordinary file request
            let s = context.Request.Url.LocalPath 
            let file = root + (if s.EndsWith("/") then s + "index.html" else s)
            if File.Exists(file) then 
              let ext = Path.GetExtension(file).ToLower()
              let typ = contentTypes.[ext]
              context.Response.Reply(typ, File.ReadAllBytes(file))
            else 
              printfn "File not found: %s" file
              context.Response.Reply(sprintf "File not found: %s" file) 
      with e ->
        printfn "Error processing request: %A" e
        context.Response.Reply(sprintf "Error processing request: %A" e) }), tokenSource.Token)

  let server = async { 
    use listener = new HttpListener()
    listener.Prefixes.Add(url)
    listener.Start()
    while true do 
      let! context = listener.AsyncGetContext()
      agent.Post(context) }

  do Async.Start(server, cancellationToken = tokenSource.Token)

  /// Stops the HTTP server and releases the TCP connection
  member x.Stop() = tokenSource.Cancel()

  /// Starts new HTTP server on the specified URL. The specified
  /// function represents computation running inside the agent.
  static member Start(url, root, controller) = 
    new HttpServer(url, root, controller)
