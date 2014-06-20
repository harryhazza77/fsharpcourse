#r "System.Xml.Linq.dll"
#load "packages/FsLab.0.0.14-beta/FsLab.fsx"
open FSharp.Data
open FSharp.Charting

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
t.Rows
|> Seq.filter (fun r -> r.Survived && r.Survived && r.Sex = "male")
|> Seq.length

t.Rows
|> Seq.filter (fun r -> r.Survived && r.Survived && r.Sex = "female")
|> Seq.length
// Can you create a chart showing the survival rates for males/females?

// Hint: Here is how to create a simple chart!
Chart.Pie [ ("Good", 10); ("Bad", 1)]


type Stocks = CsvProvider<"msft.csv">

// TASK: Load the data from "aapl.csv" file

// TASK: Filter the data to get only prices from 2013.
// Calculate the average price, minimal and maximal
// price and the difference between minimal and maximal

// TASK: On how many days was the price above average?
// On how many days was the price below average?


// ----------------------------------------------------------------------------
// JSON type provider demo
// ----------------------------------------------------------------------------


// Get temperature in Madrid today!
Http.Request("http://api.openweathermap.org/data/2.5/weather?units=metric&q=Madrid")


// Prints all the capital cities in EU
// TASK: Can you print the temperature in the capital too?
for country in wb.Regions.``European Union``.Countries do
  printfn "%s" country.CapitalCity



// ----------------------------------------------------------------------------
// Reading RSS feeds using XML provider
// ----------------------------------------------------------------------------

type Rss = XmlProvider<"bbc.xml">
let news = Rss.Load("bbc.xml") // TODO: Use your favourite RSS feed's URL here!

// TASK: Explore the properties of the 'news' type
// and print the list of all news.

// TASK: Count how many of the items in the feed 
// have a title starting with "VIDEO"