// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System
open Akka
open Akka.FSharp
open Consoles

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

[<EntryPoint>]
let main argv = 
    let system = System.create "my-system" (Configuration.load())

    let consoleWriterActor  = spawn system "consoleWriterActor" (actorOf2(ConsoleWriterActor))
    let consoleReaderActor  = 
        actorOf2(ConsoleReaderActor consoleWriterActor)
        |> spawn system "consoleReaderActor"
    
    consoleReaderActor.Tell(Some("start"))
    printfn "%A" argv
    printInstructions

    
    system.AwaitTermination()
    0 // return an integer exit code
