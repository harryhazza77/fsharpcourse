#r @"C:\Tomas\Projects\FSharp.Data\bin\FSharp.Data.dll"
open FSharp.Net

Http.Request("http://localhost:61416/api/dashboard/test")


// ----------------------------------------------------------------------------
// Scraping - get data from Tesco

#r "FSharp.Data.TypeProviders.dll"
#r "System.ServiceModel.dll"
#r "System.Runtime.Serialization.dll"
open System
open Microsoft.FSharp.Data

type Tesco = TypeProviders.WsdlService<"http://www.techfortesco.com/groceryapi/soapservice.asmx">
let client = Tesco.GetSOAPServiceSoap12()

let res, session =
  client.Login
    ( "tomas@tomasp.net", "fsharp", 
      "fjvRSQvEooAyLq3VhJgJ", "5E7B910E52079C9264CA" )


/// Get categories that actually contain some products
let rec getLeafCategories (categories:seq<Tesco.ServiceTypes.www.techfortesco.com.Category>) = seq {
  for c in categories do 
    if c.Children = null then yield c 
    else yield! getLeafCategories c.Children }

/// Get top level categories
let _, categories = client.ListProductCategories(session)

/// Get some sample products for every top-level category
let sampleData =
  let rnd = new Random()
  [| for c in categories ->
      printfn "%s" c.Name
      let prods =
        seq { for c in getLeafCategories [c] do
                match
                  ( try Some(snd (client.ListProductsByCategory(session, c, false)))
                    with _ -> None)
                  with 
                | Some prods ->
                    for p in prods do yield p 
                | _ -> () }
         |> Seq.take (rnd.Next(100) + 50) |> Array.ofSeq
      c.Name, prods |]

/// Save the data to a simple XML file
#r "System.Xml.Linq.dll"
open System.Xml.Linq

let xn s = XName.Get(s)
let xattr n v = XAttribute(xn n, v)

let xproduct (p:Tesco.ServiceTypes.www.techfortesco.com.Product) =
  XElement(xn "product", [|xattr "name" p.Name; xattr "price" p.Price|])

let xcat (cname, cprods) =
  XElement
    ( xn "category", 
      [| yield xattr "name" cname |> box; 
         for p in cprods do yield xproduct p |> box |] )
         
let doc = XDocument(XElement(xn "tesco", [| for c in sampleData -> xcat c |]))
doc.Save(__SOURCE_DIRECTORY__ + "\\tesco.xml")

// ----------------------------------------------------------------------------
// Generate some sample orders

let allprods = [| for _, ps in sampleData do yield! ps |]
let operators = [| "Hynek"; "Vilem"; "Jarmila" |]
let quants = [| 1.0; 1.0; 1.0; 1.0; 1.0; 1.0; 1.0; 1.0; 2.0; 2.0; 3.0; 3.0; 5.0; 9.0 |]

let rnd = System.Random()

let prod() = allprods.[rnd.Next(allprods.Length)]
let operator() = operators.[rnd.Next(operators.Length)]
let quant() = quants.[rnd.Next(quants.Length)]
let time() = DateTime.Now.AddMinutes(-1.0 * float (rnd.Next(2000))).ToString()

let numSales = 40
let maxProd = 15

let createItems() = 
  let sales = 
    [ for i in 1 .. rnd.Next(maxProd) -> 
        let p = prod() in p.Name, p.UnitPrice, quant() ]
  let payment = [ for _, p, q in sales -> p * q ] |> Seq.sum
  [| for name, price, quant in sales do
       yield XElement(xn "sale", [| xattr "name" name; xattr "price" price; xattr "quantity" quant |]) 
     yield XElement(xn "tender", [| xattr "total" payment |]) |]
        

let createSales() = 
  XElement
    ( xn "sale", 
      [| yield xattr "operator" (operator()) |> box
         yield xattr "time" (time()) |> box 
         for i in createItems() do yield box i |])

let history = 
  let data = 
    [| for n in 1 .. numSales ->
         XElement(xn "history", createSales()) |]
  XDocument(XElement(xn "history", data))

history.Save(__SOURCE_DIRECTORY__ + "\\history.xml")