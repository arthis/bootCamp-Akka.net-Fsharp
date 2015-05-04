#r "..\\packages\\Akka.0.8.0\\lib\\net45\\Akka.dll"
#r "..\\packages\\Akka.FSharp.0.8.0\\lib\\net45\\Akka.FSharp.dll"
#r "..\\packages\\FsPickler.0.9.11\\lib\\net45\\FsPickler.dll"
#r "..\\packages\\Newtonsoft.Json.6.0.1\\lib\\net45\\Newtonsoft.Json.dll"

//#load "Messages.fs"
//#load "ConsoleWriter.fs"
//#load "FileValidator.fs"
//#load "ConsoleReader.fs"

open Akka
open Akka.FSharp



let system = System.create "my-system" (Configuration.load())

//let consoleWriterActor  = spawn system "consoleWriterActor" (actorOf2(ConsoleWriter.actor))
//let fileValidatorActor  = 
//    FileValidator.actor(consoleWriterActor)
//    |> actorOf2
//    |> spawn system "validatorActor" 
//    
//let consoleReaderActor  = 
//    actorOf2(ConsoleReader.actor(fileValidatorActor))
//    |> spawn system "consoleReaderActor" 
//    
//consoleReaderActor.Tell(Start)
//
//fileValidatorActor.Tell(FileValidator.Input(""))
//fileValidatorActor.Tell(FileValidator.Input("test"))
//fileValidatorActor.Tell(FileValidator.Input("tests"))
//
//consoleReaderActor.Tell(Exit)
