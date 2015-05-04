module PerformanceCounterActor

    open System
    open Akka.Actor
    open Akka.FSharp
    open System.Windows.Forms.DataVisualization.Charting
    open System.Linq
    open System.Collections.Generic
    open System.Diagnostics
    open System.Threading

    type State = 
        { subscriptions : HashSet<IActorRef> ;
        }
        static member Initial = { 
            subscriptions = new HashSet<IActorRef>(); 
        }

    let actor (seriesName:string) (performanceCounterGenerator: unit->PerformanceCounter) = 
        

        let counter = performanceCounterGenerator()
        
        fun (mailbox:Actor<PerformanceCounterMsg>) ->
            
            let cancelPublishing = new Cancelable(mailbox.Context.System.Scheduler)    
            mailbox.Context.System.Scheduler.ScheduleTellRepeatedly(250 , 250, mailbox.Self, GatherMetrics,mailbox.Self, cancelPublishing) 

            //add PostStop events
            mailbox.Defer(fun() -> 
                cancelPublishing.Cancel(false)
                counter.Dispose()
            )
         
            let rec loop state = actor {
                let! message = mailbox.Receive()

                let newState = 
                    match message with
                    | GatherMetrics -> 
                        state.subscriptions
                        |> Seq.iter (fun(x:IActorRef) -> x.Tell(AddMetric({ Series=seriesName;  CounterValue = counter.NextValue() |> float })))
                        state
                    | SubscribeCounter(counterType,actor)-> 
                        state.subscriptions.Add(actor) |> ignore
                        state
                    | UnsubscribeCounter(counterType,actor) -> 
                        state.subscriptions.Remove(actor) |> ignore
                        state

                return! loop newState}
            loop State.Initial
        