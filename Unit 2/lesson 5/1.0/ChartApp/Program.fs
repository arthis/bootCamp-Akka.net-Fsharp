namespace ChartApp

open Akka.FSharp
open System
open System.Drawing
open System.Windows.Forms
open System.Configuration
open Akka.Configuration.Hocon

module Main =

    [<STAThread>]
    [<EntryPoint>]
    do
        let section = ConfigurationManager.GetSection("akka"):?> AkkaConfigurationSection
        
        let ChartActors = System.create "ChartActors" ( section.AkkaConfig)
            
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(false)
        Application.Run(new MainForm(ChartActors) :> Form) 
