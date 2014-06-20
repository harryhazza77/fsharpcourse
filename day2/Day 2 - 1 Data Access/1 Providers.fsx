#r "System.Xml.Linq.dll"
#load "packages/FsLab.0.0.14-beta/FsLab.fsx"
open FSharp.Data
open FSharp.Charting
open System.Linq

// ----------------------------------------------------------------------------
// Type provider introduction - WorldBank and R  
// ----------------------------------------------------------------------------

let wb = WorldBankData.GetDataContext()

wb.Countries.``United States``.Indicators.``Population (Total)``
|> Chart.Line

//wb.Topics.``Science & Technology``.Indicators.``Bank cost to income ratio (%)``
//|> Chart.Line


// ----------------------------------------------------------------------------
// Infering schema for CSV files
// ----------------------------------------------------------------------------

type Titanic = CsvProvider<"titanic.csv">
let t = Titanic.Load("titanic.csv")

// Print all names 
for p in t.Rows do 
  printfn " - %s" p.Name

// Count how many people survived
t.Rows
|> Seq.filter (fun r -> r.Survived)
|> Seq.length

// TASK: Count how many Male & Female passangers survived and died
let survivedMales = 
    t.Rows
    |> Seq.filter (fun r -> r.Survived && r.Sex = "male")
    |> Seq.length

let survidedFemales =
    t.Rows
    |> Seq.filter (fun r -> r.Survived && r.Sex = "female")
    |> Seq.length

let deadMales = 
    t.Rows
    |> Seq.filter (fun r -> not r.Survived && r.Sex = "male")
    |> Seq.length

let deadFemales =
    t.Rows
    |> Seq.filter (fun r -> not r.Survived && r.Sex = "female")
    |> Seq.length

let maleRatio = float survivedMales / float deadMales 
let femaleRatio = float survidedFemales / float deadFemales
// Can you create a chart showing the survival rates for males/females?

// Hint: Here is how to create a simple chart!
Chart.Bar [ ("Males", maleRatio); ("Females", femaleRatio)]

type Stocks = CsvProvider<"msft.csv">

let aapl = Stocks.Load("aapl.csv")
// TASK: Load the data from "aapl.csv" file

// TASK: Filter the data to get only prices from 2013.

let ``2013`` =
    aapl.Rows
        |> Seq.filter (fun r -> r.Date.Year = 2013)


// Calculate the average price, minimal and maximal
// price and the difference between minimal and maximal

// TASK: On how many days was the price above average?
// On how many days was the price below average?


// ----------------------------------------------------------------------------
// JSON type provider demo
// ----------------------------------------------------------------------------

// Get temperature in Madrid today!
type weather = JsonProvider<"http://api.openweathermap.org/data/2.5/weather?units=metric&q=Madrid">

let url = "http://api.openweathermap.org/data/2.5/weather?units=metric&q="
let getTempFromCity city = 
    try
        let temp = weather.Load(url + city).Main.Temp
        Some(city, temp)
    with 
        | ex -> None
// Prints all the capital cities in EU

wb.Regions.``European Union``.Countries
    |> Seq.map (fun c ->  getTempFromCity c.CapitalCity)
    |> Seq.toList

wb.Regions.Africa.Countries.Where(fun a -> a.CapitalCity.Length > 10)
   
//for country in wb.Regions.``European Union``.Countries do
 // printfn "%s %f" country.CapitalCity getTempFromCity

// ----------------------------------------------------------------------------
// Reading RSS feeds using XML provider
// ----------------------------------------------------------------------------

type Rss = XmlProvider<"bbc.xml">
let news = Rss.Load("bbc.xml") // TODO: Use your favourite RSS feed's URL here!

// TASK: Explore the properties of the 'news' type
// and print the list of all news.

// TASK: Count how many of the items in the feed 
// have a title starting with "VIDEO"