module ChartActor


    open Akka.FSharp
    open System.Windows.Forms.DataVisualization.Charting
    open System.Linq

    let actor (chart:Chart)  =
        fun (mailbox:Actor<ChartMsg>) (msg:ChartMsg) -> 
        match msg with
        |  InitializeChart(dicSeries) ->
            chart.Series.Clear()
            dicSeries
            for KeyValue(k,v) in dicSeries do
                v.Name = k
                chart.Series.Add(v)

        


