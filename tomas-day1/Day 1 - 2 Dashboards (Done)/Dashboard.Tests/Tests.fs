// ------------------------------------------------------------------
// This file can be used both as part of a compiled project
// (to run unit tests), but it can also be used in F# interactive - 
// the block of code in #if INTERACTIVE will be executed in F# 
// Interactive, but not when compiled, so we can setup the interactive
// environment and load all that we need
// ------------------------------------------------------------------

#if INTERACTIVE
#r "../lib/FSharp.Data.dll"
#r "../lib/FsCheck.dll"
#r "../lib/nunit.framework.dll"
#r "../Dashboard.Web/bin/Debug/Dashboard.Web.exe"
#r "System.Xml.Linq.dll"
#else
module SalesTests
#endif

open FsCheck
open FsWeb.Domain
open NUnit.Framework

// ------------------------------------------------------------------
// After executing everything above this line, you can run the body
// of individual test functions in F# interactive!
// ------------------------------------------------------------------

// ----------------------------------------------------------------
// Tests for: Sales by operator

let [<Test>] ``Empty operator name is (unknown)``() =
  let actual = Analytics.operatorName None
  Assert.AreEqual("(Unknown)", actual)

let [<Test>] ``Some "Tomas" returns "Tomas" as the operator``() =
  let actual = Analytics.operatorName (Some "Tomas")
  Assert.AreEqual("Tomas", actual)

let [<Test>] ``Sales by operator works on the sample history data``() =
  let actual = Analytics.salesByOperator Data.history
  let expected = [("Hynek", 11); ("Vilem", 15); ("Jarmila", 14)]
  Assert.AreEqual(expected, actual)

// ----------------------------------------------------------------
// Tests for: Count products in each category

let [<Test>] ``Total number of sample products is 770``() =
  Assert.AreEqual(770, Analytics.countProducts Data.products)

let [<Test>] ``Number of fresh food products is 53``() =
  let counts = dict (Analytics.countProductsInCategories Data.categories)
  Assert.AreEqual(53, counts.["Fresh Food"])

// ------------------------------------------------------------------
// Tests for: Count the number of purchased products by their category

let [<Test>] ``Comfort Vaporesse is in the Household category``() =
  let name, _ = Analytics.findProductCategory (Product("Comfort Vaporesse Blue 1 Litre", 1.0M<GBP>))
  Assert.AreEqual("Household", name)

let [<Test>] ``Total 27 products are purchased in Frozen Food``() =
  let purchases = dict (Analytics.purchasedByCategory Data.history)
  Assert.AreEqual(27, purchases.["Frozen Food"])


// ------------------------------------------------------------------
// Tests for: Money spent by category

let [<Test>] ``Total 130.80 GBP is spent on Frozen Food``() =
  let spends = dict (Analytics.moneySpentByCategory Data.history)
  Assert.AreEqual(130.800M<GBP>, spends.["Frozen Food"])


// ------------------------------------------------------------------
// Now, let's see how we can add a couple of more clever tests
// using FsCheck - this is a library that tests your code with
// random inputs to discover subtle errors.
// ------------------------------------------------------------------

let [<Test>] ``Non-empty operator name is the name of the operator``() =
  Check.Quick(fun (name:string) -> 
    let actual = Analytics.operatorName (Some name)
    Assert.AreEqual(name, actual) )

let [<Test>] ``Total number of random products matches``() =
  Check.Quick(fun (names:string[]) -> 
    let products = 
      names 
      |> Seq.map (fun name -> Product(name, 10.0M<GBP>))
      |> List.ofSeq
    Assert.AreEqual(names.Length, Analytics.countProducts products) )

// ------------------------------------------------------------------
// Simple console runner
// ------------------------------------------------------------------
do
  printfn "Running tests in console..."
  ``Empty operator name is (unknown)``() 
  ``Non-empty operator name is the name of the operator``()
  ``Sales by operator works on the sample history data``()
  ``Total number of sample products is 770``()
  ``Number of fresh food products is 53``()
  ``Comfort Vaporesse is in the Household category``()
  ``Total 27 products are purchased in Frozen Food``()
  ``Total 130.80 GBP is spent on Frozen Food``()