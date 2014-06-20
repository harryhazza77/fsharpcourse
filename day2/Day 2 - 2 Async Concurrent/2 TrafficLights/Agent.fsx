type Agent<'T> = MailboxProcessor<'T>

type ChatMessage = 
    | Send of userName:string * message:string
    | PrintHtml

let chatAgent = Agent.Start(fun inbox ->
    let rec loop msgs = async {
        let! cmd = inbox.Receive()
        match cmd with
        | Send (u,m) ->
            let newMsgs = (u,m)::msgs
            return! loop newMsgs
        | PrintHtml -> 
            printf "<ul>"
            msgs 
                |> Seq.iter (fun (u,m) -> printf "<li><strong>%s</strong>%s</li>" u m)
            printf "</ul>"
            return! loop msgs
    }
    loop [] )

chatAgent.Post(Send("Tomas", "Hello People"))
chatAgent.Post(Send("Scott", "Hello back"))
chatAgent.Post(PrintHtml)


let statsAgent = Agent.Start(fun inbox -> 
    let rec loop sum count = async {
        let! value = inbox.Receive()
        printfn "Avg = %f" (sum/count)
        return! loop (sum + value) (count + 1.0) }
        
    loop 0.0 0.0 )

let rnd = new System.Random()
for i in 1.0 .. 100000.0 do
    statsAgent.Post(rnd.NextDouble())
