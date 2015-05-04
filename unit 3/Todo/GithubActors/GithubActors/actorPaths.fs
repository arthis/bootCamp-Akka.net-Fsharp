
module actorPaths 

    type ActorMetaData = {
        name:string;
        path : string;
    }

    let GithubAuthenticatorActor = { name ="authenticator"; path ="akka://GithubActors/user/authenticator"}
    let MainFormActor = { name ="mainform"; path ="akka://GithubActors/user/mainform"}
    let GithubValidatorActor = { name ="validator"; path ="akka://GithubActors/user/validator"}
    let GithubCommanderActor = { name ="commander"; path ="akka://GithubActors/user/commander"}
    let GithubCoordinatorActor = { name ="coordinator"; path ="akka://GithubActors/user/commander/authenticator"}