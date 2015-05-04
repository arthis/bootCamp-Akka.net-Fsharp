[<AutoOpen>]
module Messages

    open System.Collections.Generic
    open System.Windows.Forms.DataVisualization.Charting
    open Akka.Actor
    open Microsoft.FSharp.Reflection

    let inline (|>>) x1 x2 = (x1, x2)

    let toString (x:'a) = 
        match FSharpValue.GetUnionFields(x, typeof<'a>) with
        | case, _ -> case.Name

    let fromString<'a> (s:string) =
        match FSharpType.GetUnionCases typeof<'a> |> Array.filter (fun case -> case.Name = s) with
        |[|case|] -> Some(FSharpValue.MakeUnion(case,[||]) :?> 'a)
        |_ -> None

    type Metric  = {
        Series : string;
        CounterValue : float
    }

    type ButtonToggleMsg =
        | Toggle

    type CounterType = 
        | Cpu
        | Memory
        | Disk
        with member this.toString = toString this

    type ChartTogglingMsg = 
        | TogglePause

    type ChartMsg = 
        | InitializeChart of Dictionary<string, Series> 
        | AddSeries of Series
        | RemoveSeries of string
        | AddMetric of Metric
        | TogglePause

    type PerformanceCounterCoordinatorMsg =
        | Watch of CounterType
        | Unwatch of CounterType

    type PerformanceCounterMsg =
        | GatherMetrics
        | SubscribeCounter of CounterType*IActorRef
        | UnsubscribeCounter of CounterType*IActorRef

    