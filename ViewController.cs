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
	double _timerInterval = 5 * 60 * 1000;
	bool _isCapturing = false;
	ScreenshotTimerService _screenshotService;
    string[] _units = { "Seconds", "Minutes", "Hours" };
    int _currentUnitIndex = 0;

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
			_urlTextBox.StringValue = "";
		}
		else
		{
			_startButton.Title = "Stop";
			_screenshotService = new ScreenshotTimerService(_timerInterval);
			_screenshotService.InputRect = View.Window.ConvertRectToScreen(_firstRow.Frame);
			_screenshotService.Start();

			var guidPart = Guid.NewGuid().ToString("N");
			var randomPart = Path.GetRandomFileName().Replace(".", "");
			var timestampPart = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");

			var currentId = $"{guidPart}{randomPart}{timestampPart}";
			await _screenshotService.CaptureAndUploadScreenshot(currentId);
			_urlTextBox.StringValue = $"{Constants.BaseUrl}/{currentId}";
		}

		_isCapturing = !_isCapturing;
	}

    private void UnitButton_Activated(object sender, EventArgs e)
    {
        _currentUnitIndex = (_currentUnitIndex + 1) % _units.Length;
        _unitButton.Title = _units[_currentUnitIndex];
    }
    private void MinusButton_Activated(object sender, EventArgs e)
    {
	    // Handle Minus button click
    }

    private void PlusButton_Activated(object sender, EventArgs e)
    {
	    // Handle Plus button click
    }
    

    private void VisitButton_Activated(object sender, EventArgs e)
    {
	    // Handle Small button click
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