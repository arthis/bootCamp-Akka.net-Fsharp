module TailCoordinator

    open System
    open Akka
    open Akka.Actor
    open Akka.FSharp

     let actor  =
        fun (mailbox:Actor<Messages.TailCoordinator>) msg -> 

            let strategy (error:Exception) =   
                match error with
                | :? ArithmeticException -> Directive.Resume //Maybe we consider ArithmeticException to not be application critical so we just ignore the error and keep going.
                | :? NotSupportedException -> Directive.Stop //Error that we cannot recover from, stop <thead></thead> failing actor
                | _ -> Directive.Restart ////In all other cases, just restart the failing actor
            
            match msg with
            | StartTail(fileName,reporterActor) -> 
                // here we are creating our first parent/child relationship!
                // the TailActor instance created here is a child
                // of this instance of TailCoordinatorActor
                [SpawnOption.SupervisorStrategy (Strategy.OneForOne (strategy,10,TimeSpan(0,0,30) ) )]
                |> spawnOpt mailbox.Context "TailActor"  (Tail.actor reporterActor fileName) 
                |> ignore
            | StopTail(fileName) -> ()
            