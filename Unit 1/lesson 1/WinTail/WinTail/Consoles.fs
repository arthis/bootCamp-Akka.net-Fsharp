module Consoles

    open System
    open Akka
    open Akka.Actor
    open Akka.FSharp

    let system = System.create "my-system" (Configuration.load())

    let ConsoleReaderActor (consoleWriter:ActorRef) =
        fun (mailbox:Actor<string option>) msg -> 
            let read = Console.ReadLine()
            match read with 
            | "exit" ->mailbox.Context.System.Shutdown()
            | _ -> 
                if String.IsNullOrEmpty(read) then consoleWriter.Tell(None)
                else consoleWriter.Tell(Some(read))
                
                mailbox.Self.Tell(Some("Continue"))
           

    let ConsoleWriterActor (mailbox:Actor<string option>) (msg:string option) =
    
        let writeToConsole color (m:string) =
            Console.ForegroundColor <- color
            Console.WriteLine(m)
            Console.ResetColor()
        match msg with
        | Some(message) -> 
            match message.Length % 2 with
            | 0 ->  writeToConsole ConsoleColor.Red "Your string had an even # of characters.\n"
            | _ -> writeToConsole ConsoleColor.Green "Your string had an odd # of characters.\n"
        | None -> writeToConsole ConsoleColor.DarkYellow "Please provide an input.\n"

