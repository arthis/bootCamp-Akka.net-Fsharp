namespace ChartApp

open Akka.FSharp
open System
open System.Drawing
open System.Windows.Forms

module Main =

    [<STAThread>]
    [<EntryPoint>]
    do
        let ChartActors = System.create "ChartActors" (Configuration.load())

        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(false)
        Application.Run(new MainForm(ChartActors) :> Form) 
