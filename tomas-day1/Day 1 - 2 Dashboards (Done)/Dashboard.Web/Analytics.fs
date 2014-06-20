module FsWeb.Domain.Analytics
open FsWeb.Domain

// ------------------------------------------------------------------
// Sales by operator
// ------------------------------------------------------------------

let operatorName (operator:Operator option) = 
  // TODO: Return the operator name or "(unknown)" 
  // if the operator name is not available
  match operator with
  | Some name -> name
  | _ -> "(Unknown)"

  // ""

let salesByOperator (history:History) = 
  history
  |> Model.groupHistoryByOperator 
  |> Model.mapList (fun (operator, sales) -> 
      operatorName operator, Seq.length sales)

// ------------------------------------------------------------------
// Count products in each category
// ------------------------------------------------------------------

let rec countProducts (products:Product list) =
  // TODO: Implement recursive function 'countProducts'
  // that counts the number of items in a given list of products
  // (The function below is similar, but it generates a 
  // new list - a list of counts instead of list of categories)
  match products with
  | [] -> 0
  | product::remainingProducts -> 
      let remainingCount = countProducts (remainingProducts)
      remainingCount + 1
//  0


let rec countProductsInCategories (categories:Category list) =
  match categories with
  | [] -> []
  | category::remainingCategories ->
      let name, products = category
      let count = countProducts products
      let remainingCounts = countProductsInCategories remainingCategories
      let result = name, count
      result::remainingCounts

// ------------------------------------------------------------------
// Count the number of purchased products by their category
// ------------------------------------------------------------------

let findProductCategory (product:Product) = 
  // TODO: Use functions 'Model.findCategory' and
  // 'Model.containsProduct' to get a category 
  // containing the given 'product'
  // (To make this run, just return the first one now...)
  Data.categories
  |> Model.findCategory (fun (name, products) ->
      products |> Model.containsProduct product)
  
  // Data.categories |> Seq.head

let purchasedByCategory (history:History) =
  // Obtain a list of all individual product sales from the history
  let allPurchased = 
    history |> Model.collectAllSales
    
  allPurchased
  // Group the sales by the category of a product
  |> Model.groupAllSalesBy findProductCategory

  // For every group, return the name of the category
  // paired with the number of products in the category
  |> Model.mapList (fun (category, products) -> 
      let name, _ = category
      name, Model.length products)

// ------------------------------------------------------------------
// Money spent by category
// ------------------------------------------------------------------

let moneySpentByCategory (history:History) =
  // Obtain a list of all individual product sales from the history
  let allPurchased = 
    history |> Model.collectAllSales

  // TODO: Count money spent by category - this is very similar to 
  // the 'purchasedByCategory' function, but instead of just counting
  // products, we also need to sum the price of all purchased products
  // (You can use 'Model.mapList' to turn list of products into a list
  // of prices and then use 'Model.sumList' to add them)
  allPurchased
  |> Model.groupAllSalesBy findProductCategory
  |> Model.mapList (fun (category, productsAndQuantities) -> 
      let name, _ = category
      let spent = 
        productsAndQuantities 
        |> Model.mapList (fun (product, quantity) ->
            let (Product(name, price)) = product
            price * quantity)
        |> Model.sumList
      name, spent)

//  [ ("Test", 1) ]
