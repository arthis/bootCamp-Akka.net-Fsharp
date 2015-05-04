// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System
open Akka
open Akka.FSharp



[<EntryPoint>]
let main argv = 
    let system = System.create "my-system" (Configuration.load())

    let consoleWriterActor  = spawn system "consoleWriterActor" (actorOf2(ConsoleWriter.actor))

    let tailCoordinatorActor  = spawn system "tailCoordinatorActor" (actorOf2(TailCoordinator.actor))

    let fileValidatorActor  = 
        FileValidator.actor consoleWriterActor tailCoordinatorActor
        |> actorOf2
        |> spawn system "validatorActor" 

    let consoleReaderActor  = 
        actorOf2(ConsoleReader.actor(fileValidatorActor))
        |> spawn system "consoleReaderActor" 
    
    consoleReaderActor.Tell(Start)
    printfn "%A" argv
    

    
    system.AwaitTermination()
    Console.WriteLine("Actor System Terminated!!")
    Console.ReadLine() |> ignore
    0 // return an integer exit code
