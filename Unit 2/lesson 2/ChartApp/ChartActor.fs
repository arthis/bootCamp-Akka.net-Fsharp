module ChartActor


    open Akka.FSharp
    open System.Windows.Forms.DataVisualization.Charting
    open System.Linq
    open System.Collections.Generic
    
    type State = 
        { series : Dictionary<string, Series> }
        static member Initial = { series = new Dictionary<string, Series>()}


    let actor (chart:Chart)  =
        let processMessage (mailbox:Actor<ChartMsg>) msg (state:State)= 
            match msg with
            |  InitializeChart(dicSeries) ->
                chart.Series.Clear()
                for KeyValue(k,v) in dicSeries do
                    v.Name = k |> ignore
                    chart.Series.Add(v)
                { state with series = dicSeries }
            | AddSeries(series) -> 
                if not <| state.series.ContainsKey(series.Name) then 
                    state.series.Add(series.Name,series)
                    chart.Series.Add(series)
                state

        fun (mailbox:Actor<ChartMsg>) -> 
            let rec loop state = actor {
                let! message = mailbox.Receive()
                let newState = processMessage mailbox message state
                return! loop  newState
            }
            loop State.Initial

        

        


