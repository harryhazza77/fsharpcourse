type Temp = 
    | DegreesC of float
    | DegreesF of float

let toC input =
    match input with 
    | DegreesF f -> (f - 32.0) / 1.8 |> DegreesC
    | _ -> input

let isFever input =
    match input with 
    | DegreesC c -> c >= 38.0
    | DegreesF f -> f >= 101.0

DegreesC 100.0 |> isFever

// equivalent ..
isFever (DegreesC 100.0)
DegreesF 103.0 |> isFever // best
103.0 |> DegreesF |> isFever


