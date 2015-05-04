module Tail

    open Akka.Actor
    open Akka.FSharp
    open System
    open System.IO
    open System.Text

    let actor (reporterActor:ActorRef) (filePath:string)=
        fun (mailbox:Actor<Messages.Tail>) -> 
            
            let observer = FileObserver.create mailbox.Self filePath

            let fileStream =new FileStream(Path.GetFullPath(filePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            let fileStreamReader = new StreamReader(fileStream, Encoding.UTF8)

            let text = fileStreamReader.ReadToEnd()

            mailbox.Self.Tell(InitialRead(filePath, text))

            

            let rec loop() = actor {
                let! message = mailbox.Receive()
                
                match message with
                | FileWrite(f) ->
                    let text = fileStreamReader.ReadToEnd()
                    if not <| String.IsNullOrEmpty(text) then  reporterActor.Tell(Processing(filePath, text))
                | FileError(fileName, reason) -> 
                    let msg= sprintf "Tail error: %s" reason
                    reporterActor.Tell(ProcessingError(msg))
                | InitialRead(fileName, text) -> reporterActor.Tell(Processing(filePath, text))

                return! loop()
            }
            loop()
        