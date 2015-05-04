module FileValidator

    open Akka
    open Akka.Actor
    open Akka.FSharp
    open System
    open System.IO

    
    let actor (consoleWriter:ActorRef) (tailCoordinatorActor: ActorRef) =

        let checkNull m =
            if String.IsNullOrEmpty(m) then Result.Failure( ConsoleWriter.NullError("No input received.") )
            else Result.Success (m)
        let IsFileUri m =
            if (File.Exists(m)) then
                Result.Success(m)
            else
                let failureMsg = sprintf "%s is not an existing URI on disk." m
                Result.Failure(ConsoleWriter.ValidationError(failureMsg))
        let finalizeMessage result =
            match  result with
            | Success(m) -> 
                let successMsg = sprintf "Starting processing for %s"  m
                Processing(m, successMsg)
            | Failure(f) -> f


        fun (mailbox:Actor<FileValidator>) (msg:FileValidator) -> 
            let validateInput =
                checkNull
                >=> IsFileUri
                >> finalizeMessage
            let react = fun m ->
                consoleWriter.Tell(m)
                match m with
                | Processing(f,m) -> tailCoordinatorActor.Tell(StartTail(f,consoleWriter))
                | NullError(r) -> mailbox.Sender().Tell(ContinueProcessing)
                | ValidationError(r) -> mailbox.Sender().Tell(ContinueProcessing)
            
            match msg with
            | FileValidator.Input(m) -> 
                m |> (validateInput >> react)
           

