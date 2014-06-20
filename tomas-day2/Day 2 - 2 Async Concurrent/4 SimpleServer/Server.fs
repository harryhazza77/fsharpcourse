open HttpUtils
open System
open System.IO
open System.Net

// ------------------------------------------------------------------
// Simple web server

let url = "http://localhost:8082/"
let server = HttpAgent.Start(url, fun server -> async {
    while true do 
        let! ctx = server.Receive()
        ctx.Response.Reply("Hello world!") })

        
Console.WriteLine("Server running...")
System.Diagnostics.Process.Start(url) |> ignore
Console.ReadLine() |> ignore

// Stop the HTTP server and release the port 8082
server.Stop()
