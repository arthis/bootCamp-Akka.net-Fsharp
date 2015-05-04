#nowarn "40"
namespace GithubActors.Actors

open Akka.Actor 
open System.Windows.Forms
open Akka.FSharp
open System.Drawing


module githubCoordinator =

    type State = {
        
    }

    let renderUnAuthenticated (label:Label) reason =
        label.ForeColor <- Color.Red
        label.Text <- reason;

    let actor (statusLabel:Label) = (fun (mailbox:Actor<_>) -> 
        let rec unauthenticatedActor = 
                actor {
                    let! message = mailbox.Receive ()
                    match message with 
                    | Authenticate(token) -> 
                    | _ -> ()
                    return! authenticatedActor
                } 
            and authenticatingActor = 
                actor {
                    let! message = mailbox.Receive ()
                    match message with 
                    | AuthenticationFailed -> 
                        renderUnAuthenticated statusLabel "Authentication failed."
                        return! unauthenticatedActor
                    | AuthenticationCancelled -> 
                        renderUnAuthenticated statusLabel "Authentication timed out."
                        return! unauthenticatedActor
                    | AuthenticationSuccess -> 
                        return! authenticatingActor
                }

        unauthenticatedActor
    )