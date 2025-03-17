using System.Runtime.InteropServices;
using ObjCRuntime;

namespace GuckGuck_macOS;

public static class Interop
{
    [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
    private static extern IntPtr CGWindowListCopyWindowInfo(CGWindowListOption option, uint relativeToWindow);
        
    [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
    public static extern IntPtr CGWindowListCreateImage(CGRect screenBounds, CGWindowListOption windowOption, uint windowId, CGWindowImageOption imageOption);
    
    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    public static extern IntPtr CGDisplayCreateImage(uint displayId);
    
    public static NSArray? GetWindowInfoList(CGWindowListOption option, uint relativeToWindow)
    {
        var ptr = CGWindowListCopyWindowInfo(option, relativeToWindow);
        return Runtime.GetNSObject<NSArray>(ptr);
    }
    
    public static int GetForegroundWindowProcessId()
    {
        var foregroundApp = NSWorkspace.SharedWorkspace.FrontmostApplication;
        return foregroundApp.ProcessIdentifier;
    }

}