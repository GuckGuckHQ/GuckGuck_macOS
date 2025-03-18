namespace GuckGuck;

[Register ("AppDelegate")]
public class AppDelegate : NSApplicationDelegate {
	public override void DidFinishLaunching (NSNotification notification)
	{

	}

	public override void WillTerminate (NSNotification notification)
	{
		// Insert code here to tear down your application
	}


    public override NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication sender)
    {
        // Allow the application to terminate
        return NSApplicationTerminateReply.Now;
    }
}