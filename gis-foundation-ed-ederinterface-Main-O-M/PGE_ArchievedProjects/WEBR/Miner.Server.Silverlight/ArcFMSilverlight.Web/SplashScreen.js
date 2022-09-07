function onSourceDownloadProgressChanged(sender, eventArgs) {
    sender.findName("uxStatus").Text = "Loading " + Math.round((eventArgs.progress * 100)) + "%";   
    sender.findName("uxProgress").Width = eventArgs.progress * 298;
}