module ChartActor

    open System
    open Akka.FSharp
    open System.Windows.Forms.DataVisualization.Charting
    open System.Linq
    open System.Collections.Generic
    open System.Windows.Forms
    
    type State = 
        { series : Dictionary<string, Series> ;
          xPosCounter : float; }
        static member Initial = { 
            series = new Dictionary<string, Series>(); 
            xPosCounter =0.0; //Incrementing counter we use to plot along the X-axis
        }



    let actor (chart:Chart)  (pauseButton:Button) =

        //Maximum number of points we will allow in a series
        let MaxPoints = 250
        
        let xPosCounter:float = 0.0

        let SetChartBoundaries (state:State) = 
            
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

        let HandleInitialize state dicSeries =
                let newState = { state with series = dicSeries }
                 // delete any existing series
                chart.Series.Clear()

                // set the axes up
                let area = chart.ChartAreas.[0]
                area.AxisX.IntervalType <- DateTimeIntervalType.Number
                area.AxisY.IntervalType <- DateTimeIntervalType.Number
            
                SetChartBoundaries newState

                // attempt to render the initial chart
                if newState.series.Any() then
                    for KeyValue(k,v) in newState.series do
                        v.Name = k |> ignore
                        chart.Series.Add(v)
            
                SetChartBoundaries newState
                newState
            

        let handleAddSeries state (series:Series) = 
            if  (not <| String.IsNullOrEmpty(series.Name) && not <| state.series.ContainsKey(series.Name)) then
                state.series.Add(series.Name,series)
                chart.Series.Add(series)
                SetChartBoundaries state
            state

        let handleRemoveSeries state (serieName:string) = 
            if (not <|String.IsNullOrEmpty(serieName) && not <| state.series.ContainsKey(serieName)) then
                chart.Series.Remove(state.series.[serieName]) |> ignore
                state.series.Remove(serieName) |> ignore 
            state   

        let handleMetric state metric =
            if (not <|String.IsNullOrEmpty(metric.Series) && state.series.ContainsKey(metric.Series)) then
                let newState = { state with xPosCounter = state.xPosCounter+ 1.0}
                if (newState.series.[metric.Series].Points <> null) then 
                    newState.series.[metric.Series].Points.AddXY(newState.xPosCounter , metric.CounterValue) |> ignore
                    while(newState.series.[metric.Series].Points.Count > MaxPoints) do
                        newState.series.[metric.Series].Points.RemoveAt(0)
                    SetChartBoundaries newState
                newState    
            else state

        
             
        fun (mailbox:Actor<ChartMsg>) -> 

            
            let rec loop state = actor {
                let! message = mailbox.Receive()
                let newState = match message with  
                                | InitializeChart(dicSeries) -> HandleInitialize state dicSeries
                                | AddSeries(series) ->  handleAddSeries state series
                                | RemoveSeries(serieName) -> handleRemoveSeries state serieName
                                | AddMetric(metric) -> handleMetric state metric
                                | TogglePause -> state
                return! loop  newState
            }
            loop State.Initial

        

        


