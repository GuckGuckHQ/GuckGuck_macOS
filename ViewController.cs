using System.Runtime.InteropServices;
using AppKit;
using Foundation;
using CoreGraphics;
using ImageIO;
using MobileCoreServices;
using ObjCRuntime;
using ScreenCaptureKit;

namespace GuckGuck_macOS;

public partial class ViewController : NSViewController
{
    // Parameterless constructor
    public ViewController() : base(nameof(ViewController), null)
    {
    }

    protected ViewController(NativeHandle handle) : base(handle)
    {
        // This constructor is required if the view controller is loaded from a xib or a storyboard.
        // Do not put any initialization here, use ViewDidLoad instead.
    }
private async void Button_Activated(object sender, EventArgs e)
{
    var screenshot = await CaptureScreenRegion(_firstRow.Frame);
    if (screenshot != null)
    {
        var dataProvider = screenshot.DataProvider;
        if (dataProvider != null)
        {
            var data = dataProvider.CopyData();
            if (data != null)
            {
                var byteArray = new byte[data.Length];
                Marshal.Copy(data.Bytes, byteArray, 0, (int)data.Length);

                var byteDataProvider = new CGDataProvider(byteArray, 0, byteArray.Length);
                var cgImage =
                    CGImage.FromPNG(byteDataProvider, null, true, CGColorRenderingIntent.Default);

                //upload to https://runasp.net

            }
        }
    }
}


    private async Task<CGImage?> CaptureScreenRegion(CGRect region)
        {
            var screenRect = View.Window.ConvertRectToScreen(region);
            var display = NSScreen.MainScreen.DeviceDescription["NSScreenNumber"];
            var configuration = new SCStreamConfiguration
            {
                Width = (uint)screenRect.Width,
                Height = (uint)screenRect.Height,
            };

            var processId = Interop.GetForegroundWindowProcessId();
            var windowInfoList = Interop.GetWindowInfoList(CGWindowListOption.OnScreenOnly, 0);

            for (uint i = 0; i < windowInfoList?.Count; i++)
            {
                var window = Runtime.GetNSObject<NSDictionary>(windowInfoList.ValueAt(i));
                var windowDict = Runtime.GetNSObject<NSDictionary>(windowInfoList.ValueAt(i));

                var windowNumber = uint.Parse(windowDict.ObjectForKey((NSString)"kCGWindowNumber").ToString() ?? "0");
                var processIdNumber = windowDict.ObjectForKey((NSString)"kCGWindowOwnerPID") as NSNumber;
                if (processIdNumber is not null)
                {
                    if (processId == processIdNumber.Int32Value)
                    {
                        var availableContent = Interop.CGWindowListCreateImage(screenRect,
                            CGWindowListOption.OnScreenBelowWindow,
                            windowNumber, CGWindowImageOption.BestResolution);
                        if (availableContent != IntPtr.Zero)
                        {
                            var img = Runtime.GetINativeObject<CGImage>(availableContent, false);
                            return img;
                        }
                    }
                }
            }

            return null;
        }


    public override NSObject RepresentedObject
    {
        get => base.RepresentedObject;
        set
        {
            base.RepresentedObject = value;

            // Update the view, if already loaded.
        }
    }
}