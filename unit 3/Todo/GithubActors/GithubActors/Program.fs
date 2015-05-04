namespace GithubActors
// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System
open System.Drawing
open System.Windows.Forms
open System.Configuration
open Akka.Actor
open Akka.Configuration.Hocon


module Main =
    
    [<STAThread>]
    [<EntryPoint>]
    do 

        let section = ConfigurationManager.GetSection("akka")
        let config = (section:?>AkkaConfigurationSection).AkkaConfig
        let GithubActors = ActorSystem.Create("GithubActors", config);

        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(false)
        Application.Run(new GithubAuthForm(GithubActors) :> Form) 
        
