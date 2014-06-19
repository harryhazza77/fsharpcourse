let  add1 x = x + 1
let p = printfn
[1..10] |> List.map add1

let logAdd1 x = 
    p "x is %i" x 

let genericLogger f x = 
    p "x is %i" x
    f x

let logAdd1b = genericLogger add1

10 |> logAdd1b