#load "HttpUtils.fs"

open System
open System.Net
open System.Threading
open System.Collections.Generic
open FSharp.Control

let root = "http://www.guardian.co.uk"

// ----------------------------------------------------------------------------
// Completely simple proxy 

let proxy1 = HttpServer.Start("http://localhost:8082/", fun ctx -> async {
  let url = root + ctx.Request.Url.PathAndQuery
  let wc = new WebClient()
  let! html = wc.AsyncDownloadString(Uri(url))
  let html = html.Replace(root, "http://localhost:8082")
  let html = html.Replace("<title>", "<title>Proxy: ")
  do! ctx.Response.AsyncReply(html) })

proxy1.Stop()

// ------------------------------------------------------------------
// TASK #1: The above example is nice, but not efficient!
// It should forward data as they arrive, e.g. in 1kb blocks
// This can be done by creating HttpWebRequest:
// 
//  let request = HttpWebRequest.Create(url)
//  use! response = request.AsyncGetResponse()
//  use stream = response.GetResponseStream()
//
// and then using 'stream.AsyncRead' to read 1kb blocks and
// 'ctx.Response.OutputStream.AsyncWrite' to write them to the output.


// ------------------------------------------------------------------
// TASK #2: As a second step, add caching to the proxy using 
// the following object that exposes methods for caching
// (it is implemented using agents - we will cover them later)
// For a commented version, see: http://fssnip.net/8V

// Internal type used by the CachingAgent object only
type internal CachingMessage =
  | Add of string * string
  | Get of string * AsyncReplyChannel<option<string>>
  | Clear

type CachingAgent() =
  let caching = MailboxProcessor.Start(fun agent -> async {
    let table = Dictionary<string, string>()
    while true do
      let! msg = agent.Receive()
      match msg with
      | Add(url, html) -> 
          table.Add(url, html)
      | Get(url, repl) -> 
          if table.ContainsKey(url) then
            repl.Reply(Some table.[url])
          else
            repl.Reply(None) 
      | Clear ->
          table.Clear() })

  member x.Clear() = caching.Post(Clear)
  member x.Add(key, value) = caching.Post(Add(key, value))
  member x.AsyncGet(key) = caching.PostAndAsyncReply(fun ch -> Get(key, ch))

// After creating an instance of the agent, you can use
// the following to add pages to the cache:
//
//   agent.Add(url, html)
//
// .. and in an asynchronous workflow, you can retreive
// result (which is an option value) as follows:
//
//   let! optHtml = agent.AsyncGet(url)
//
