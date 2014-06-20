﻿open System
open System.IO
open System.Net
open System.Threading

// ------------------------------------------------------------------
// Read a stream into the memory and then return it as a string
// ------------------------------------------------------------------

let readToEnd (stream:Stream) = 
  // Allocate 1kb buffer for downloading dat
  let buffer = Array.zeroCreate 1024
  use output = new MemoryStream()
  let finished = ref false
  
  while not finished.Value do
    // Download one (at most) 1kb chunk and copy it
    let count = stream.Read(buffer, 0, 1024)
    output.Write(buffer, 0, count)
    finished := count > 0

  // Read all data into a string
  output.Seek(0L, SeekOrigin.Begin) |> ignore
  use sr = new StreamReader(output)
  sr.ReadToEnd()


// ------------------------------------------------------------------
// Download a web page using HttpWebRequest
// ------------------------------------------------------------------

/// Downlaod content of a web site using 'readToEnd'
let download url = 
  let request = HttpWebRequest.Create(Uri(url))
  use response = request.GetResponse()
  use stream = response.GetResponseStream()
  let res = readToEnd stream 
  res

let html = download "http://skillsmatter.com"
printfn "%s" html
