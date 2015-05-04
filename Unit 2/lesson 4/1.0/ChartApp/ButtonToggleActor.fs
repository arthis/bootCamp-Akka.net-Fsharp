module ButtonToggleActor

    open System
    open Akka.Actor
    open Akka.FSharp
    open System.Windows.Forms

    let actor (coordinatorActor:IActorRef) (myButton:Button) (myCounterType:CounterType) (isToggledOn:bool) = 
        
        fun (mailbox:Actor<ButtonToggleMsg>) ->
            
            let changeTextButton state = 
                let textOnOff = if  state then "ON" else "OFF" 
                myButton.Text <- String.Format("{0} ({1})", myCounterType.toString.ToUpperInvariant(),   textOnOff  )

            let rec loop (state:bool) = actor {
                let! message = mailbox.Receive()

                match message with 
                | Toggle -> 
                    let newState = not <| state    
                    if newState then
                        // toggle is currently off
                        // start watching this counter
                        coordinatorActor.Tell(Watch(myCounterType)) 
                    else
                        // toggle is currently on
                         // stop watching this counter
                        coordinatorActor.Tell(Unwatch(myCounterType))
                    // change the text of the button
                    changeTextButton (newState)
                    // flip the toggle 
                    return! loop(newState) 
            }
            loop(isToggledOn)
        