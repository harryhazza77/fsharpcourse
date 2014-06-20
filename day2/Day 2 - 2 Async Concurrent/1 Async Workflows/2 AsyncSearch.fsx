#r "HtmlAgilityPack.dll"
#load "fssnip.fs"

open System
open System.Net
open System.Threading
open HtmlAgilityPack

// ----------------------------------------------------------------------------
// Helper functions for extracting stuff from HTML
// ----------------------------------------------------------------------------

/// Extract the <title> of the web page
let getTitle (doc:HtmlDocument) =
  try
    let title = doc.DocumentNode.SelectSingleNode("//title")
    if title <> null then title.InnerText.Trim() else "Untitled"
  with _ -> "Untitled"


// ------------------------------------------------------------------
// Downloading F# snippets
// ------------------------------------------------------------------

/// Downloads page and returns the title and size in bytes
let getTitleAndLength url = async {
  let wc = new WebClient()
  let! html = wc.AsyncDownloadString(Uri(url))
  let doc = HtmlDocument()
  doc.LoadHtml(html)
  return getTitle doc, html.Length }


// ------------------------------------------------------------------
// Download snippets in parallel using a thread for every snippet (!!)
// ------------------------------------------------------------------

let processSnippet index = async {
  let url = sprintf "http://fssnip.net/%s" (FsSnip.createId index)
  try
    let! title, length = getTitleAndLength url
    printfn "%s %s" url System.Threading.Thread.CurrentThread.Name
    return Some(title, length)
//    printfn "%s: %s (Size=%d)" url title length
  with e ->
    return None }
//    printfn "%s: Failed!" url }

[0..40] 
    |> Seq.map processSnippet
    |> Async.Parallel
    |> Async.RunSynchronously
    |> Seq.choose (fun x -> x)
    |> Seq.maxBy (fun (a,b) -> b)
   
//  let thr = Thread(fun () -> processSnippet i)
//  thr.Start()

// ------------------------------------------------------------------
// TASK #1: Modify the above code to use Async.Parallel instead of
// starting new Thread for every snippet.

// ------------------------------------------------------------------
// TASK #2: Change the code that processes the snippet to return
// 'Option<string * int>'. When the download fails, it should
// return 'None', otherwise, it should return 'Some' with the
// downloaded information.

// ------------------------------------------------------------------
// TASK #3 (BONUS): Use the result of the previous together with 
// Async.Parallel. This gives you an array of option values. Then use 
// Seq.xyz functions to filter out None values and then find the 
// longest and the shortest snippet.
