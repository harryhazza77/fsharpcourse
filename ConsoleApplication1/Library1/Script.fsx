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
let anotherp3 = {name="harry"; dob=DateTime.Now}

// value equality for free!
thisWillBeAPerson3BecauseItsTheLastOneDefined = anotherp3

type Person4 = {name:string; dob:DateTime; email:string option}

printfn "hello world %i" 42

printfn "hello world %s" "42"

let hellopartial = printfn "hello world %s"

let add1 (x:int) = x + 1

let tempInterfaceImpl = 
    {new IDisposable with
        member this.Dispose() = printfn "I'm disposed"}

tempInterfaceImpl.Dispose()

do 
    printfn "started"
    use tempInterfaceImpl = 
        {new IDisposable with
          member this.Dispose() = printfn "I'm disposed"}
    printfn "ended"

// this is 3 different implemenations of IPaymentMethod all with their own constructor!
type PaymentMethods = 
    | Cash
    | Cheque of int
    | Card of string * int   

let paymethod1 = Cash

// pattern matching (switch statement on drugs)
let printPaymentMethod meth =
    match meth with
    | Cash -> printfn "type of Cash"
    | Cheque cheqNo -> printf "type of cheque %i" cheqNo
    | Card (name,num) -> printf "type of cheque %s %A" name num

printPaymentMethod paymethod1

let list1 = [2;3;4;5]
let prepend = 1 :: list1
let append = [4] @ list1

let sumOfSquares =
    let sq x = x * x
    [1..100]
    |> List.map sq
    |> List.sum

let aTuple = 1,2


