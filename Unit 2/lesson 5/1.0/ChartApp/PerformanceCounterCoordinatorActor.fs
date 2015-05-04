module PerformanceCounterCoordinatorActor

    open System
    open System.Collections.Generic
    open System.Diagnostics
    open System.Drawing
    open System.Windows.Forms.DataVisualization.Charting
    open Akka
    open Akka.FSharp
    open Akka.Actor

    let counterGenerators counterType =
        match counterType with
        | Cpu -> fun()  -> new PerformanceCounter("Processor", "% Processor Time", "_Total", true)
        | Memory -> fun()  ->new PerformanceCounter("Memory", "% Committed Bytes In Use", true)
        | Disk -> fun()  ->new PerformanceCounter("LogicalDisk", "% Disk Time", "_Total", true)

    let counterSeries counterType =
        let createSerie (counterType:CounterType) chartType color = 
            let s = new Series(counterType.toString)
            s.ChartType <- chartType
            s.Color <- color
            s

        match counterType with
        | Cpu -> createSerie CounterType.Cpu SeriesChartType.SplineArea Color.DarkGreen
        | Memory -> createSerie CounterType.Memory SeriesChartType.FastLine Color.MediumBlue 
        | Disk -> createSerie CounterType.Disk SeriesChartType.SplineArea Color.DarkRed 
    
    let actor (chartingActor:IActorRef) (counterActors:Dictionary<CounterType, IActorRef>) = 
        fun (mailbox:Actor<PerformanceCounterCoordinatorMsg>) msg ->
        match msg with
        | Watch(counterType) -> 
            if not <| counterActors.ContainsKey(counterType) then
                // create a child actor to monitor this counter if one doesn't exist already
                let counterTypeName =counterType.toString
                let actor = PerformanceCounterActor.actor  counterTypeName  (counterGenerators counterType)
                            |> spawn mailbox.Context counterTypeName  
                counterActors.Add(counterType, actor)
            
            // register this series with the ChartingActor
            chartingActor.Tell(AddSeries(counterSeries counterType))
            // tell the counter actor to begin publishing its statistics to the _chartingActor
            counterActors.[counterType].Tell(SubscribeCounter(counterType,chartingActor))

        | Unwatch(counterType) -> 
            if counterActors.ContainsKey(counterType) then
                // unsubscribe the ChartingActor from receiving anymore updates
                counterActors.[counterType].Tell(UnsubscribeCounter(counterType,chartingActor))
                // remove this series from the ChartingActor
                let counterTypeName =counterType.toString
                chartingActor.Tell(RemoveSeries(counterTypeName))
