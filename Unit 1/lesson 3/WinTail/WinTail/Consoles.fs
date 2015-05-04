module Consoles

    open System
    open Akka
    open Akka.Actor
    open Akka.FSharp
    
    let system = System.create "my-system" (Configuration.load())

    type InputFailure = 
        | NullError of string
        | ValidationError of string
    
    type MessageReader =
        | Start 
        | Continue
        | Exit
        | Input of string

    type MessageValidator =
        | Input of string
        
    type MessageWriter =
        | InputSuccess of string
        | InputFailure  of InputFailure

    let ValidatorActor (consoleWriter:ActorRef) =
        let checkNullMessage m =
             if String.IsNullOrEmpty(m) then InputFailure(NullError("No input received."))
                else InputSuccess(m)

        fun (mailbox:Actor<MessageValidator>) msg -> 
            match msg with
            | Input(m) -> 
                m
                |> checkNullMessage
                |> consoleWriter.Tell 
              
            mailbox.Sender().Tell(Continue)  


    let ConsoleReaderActor  (consoleValidator:ActorRef) =


        let printInstructions =
            Console.WriteLine("Write whatever you want into the console!")
            Console.Write("Some lines will appear as")
            Console.ForegroundColor <- ConsoleColor.DarkRed
            Console.Write(" red ")
            Console.ResetColor()
            Console.Write(" and others will appear as")
            Console.ForegroundColor <- ConsoleColor.Green
            Console.Write(" green! ")
            Console.ResetColor()
            Console.WriteLine()
            Console.WriteLine()
            Console.WriteLine("Type 'exit' to quit this application at any time.\n")

        let GetAndValidateInput = fun () ->
            let message = Console.ReadLine()
            match message with
            | "exit" -> Exit
            | _ -> MessageReader.Input(message)

        fun (mailbox:Actor<MessageReader>) msg -> 
            match msg with 
            | Start -> 
                printInstructions
                mailbox.Self.Tell(Continue)
            | MessageReader.Input(m) -> consoleValidator.Tell(MessageValidator.Input(m))
            | Continue -> mailbox.Self.Tell <| GetAndValidateInput() 
            | Exit -> mailbox.Context.System.Shutdown()

    let ConsoleWriterActor (mailbox:Actor<MessageWriter>) (msg:MessageWriter) =
    
        let writeToConsole color (m:string) =
            Console.ForegroundColor <- color
            Console.WriteLine(m)
            Console.ResetColor()

        match msg with
        | InputSuccess (m) -> 
            match m.Length % 2 with
            | 0 ->  writeToConsole ConsoleColor.Red "Your string had an even # of characters.\n"
            | _ -> writeToConsole ConsoleColor.Green "Your string had an odd # of characters.\n"
        | InputFailure(failure) -> 
            match failure with
            | NullError(reason) | ValidationError(reason) -> writeToConsole ConsoleColor.DarkYellow "Please provide an input.\n"

