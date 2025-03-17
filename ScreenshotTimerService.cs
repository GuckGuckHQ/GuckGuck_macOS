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

		var screenshot = await CaptureScreenRegion(InputRect);
		if (screenshot is not null)
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



					//save screenshot to file
					//using (var fs = File.OpenWrite(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), id + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png")))
					//{
					//	fs.Write(bytes, 0, bytes.Length);
					//}

					var content = new MultipartFormDataContent();
					content.Add(new ByteArrayContent(byteArray), "Image", "screenshot.png");
					content.Add(new StringContent(_currentId), "Id");
					var response = await _client.PostAsync($"{Constants.BaseUrl}/image", content);
				}
			}
		}
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

	public void Dispose()
	{
		_timer.Dispose();
		_client.Dispose();
	}
}