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

    let sysChart =   new System.Windows.Forms.DataVisualization.Charting.Chart()
    let btnCpu = new Button()
    let btnMemory = new Button()
    let btnDisk = new Button()
    let btnPauseResume = new Button()
    let seriesCounter = new AtomicCounter(1)
    let toggleActors = new Dictionary<CounterType, IActorRef>()

    //Chart Actors
    let chartActor  = spawn system "charting" <| (ChartActor.actor sysChart btnPauseResume)
    let coordinatorActor = 
        PerformanceCounterCoordinatorActor.actor chartActor toggleActors
        |> actorOf2
        |> spawn system "counters" 

    // CPU button toggle actor
    let cpuActor = 
        ButtonToggleActor.actor coordinatorActor btnCpu CounterType.Cpu false
        |>> [SpawnOption.Dispatcher("akka.actor.synchronized-dispatcher")]
        ||> spawnOpt system "CpuButton" 
    
    // MEMORY button toggle actor
    let memoryActor = 
        ButtonToggleActor.actor coordinatorActor btnMemory CounterType.Memory false
        |>> [SpawnOption.Dispatcher("akka.actor.synchronized-dispatcher")]
        ||> spawnOpt system "MemoryButton"     
    
    // DISK button toggle actor
    let diskActor = 
        ButtonToggleActor.actor coordinatorActor btnDisk CounterType.Disk false
        |>> [SpawnOption.Dispatcher("akka.actor.synchronized-dispatcher")]
        ||> spawnOpt system "DiskButton"         

//    // Pause Resume button toggle actor
//    let pauseResumeActor = 
//        ButtonToggleActor.actor coordinatorActor btnDisk CounterType.Disk false
//        |>> [SpawnOption.Dispatcher("akka.actor.synchronized-dispatcher")]
//        ||> spawnOpt system "DiskButton"         

    let formLoad e = 
        let dicInitial = new Dictionary<string, Series>()
        chartActor.Tell <| InitializeChart (dicInitial)
        cpuActor.Tell Toggle

    //button click
    let btnCpu_Click e = cpuActor.Tell Toggle
    let btnMemory_Click e = memoryActor.Tell Toggle
    let btnDisk_Click e = diskActor.Tell Toggle
    let btnPauseResume_Click e = ()
    

    let initControls() = 

        // chart area 
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

         
        // add btnCpu button
        btnCpu.Name <- "btnCpu"
        btnCpu.Text <- "Add CPU (ON)"
        btnCpu.Size <- new Size(150,50)
        btnCpu.Location <- new System.Drawing.Point(850, 75)
        btnCpu.Click.AddHandler(new EventHandler( fun s e-> btnCpu_Click(e)))   

        // add btnCpu button
        btnMemory.Name <- "btnMemory"
        btnMemory.Text <- "MEMORY (OFF)"
        btnMemory.Size <- new Size(150,50)
        btnMemory.Location <- new System.Drawing.Point(850, 175)
        btnMemory.Click.AddHandler(new EventHandler( fun s e-> btnMemory_Click(e)))   

        // add btnCpu button
        btnDisk.Name <- "btnMemory"
        btnDisk.Text <- "DISK (OFF)"
        btnDisk.Size <- new Size(150,50)
        btnDisk.Location <- new System.Drawing.Point(850, 275)
        btnDisk.Click.AddHandler(new EventHandler( fun s e-> btnDisk_Click(e)))   

        // add btnCpu button
        btnPauseResume.Name <- "btnPauseResume"
        btnPauseResume.Text <- "PAUSE ||"
        btnPauseResume.Size <- new Size(150,50)
        btnPauseResume.Location <- new System.Drawing.Point(850, 375)
        btnPauseResume.Click.AddHandler(new EventHandler( fun s e-> btnPauseResume_Click(e)))   

    do
        
        (sysChart:>System.ComponentModel.ISupportInitialize).BeginInit()
        form.SuspendLayout();

        initControls()

        // TODO add controls to the form
        form.Controls.AddRange([|   (btnCpu:>Control);
                                    (btnMemory:>Control);
                                    (btnDisk:>Control);
                                    (sysChart:>Control);
            |])

        // TODO define form properties
        form.AutoScaleDimensions <- new System.Drawing.SizeF(6 |> float32,  13|> float32);
        form.AutoScaleMode <- System.Windows.Forms.AutoScaleMode.Font;
        form.ClientSize <- new System.Drawing.Size(1024, 500);
        
        form.Name <- "Main";
        form.Text <- "System Metrics";
        form.Load.Add(formLoad);
        (sysChart:>System.ComponentModel.ISupportInitialize).EndInit();

        
        // render the form
        form.ResumeLayout(false)
        form.PerformLayout()
