namespace FsWeb.Domain

open System

// ------------------------------------------------------------------
// Domain model 
// ------------------------------------------------------------------

type [<Measure>] GBP

type Name = string
type UnitPrice = decimal<GBP>
type Quantity = decimal
type Amount = decimal<GBP>

type Product = Product of Name * UnitPrice
type Category = Name * Product list

type TenderKind = Cash | Card

type SaleLineItem = 
  | SaleLineItem of Product * Quantity
  | TenderLineItem of TenderKind * Amount

type Operator = string
type Sale = Operator option * DateTime * SaleLineItem list
type History = Sale list

type AllSales = (Product * Quantity) list

// ------------------------------------------------------------------
// Helper functions
// ------------------------------------------------------------------

module Model =
  /// Takes a value representing the trade history and produces
  /// a list that contains items grouped by the operator. The
  /// list consists of pairs containing the operator (optional)
  /// and a nested list with all trades of that operator.
  let groupHistoryByOperator (history:History) =
    history 
    |> Seq.groupBy (fun (operator, time, items) -> operator)
    |> Seq.map (fun (k, v) -> k, List.ofSeq v)
    |> List.ofSeq

  /// Builds a new list which is the result of applying the 
  /// specified function (first argument) to all elements of
  /// the input list (second argument). The function can be 
  /// used with pipelining:
  ///
  ///   data |> Model.mapList (fun inp -> ...)
  ///
  let mapList f l = List.map f l

  /// Tests whether the specified list of products (second argument)
  /// contain a given product (first argument)
  let containsProduct product (list:Product list) = 
    Seq.exists ((=) product) list

  /// Returns a category (from the list of categories given as the
  /// second argument) that matches the specified predicate. The 
  /// predicate is called for all categories and when a matching
  /// one is found, it is returned. Otherwise the function fails.
  let findCategory predicate (list:Category list) =
    Seq.find predicate list

  /// Given a value representing the history, returns a list with individual
  /// sale items from the entire history (the list contains pairs with the
  /// product and purchased quantity)    
  let collectAllSales (history:History) : AllSales =
    history |> Seq.collect (fun (operator, time, items) ->
    items |> Seq.choose (fun item ->
      match item with 
      | SaleLineItem(prod, quantity) -> Some (prod, quantity)
      | _ -> None)) |> List.ofSeq

  /// Returns the length of a list (works on lists of any type)
  let length list = List.length list 

  /// Group the list containing all sales - takes all sales
  /// (Can be obtained using 'collectAllSales') and a function
  /// that specifies the "key" from a product. The result is 
  /// a list of groups where each group contains the group key
  /// and a list of elements in that group.
  let groupAllSalesBy (selector:_ -> 'TKey) (allSales:AllSales) =
    allSales 
    |> Seq.groupBy (fun (prod, _) -> selector prod)
    |> Seq.map (fun (k, v) -> k, List.ofSeq v)
    |> List.ofSeq

  /// Returns the sum of the elements of the list
  let inline sumList list = Seq.sum list

// ------------------------------------------------------------------
// Loading data from XML files
// ------------------------------------------------------------------

module Data =
  open FSharp.Data
  
  type TescoData = XmlProvider<"../Data/tesco.xml">
  type HistoryData = XmlProvider<"../Data/history.xml">

  /// Returns the location of a file containing data
  /// (This function behaves differently in F# interactive and in Web server)
  let file name = 
    System.IO.Path.Combine(__SOURCE_DIRECTORY__, "..\\Data\\", name)
  
  /// A list of all categories (and the products they contain)
  let categories : Category list = 
    let tesco = TescoData.Load(file "tesco.xml")
    [ for cat in tesco.GetCategories() ->
        let products = 
          [ for p in cat.GetProducts() -> Product(p.Name, p.Price * 1.0M<GBP>) ]
        cat.Name, products ]

  /// A list containing just the products 
  let products =
    [ for c in categories do yield! snd c ]

  /// History with past sale information
  let history : History =
    let history = HistoryData.Load(file "history.xml")
    [ for item in history.GetHistories() ->
        let items = 
          [ for sale in item.Sale.GetSales() do 
              let prod = products |> List.find (fun (Product(name, _)) -> name = sale.Name)
              yield SaleLineItem(prod, decimal sale.Quantity)
            yield TenderLineItem(Cash, item.Sale.Tender.Total * 1.0M<GBP>) ]
        Some(item.Sale.Operator), item.Sale.Time, items ]
