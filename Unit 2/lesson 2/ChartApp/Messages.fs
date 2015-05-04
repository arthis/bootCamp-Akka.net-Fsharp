[<AutoOpen>]
module Messages

    open System.Collections.Generic
    open System.Windows.Forms.DataVisualization.Charting

    type ChartMsg = 
        | InitializeChart of Dictionary<string, Series> 
        | AddSeries of Series