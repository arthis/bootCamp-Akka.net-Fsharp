// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System
open Akka
open Akka.FSharp
open Consoles



[<EntryPoint>]
let main argv = 
    let system = System.create "my-system" (Configuration.load())

    let consoleWriterActor  = spawn system "consoleWriterActor" (actorOf2(ConsoleWriterActor))
    let validatorActor  = 
        actorOf2(ValidatorActor(consoleWriterActor))
        |> spawn system "validatorActor" 
    
    let consoleReaderActor  = 
        actorOf2(ConsoleReaderActor(validatorActor))
        |> spawn system "consoleReaderActor" 
    
    consoleReaderActor.Tell(Start)
    printfn "%A" argv
    

    
    system.AwaitTermination()
    0 // return an integer exit code
