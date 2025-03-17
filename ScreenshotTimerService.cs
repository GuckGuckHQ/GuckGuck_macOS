using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Timers;
using System.Runtime.InteropServices;
using AppKit;
using Foundation;
using CoreGraphics;
using ImageIO;
using MobileCoreServices;
using ObjCRuntime;
using ScreenCaptureKit;
using Timer = System.Timers.Timer;

namespace GuckGuck;

public class ScreenshotTimerService : IDisposable
{
	private readonly Timer _timer;
	private readonly HttpClient _client;
	private string _currentId;
	public CGRect InputRect { get; set; }


	public ScreenshotTimerService(double interval)
	{
		_timer = new Timer(interval);
		_timer.Elapsed += OnTimedEvent;
		_client = new HttpClient();
	}

	public void Start()
	{
		_timer.Stop();
		_timer.Start();
	}

	public void Stop()
	{
		_timer.Stop();
	}

	public void UpdateInterval(double interval)
	{
		_timer.Interval = interval;
	}

	public void UpdateCurrentId(string id)
	{
		_currentId = id;
	}

	private async void OnTimedEvent(object sender, ElapsedEventArgs e)
	{
		await CaptureAndUploadScreenshot(_currentId);
	}

public async Task CaptureAndUploadScreenshot(string id)
{
    Debug.WriteLine("Capture: " + DateTime.Now.ToString());
    UpdateCurrentId(id);

    NSApplication.SharedApplication.InvokeOnMainThread(async () =>
    {
        var screenshot = await CaptureScreenRegion(InputRect);

        if (screenshot is not null)
        {
            var bitmapRep = new NSBitmapImageRep(screenshot);

            var jpegData = bitmapRep.RepresentationUsingTypeProperties(NSBitmapImageFileType.Jpeg, new NSDictionary());
            var byteArray = new byte[jpegData.Length];
            Marshal.Copy(jpegData.Bytes, byteArray, 0, (int)jpegData.Length);

            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(byteArray), "Image", "screenshot.jpg");
            content.Add(new StringContent(_currentId), "Id");
            var response = await _client.PostAsync($"{Constants.BaseUrl}/image", content);
        }
    });
}


	async Task<CGImage?> CaptureScreenRegion(CGRect screenRect)
	{
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
						CGWindowListOption.All,
						windowNumber, CGWindowImageOption.Default);
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

	public void Dispose()
	{
		_timer.Dispose();
		_client.Dispose();
	}
}