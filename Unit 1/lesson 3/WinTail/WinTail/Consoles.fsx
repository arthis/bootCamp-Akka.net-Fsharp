#r "..\\packages\\Akka.0.8.0\\lib\\net45\\Akka.dll"
#r "..\\packages\\Akka.FSharp.0.8.0\\lib\\net45\\Akka.FSharp.dll"
#r "..\\packages\\FsPickler.0.9.11\\lib\\net45\\FsPickler.dll"
#r "..\\packages\\Newtonsoft.Json.6.0.1\\lib\\net45\\Newtonsoft.Json.dll"

#load "Consoles.fs"

open Akka
open Akka.FSharp
open Consoles

let system = System.create "my-system" (Configuration.load())

let consoleWriterActor  = spawn system "consoleWriterActor" (actorOf2(ConsoleWriterActor))
let validatorActor  = 
    actorOf2(ValidatorActor(consoleWriterActor))
    |> spawn system "validatorActor" 
    
let consoleReaderActor  = 
    actorOf2(ConsoleReaderActor(validatorActor))
    |> spawn system "consoleReaderActor" 
    
consoleReaderActor.Tell(Start)








