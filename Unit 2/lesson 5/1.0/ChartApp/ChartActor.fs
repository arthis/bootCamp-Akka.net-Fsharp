module ChartActor

open System
open Akka.FSharp
open System.Windows.Forms.DataVisualization.Charting
open System.Linq
open System.Collections.Generic
open System.Windows.Forms

    
    
type State = 
    { series : Dictionary<string, Series> ;
        xPosCounter : float;
    }
    static member Initial = { 
        series = new Dictionary<string, Series>(); 
        xPosCounter =0.0; //Incrementing counter we use to plot along the X-axis
    }


let actor (chart:Chart)  (pauseButton:Button) =

    //Maximum number of points we will allow in a series
    let MaxPoints = 250
        
    let xPosCounter:float = 0.0

    let setChartBoundaries (state:State) = 
            
        let allPoints = state.series.Values
                        |> Seq.fold (fun(set:HashSet<DataPoint>) (serie:Series) -> 
                            set.Concat(serie.Points) |>ignore
                            set
                        ) (new HashSet<DataPoint>())
                        
        let yValues = allPoints
                        |> Seq.fold (fun (l:float []) (point:DataPoint) ->  
                            l.Concat(point.YValues)|> ignore
                            l 
                        ) [||]
        let maxAxisX = state.xPosCounter
        let minAxisX = state.xPosCounter -  (MaxPoints |> float)
        let maxAxisY = if yValues.Length > 0 then Math.Ceiling(yValues.Max()) else 1.0 
        let minAxisY = if yValues.Length > 0 then Math.Floor(yValues.Min()) else 0.0 
        if (allPoints.Count > 2) then
            let area = chart.ChartAreas.[0]
            area.AxisX.Minimum <- minAxisX
            area.AxisX.Maximum <- maxAxisX
            area.AxisY.Minimum <- minAxisY
            area.AxisY.Maximum <- maxAxisY

    let handleAddSeries state (series:Series) = 
        if  (not <| String.IsNullOrEmpty(series.Name) && not <| state.series.ContainsKey(series.Name)) then
            state.series.Add(series.Name,series)
            chart.Series.Add(series)
            setChartBoundaries state
        state

    let handleRemoveSeries state (serieName:string) = 
        if (not <|String.IsNullOrEmpty(serieName) && not <| state.series.ContainsKey(serieName)) then
            chart.Series.Remove(state.series.[serieName]) |> ignore
            state.series.Remove(serieName) |> ignore 
        state   

    let handleMetric state metric =
        if (not <|String.IsNullOrEmpty(metric.Series) && state.series.ContainsKey(metric.Series) && state.series.[metric.Series].Points <> null ) then
            let newState = { state with xPosCounter = state.xPosCounter+ 1.0}
            let series = newState.series.[metric.Series]
            series.Points.AddXY(newState.xPosCounter , metric.CounterValue) |> ignore
            while(series.Points.Count > MaxPoints) do series.Points.RemoveAt(0)
            setChartBoundaries newState
            newState    
        else state
    
    let setPauseButtonText paused = pauseButton.Text <- if not paused then "PAUSE ||" else "RESUME ->"

    let HandleInitialize state dicSeries =
            let newState = { state with series = dicSeries }
                // delete any existing series
            chart.Series.Clear()

            // set the axes up
            let area = chart.ChartAreas.[0]
            area.AxisX.IntervalType <- DateTimeIntervalType.Number
            area.AxisY.IntervalType <- DateTimeIntervalType.Number
            
            setChartBoundaries newState

            // attempt to render the initial chart
            if newState.series.Any() then
                for KeyValue(k,v) in newState.series do
                    v.Name = k |> ignore
                    chart.Series.Add(v)
            
            setChartBoundaries newState
            newState

       

    let runningHandler state message =
        match message with 
        | AddSeries(series) ->  handleAddSeries state series
        | RemoveSeries(serieName) -> handleRemoveSeries state serieName
        | AddMetric(metric) -> handleMetric state metric 
        | InitializeChart(dicSeries) -> HandleInitialize state dicSeries
        | _ -> state
             
    fun (mailbox:Actor<ChartMsg>) -> 
        let rec runningChartActor state =  actor {
            let! message = mailbox.Receive ()

            match message with
            | TogglePause -> 
                setPauseButtonText true
                return! pausedChartActor(state)
            | m -> 
                let newState = runningHandler state m
                return! runningChartActor newState
            } 
        and pausedChartActor state = actor {
            let! message = mailbox.Receive ()
            match message with
            | TogglePause -> 
                setPauseButtonText false

                // ChartingActor is leaving the Paused state, put messages back
                // into mailbox for processing under new behavior
                mailbox.UnstashAll()
                return! runningChartActor state
            | AddMetric(metric)  -> 
                let newState = { state with xPosCounter = state.xPosCounter+ 1.0}
                let series = newState.series.[metric.Series]
                series.Points.AddXY ( newState.xPosCounter, 0.) |> ignore
                return! pausedChartActor newState
            | AddSeries(series) ->  
                mailbox.Stash()
                return! pausedChartActor state
            | RemoveSeries(serieName)->  
                mailbox.Stash()
                return! pausedChartActor state
            | InitializeChart(dicSeries) -> failwith "cannot initilize paused actor"
            
        }
        runningChartActor State.Initial
