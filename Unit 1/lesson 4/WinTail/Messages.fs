[<AutoOpen>]
module Messages

    open Akka.Actor

    type ConsoleWriter =
        | Processing of string*string
        | NullError of string
        | ValidationError of string
        | ProcessingError of string

    type FileValidator =
        | Input of string

    type ConsoleReader =
        | Start 
        | ContinueProcessing
        | Exit
        | Input of string

    type TailCoordinator =
        | StartTail  of string* ActorRef
        | StopTail of string

    type Tail =
        | FileWrite of string
        | FileError of string*string
        | InitialRead of string*string
