module FileObserver

    open Akka.Actor
    open System.IO

    let create (tailActor:ActorRef) (absoluteFilePath:string) = 
        
        let fileDir = Path.GetDirectoryName(absoluteFilePath)
        let fileNameOnly = Path.GetFileName(absoluteFilePath)
        

        let OnFileChanged (e:FileSystemEventArgs )=
            // here we use a special ActorRef.NoSender
            // since this event can happen many times, this is a little microoptimization
            if (e.ChangeType = WatcherChangeTypes.Changed) then tailActor.Tell(FileWrite(e.Name),ActorRef.NoSender)

        let OnFileError (e:ErrorEventArgs ) =
            tailActor.Tell(FileError(fileNameOnly, e.GetException().Message), ActorRef.NoSender)


        // Need this for Mono 3.12.0 workaround
        //Environment.SetEnvironmentVariable("MONO_MANAGED_WATCHER", "enabled") // uncomment this line if you're running on Mono!

        let watcher =  new FileSystemWatcher(fileDir, fileNameOnly)
            
        // watch our file for changes to the file name, or new messages being written to file
        watcher.NotifyFilter <- NotifyFilters.FileName ||| NotifyFilters.LastWrite

        // assign callbacks for event types
        watcher.Changed.Add(OnFileChanged)
        watcher.Error.Add(OnFileError)

        // start watching
        watcher.EnableRaisingEvents <- true

        watcher

        
            


