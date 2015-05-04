[<AutoOpen>]
module messages

open System
open Octokit
open Akka.Actor
open System.Collections.Generic



type RepoKey = {
    owner : string;
    repo : string;
}

type RetryableQuery = {
    query : Object;
    allowableTries : int;
    currentAttempt : int;
}

type GithubAuthenticationMsg =
    | Authenticate of string
    | AuthenticationFailed
    | AuthenticationCancelled
    | AuthenticationSuccess

type GithubCommanderMsg =
    | CanAcceptJob of RepoKey
    | AbleToAcceptJob of RepoKey
    | UnableToAcceptJob of RepoKey


type GithubCoordinatorMsg =
    | SubscribeToProgressUpdates of IActorRef
    //| PublishUpdate
    | JobFailed of RepoKey

type GithubValidatorMsg =
    | ValidateRepo of string
    | InvalidRepo of string*string
    | SystemBusy
    | RepoIsValid

type GithubWorkerMsg=
    | QueryStarrers of RepoKey
    | QueryStarrer of string
    | StarredReposForUser of string*IEnumerable<Repository>

type MainFormMsg = 
    | LaunchRepoResultsWindow of RepoKey*IActorRef
    | ProcessRepo of string