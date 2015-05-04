namespace ChartApp

open System
open System.Drawing
open System.Collections.Generic
open System.Windows.Forms
open Akka.Actor
open Akka.Util.Internal
open Akka.FSharp
open System.Windows.Forms.DataVisualization.Charting

type public MainForm(system:ActorSystem) as form =
    inherit Form()

    // TODO define your controls
//    let formLabel = new Label() 
    let sysChart =   new System.Windows.Forms.DataVisualization.Charting.Chart()

    let formLoad e = 
        let chartActor  = spawn system "charting" (actorOf2(ChartActor.actor(sysChart)))
        let seriesCounter = new AtomicCounter(99)
        let dicInitial = new Dictionary<string, Series>()
        let series = ChartDataHelper.RandomSeries "FakeSeries" SeriesChartType.Line <| seriesCounter.IncrementAndGet()
        dicInitial.Add(series.Name,series)
        chartActor.Tell <| InitializeChart (dicInitial)


    
    // TODO initialize your controls
    let initControls() = 
//        formLabel.Text <- "Main form data" 
//        formLabel.DoubleClick.AddHandler(new EventHandler 
//            (fun sender e -> form.eventLabel_DoubleClick(sender, e)))   

        let chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea()
        let legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend()
        let series1 = new System.Windows.Forms.DataVisualization.Charting.Series()

        chartArea1.Name <- "ChartArea1"
        sysChart.ChartAreas.Add(chartArea1)
        sysChart.Dock <- System.Windows.Forms.DockStyle.Fill
        legend1.Name <- "Legend1"
        sysChart.Legends.Add(legend1)
        sysChart.Location <- new System.Drawing.Point(0, 0)
        sysChart.Name <- "sysChart"
        series1.ChartArea <- "ChartArea1"
        series1.Legend <- "Legend1"
        series1.Name <- "Series1"
        sysChart.Series.Add(series1)
        sysChart.Size <- new System.Drawing.Size(684, 446)
        sysChart.TabIndex <- 0
        sysChart.Text <- "sysChart"

    do
        
        (sysChart:>System.ComponentModel.ISupportInitialize).BeginInit()
        form.SuspendLayout();

        initControls()

        // TODO add controls to the form
        form.Controls.Add(sysChart)

        // TODO define form properties
        form.AutoScaleDimensions <- new System.Drawing.SizeF(6 |> float32,  13|> float32);
        form.AutoScaleMode <- System.Windows.Forms.AutoScaleMode.Font;
        form.ClientSize <- new System.Drawing.Size(684, 446);
        
        form.Name <- "Main";
        form.Text <- "System Metrics";
        form.Load.Add(formLoad);
        (sysChart:>System.ComponentModel.ISupportInitialize).EndInit();

        
        // render the form
        form.ResumeLayout(false)
        form.PerformLayout()

   