module ConsoleReader

    open Akka
    open Akka.Actor
    open Akka.FSharp
    open System

    let printInstructions =
        Console.WriteLine("Please provide the URI of a log file on disk.\n");

    let GetAndValidateInput = fun () ->
        let message = Console.ReadLine()
        match message with
        | "exit" -> Exit
        | _ -> ConsoleReader.Input(message)

    let actor   =
        fun (mailbox:Actor<Messages.ConsoleReader>) msg -> 
            match msg with 
            | Start -> 
                printInstructions
                mailbox.Self.Tell(ContinueProcessing)
            | ConsoleReader.Input(m) -> 
                let fileValidator = mailbox.Context.ActorSelection("/user/validationActor")
                fileValidator.Tell(FileValidator.Input(m))
            | ContinueProcessing -> mailbox.Self.Tell <| GetAndValidateInput() 
            | Exit -> mailbox.Context.System.Shutdown()

