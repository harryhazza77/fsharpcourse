#r "System.Xml.Linq.dll"
#load "packages/FsLab.0.0.14-beta/FsLab.fsx"
open Deedle
open FSharp.Data
open FSharp.Charting

// ----------------------------------------------------------------------------
// Loading financial data
// ----------------------------------------------------------------------------

let msft = Frame.ReadCsv(__SOURCE_DIRECTORY__ + "/msft.csv")
let opens = msft?Open

Stats.mean opens
opens - (Stats.mean opens)
opens - (Series.shift 1 opens)
(opens - (Series.shift 1 opens)) / opens * 100.0




// ----------------------------------------------------------------------------
// Loading Titanic data
// ----------------------------------------------------------------------------

let titanic = Frame.ReadCsv(__SOURCE_DIRECTORY__ + "/titanic.csv")

titanic
|> Frame.filterRows(fun k r -> r.GetAs<bool>("Survived"))
|> Frame.countRows

titanic
|> Frame.filterRows(fun k r -> r.GetAs<bool>("Survived") |> not)
|> Frame.countRows
