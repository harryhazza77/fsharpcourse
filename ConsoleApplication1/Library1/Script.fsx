// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "Library1.fs"
open Library1
open System

type Person(name:string, dob:DateTime) = 
    member this.name = name
    member this.dob = dob
    member this.Age() =
        (DateTime.Now - this.dob).TotalDays / 365.0

// a record
type Person2 = {name:string; dob:DateTime}
//    with
//    member this.Age() = 
//        (DateTime.Now - this.dob).TotalDays / 365.0

type Person3 = {name:string; dob:DateTime}

let p2 = {Person2.name="harry"; dob=DateTime.Now}
let thisWillBeAPerson3BecauseItsTheLastOneDefined = {name="harry"; dob=DateTime.Now}

// Define your library scripting code here
printfn "hello world %i" 42

printfn "hello world %s" "42"

let hellopartial = printfn "hello world %s"

let add1 (x:int) = x + 1

