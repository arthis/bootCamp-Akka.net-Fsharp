namespace GithubActors

open System
open System.Drawing
open System.Collections.Generic
open System.Windows.Forms
open Akka.Actor
open Akka.FSharp
open GithubActors.Actors
open System.Diagnostics


type public GithubAuthForm(system : ActorSystem ) as form  =
    inherit Form()

    let label1 = new System.Windows.Forms.Label()
    let tbOAuth = new System.Windows.Forms.TextBox()
    let lblAuthStatus = new System.Windows.Forms.Label()
    let linkGhLabel = new System.Windows.Forms.LinkLabel()
    let btnAuthenticate = new System.Windows.Forms.Button()

    let githubCoordinatorActor = spawn system actorPaths.GithubCoordinatorActor.name githubCoordinator.actor
    
    let formLoad e = 
        ()

    let linkGhLabel_LinkClicked (e:LinkLabelLinkClickedEventArgs) = 
        if (e.Link.LinkData <> null) then
            let link = e.Link.LinkData:?>string
            //Send the URL to the operating system via windows shell
            Process.Start(link) |> ignore
            

    let btnAuthenticate_Click e = githubCoordinatorActor.Tell(Authenticate(tbOAuth.Text)) |> ignore
        

    do
        form.SuspendLayout();

        label1.AutoSize <- true
        label1.Font <- new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
        label1.Location <- new System.Drawing.Point(12 , 9 )
        label1.Name <- "label1"
        label1.Size <- new System.Drawing.Size(172, 18)
        label1.TabIndex <- 0
        label1.Text <- "GitHub Access Token"

        tbOAuth.Font <- new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
        tbOAuth.Location <- new System.Drawing.Point(190, 6)
        tbOAuth.Name <- "tbOAuth"
        tbOAuth.Size <- new System.Drawing.Size(379, 24)
        tbOAuth.TabIndex <- 1
        // 
        // lblAuthStatus
        // 
        lblAuthStatus.AutoSize <- true
        lblAuthStatus.Font <- new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
        lblAuthStatus.Location <- new System.Drawing.Point(187, 33)
        lblAuthStatus.Name <- "lblAuthStatus"
        lblAuthStatus.Size <- new System.Drawing.Size(87, 18)
        lblAuthStatus.TabIndex <- 2
        lblAuthStatus.Text <- "lblGHStatus"
        lblAuthStatus.Visible <- false
        // 
        // linkGhLabel
        // 
        linkGhLabel.AutoSize <- true
        linkGhLabel.Font <- new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
        linkGhLabel.Location <- new System.Drawing.Point(148, 128)
        linkGhLabel.Name <- "linkGhLabel"
        linkGhLabel.Size <- new System.Drawing.Size(273, 18)
        linkGhLabel.TabIndex <- 3
        linkGhLabel.Text <- "How to get a GitHub Access Token"
        let l = new LinkLabel.Link()
        l.LinkData <- "https://help.github.com/articles/creating-an-access-token-for-command-line-use/" 
        linkGhLabel.Links.Add(l) |> ignore
        linkGhLabel.LinkClicked.Add(linkGhLabel_LinkClicked)
        

        // 
        // btnAuthenticate
        // 
        btnAuthenticate.Font <- new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)))
        btnAuthenticate.Location <- new System.Drawing.Point(214, 81)
        btnAuthenticate.Name <- "btnAuthenticate"
        btnAuthenticate.Size <- new System.Drawing.Size(136, 32)
        btnAuthenticate.TabIndex <- 4
        btnAuthenticate.Text <- "Authenticate"
        btnAuthenticate.UseVisualStyleBackColor <- true
        btnAuthenticate.Click.Add(btnAuthenticate_Click)

        form.Controls.AddRange([|(label1:>Control);
                                (tbOAuth:>Control);
                                (lblAuthStatus:>Control);
                                (linkGhLabel:>Control);
                                (btnAuthenticate:>Control);
        |])

        form.AutoScaleMode <- System.Windows.Forms.AutoScaleMode.Font;
        form.AutoScaleDimensions <- new System.Drawing.SizeF(6 |> float32,  13|> float32);
        form.ClientSize <- new System.Drawing.Size(562, 151);
        
        form.Name <- "GithubAuth";
        form.Text <- "Sign in to GitHub";
        form.Load.Add(formLoad);

        
        // render the form
        form.ResumeLayout(false)
        form.PerformLayout()
    