module ChartDataHelper

    open System
    open System.Linq
    open System.Windows.Forms.DataVisualization.Charting
    open Akka.Util
    

    let RandomSeries seriesName seriesChartType points = 
        let createDataPoint i = 
            let rng = ThreadLocalRandom.Current.NextDouble()
            let dp = new DataPoint(i |> float, 2.0*Math.Sin(rng) + Math.Sin(rng/4.5))
            dp

        let series = new Series(seriesName)
        series.ChartType <- seriesChartType
        Enumerable.Range(0, points)
        |> Seq.map createDataPoint
        |> Seq.iter series.Points.Add
        series



