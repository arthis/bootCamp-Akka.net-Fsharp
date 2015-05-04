[<AutoOpen>]
module Core

    type Result<'TSuccess, 'TFailure> =
    | Success of 'TSuccess
    | Failure of 'TFailure


    let bind switchFunction twoTrackInput = 
        match twoTrackInput with
        | Success s -> switchFunction s
        | Failure f -> Failure f
    
    let (>>=) twoTrackInput switchFunction = 
        bind switchFunction twoTrackInput

    let (>=>) switch1 switch2 x = 
        match switch1 x with
        | Success s -> switch2 s
        | Failure f -> Failure f


