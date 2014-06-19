let add x y = x + y
let multiply x y = x * y

// partials
let add2 = add 2
let double = multiply 2

[1..10]
|> List.map add2
|> List.map double

// more common way which means you don't need to define lots of partials
[1..10]
|> List.map (add 2)
|> List.map (multiply 2)

// only loop the list once
[1..10]
|> List.map (add 2 >> multiply 2)


let replace oldStr newStr (s:string) = 
    s.Replace(oldValue=oldStr, newValue=newStr)

let startsWith lookFor (s:string) = 
    s.StartsWith(lookFor)

let result = 
    "hello"
    |> replace "h" "j"
    |> startsWith "j"

["the";"quick";"brown";"fox"]
    |> List.filter (startsWith "f")