module ConsoleReader

    open Akka
    open Akka.Actor
    open Akka.FSharp
    open System

    let actor  (fileValidator:ActorRef) =

        let printInstructions =
            Console.WriteLine("Please provide the URI of a log file on disk.\n");

        let GetAndValidateInput = fun () ->
            let message = Console.ReadLine()
            match message with
            | "exit" -> Exit
            | _ -> ConsoleReader.Input(message)

        fun (mailbox:Actor<Messages.ConsoleReader>) msg -> 
            match msg with 
            | Start -> 
                printInstructions
                mailbox.Self.Tell(ContinueProcessing)
            | ConsoleReader.Input(m) -> fileValidator.Tell(FileValidator.Input(m))
            | ContinueProcessing -> mailbox.Self.Tell <| GetAndValidateInput() 
            | Exit -> mailbox.Context.System.Shutdown()

