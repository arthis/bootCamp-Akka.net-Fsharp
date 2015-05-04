module ConsoleWriter

    open System
    open Akka
    open Akka.Actor
    open Akka.FSharp

    let actor (mailbox:Actor<ConsoleWriter>) (msg:ConsoleWriter) =
        let writeToConsole color (m:string) =
            Console.ForegroundColor <- color
            Console.WriteLine(m)
            Console.ResetColor()

        match msg with
        | Processing(f,message) -> writeToConsole ConsoleColor.Green message
        | NullError(reason) | ValidationError(reason) | ProcessingError(reason) -> writeToConsole ConsoleColor.Red reason
            