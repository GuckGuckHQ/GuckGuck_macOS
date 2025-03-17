using System.Runtime.InteropServices;
using AppKit;
using Foundation;
using CoreGraphics;
using ImageIO;
using MobileCoreServices;
using ObjCRuntime;
using ScreenCaptureKit;

namespace GuckGuck;

public partial class ViewController : NSViewController
{
	double _timerInterval = 1.0;
	bool _isCapturing = false;
	ScreenshotTimerService _screenshotService;

	public ViewController() : base(nameof(ViewController), null)
	{
	}

	protected ViewController(NativeHandle handle) : base(handle)
	{
		// This constructor is required if the view controller is loaded from a xib or a storyboard.
		// Do not put any initialization here, use ViewDidLoad instead.
	}

	private async void StartButton_Activated(object sender, EventArgs e)
	{
		if (_isCapturing)
		{
			_startButton.Title = "Start";
			_screenshotService.Stop();
			_screenshotService.Dispose();
		}
		else
		{
			_startButton.Title = "Stop";
			_screenshotService.Start();
			_screenshotService = new ScreenshotTimerService(_timerInterval);
			_screenshotService.InputRect = View.Window.ConvertRectToScreen(_firstRow.Frame);
		}

		_isCapturing = !_isCapturing;
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