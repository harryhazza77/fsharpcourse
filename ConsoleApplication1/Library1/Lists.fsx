let rec sumList list = 
    match list with
    | [] -> 0
    | head::tail ->
        head + (sumList tail)
        
let rec containsZero list = 
    match list with
    | [] -> false
    | 0::_ -> true
    | _::tail -> containsZero tail

containsZero [1..5] 
containsZero [0..5] 