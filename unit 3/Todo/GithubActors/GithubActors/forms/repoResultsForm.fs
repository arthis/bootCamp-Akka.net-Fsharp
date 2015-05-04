namespace GithubActors

open System
open System.Drawing
open System.Collections.Generic
open System.Windows.Forms



type public RepoResultsForm() as form  =
    inherit Form()

    let formLoad e = ()

    do
        form.SuspendLayout();

//        form.Controls.AddRange([|   (btnCpu:>Control);
//                                    (btnMemory:>Control);
//                                    (btnDisk:>Control);
//                                    (btnPauseResume:>Control);
//                                    (sysChart:>Control);
        form.AutoScaleMode <- System.Windows.Forms.AutoScaleMode.Font;
//            |])
        form.AutoScaleDimensions <- new System.Drawing.SizeF(6 |> float32,  13|> float32);
        form.ClientSize <- new System.Drawing.Size(562, 151);
        
        form.Name <- "LauncherForm";
        form.Text <- "Who Starred This Repo?";
        form.Load.Add(formLoad);

        
        // render the form
        form.ResumeLayout(false)
        form.PerformLayout()
    