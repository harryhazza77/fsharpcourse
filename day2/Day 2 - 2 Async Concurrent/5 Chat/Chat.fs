open HttpUtils
open System
open System.IO
open System.Net

// ------------------------------------------------------------------

// The following listing shows global declarations 
// that are used when handling requests:

let root = __SOURCE_DIRECTORY__ + "\\"
let contentTypes = dict [ ".css", "text/css"; ".html", "text/html" ]

// Using these three values, the next listing implements a function that 
// asynchronously handles an incoming HTTP request

let handleRequest (context:HttpListenerContext) = async { 
    match context.Request.Url.LocalPath with 
    | "/post" -> 
        // TODO: Send message to the chat room
        let text = context.Request.InputString
        let name = context.Request.QueryString.["name"]
        printfn "%s says %s" name text
        context.Response.Reply("OK")
    | "/chat" -> 
        // TODO: Get messages from the chat room (asynchronously!)
        let text = "<li>First</li><li>Second</li>"
        context.Response.Reply(text)
    | "/users" ->
        // TODO: Get list of active users
        context.Response.Reply("User1, User2, User3")

    | s ->
        // Handle an ordinary file request
        let file = root + (if s = "/" then "chat.html" else s)
        if File.Exists(file) then 
          let ext = Path.GetExtension(file).ToLower()
          let typ = contentTypes.[ext]
          context.Response.Reply(typ, File.ReadAllBytes(file))
        else 
          context.Response.Reply(sprintf "File not found: %s" file) }

            
// When a request is received, the agent starts the asynchronous 
// workflow constructed by handleRequest without waiting until it completes:

let chatUrl = "http://localhost:8081/"
let chatServer = HttpAgent.Start(chatUrl, fun mbox -> async {
    while true do 
      let! ctx = mbox.Receive()
      ctx |> handleRequest |> Async.Start })

Console.WriteLine("Server running...")
System.Diagnostics.Process.Start(chatUrl) |> ignore
Console.ReadLine() |> ignore
chatServer.Stop()