using System.Diagnostics;
using CoreText;

namespace GuckGuck;

public partial class ViewController : NSViewController
{
	private NSView _firstRow;
	private int _spacing = 5;
	private int _padding = 10;
	private NSButton _startButton;
	private NSButton _minusButton;
	private NSButton _plusButton;
	private NSTextField _intervalTextBox;
	private NSButton _unitButton;
	private NSTextView _urlTextView;
	private NSButton _visitButton;

	public override void ViewWillAppear()
	{
		base.ViewWillAppear();
		View.Window.Delegate = new WindowDelegate(this);

		View.Window.BackgroundColor = NSColor.Clear;
		View.Window.IsOpaque = false;
		View.Window.HasShadow = false;
	}

	public void OnWindowMoved()
	{
		if (_screenshotService != null)
		{
			_screenshotService.SetInputRect(View.Window.ConvertRectToScreen(_firstRow.Frame));
		}
	}

	public override void ViewDidLayout()
	{
		base.ViewDidLayout();
		if (_screenshotService != null)
		{
			_screenshotService.SetInputRect(View.Window.ConvertRectToScreen(_firstRow.Frame));
		}
	}

	public override void ViewDidLoad()
	{
		base.ViewDidLoad();

    // Load the custom font
    var fontPath = NSBundle.MainBundle.PathForResource("overpass-mono-regular", "otf");
    var fontData = NSData.FromFile(fontPath);
    var provider = new CGDataProvider(fontData);
    var cgFont = CGFont.CreateFromProvider(provider);

    if (cgFont != null)
    {
        CTFontManager.RegisterGraphicsFont(cgFont, out var error);
        if (error != null)
        {
            Debug.WriteLine($"Error registering font: {error.LocalizedDescription}");
        }
    }

    // Create an NSFont instance with the custom font
    var customFont = NSFont.FromFontName("Overpass Mono", 14); // Adjust size as needed
    if (customFont == null)
    {
        Debug.WriteLine("Failed to load custom font.");
    }
        

		_firstRow = new NSView
		{
			WantsLayer = true,
			TranslatesAutoresizingMaskIntoConstraints = false
		};
		_firstRow.Layer.BorderWidth = 2;
		_firstRow.Layer.BorderColor = NSColor.FromRgb(65, 239, 144).CGColor; // Set border color to #41ef90

		// Create the second row with a background color of #1e1e1e
		var secondRow = new NSView
		{
			WantsLayer = true,
			TranslatesAutoresizingMaskIntoConstraints = false
		};
		secondRow.Layer.BackgroundColor = NSColor.FromRgb(30, 30, 30).CGColor; // Set background color to #1e1e1e
		_startButton = new NSButton
		{
			Title = "Start",
			TranslatesAutoresizingMaskIntoConstraints = false,
			Font = customFont
		};

		_minusButton = new NSButton
		{
			Title = "-",
			TranslatesAutoresizingMaskIntoConstraints = false,
			Font = customFont,
		};

		_plusButton = new NSButton
		{
			Title = "+",
			TranslatesAutoresizingMaskIntoConstraints = false, Font = customFont,
		};

		_intervalTextBox = new NSTextField
		{
			StringValue = "1",
			TranslatesAutoresizingMaskIntoConstraints = false, Font = customFont,
		};

		_unitButton = new NSButton
		{
			Title = "Minutes",
			TranslatesAutoresizingMaskIntoConstraints = false, Font = customFont,
		};

		_urlTextView = new NSTextView
		{
			TranslatesAutoresizingMaskIntoConstraints = false,
			TextColor = NSColor.FromRgb(235, 236, 240),
			BackgroundColor = NSColor.Clear,
			HorizontallyResizable = true,
			VerticallyResizable = false,
			Editable = false,
            Font = NSFont.FromFontName("Overpass Mono", 11), // Set font size to 11
		};
		_urlTextView.TextContainer.WidthTracksTextView = false;

		_visitButton = new NSButton
		{
			Title = "Visit",
			TranslatesAutoresizingMaskIntoConstraints = false,
			Font = customFont,
		};

		// Create a stack view for the minus, textbox, plus, and minutes button
		var stackView = new NSStackView
		{
			Orientation = NSUserInterfaceLayoutOrientation.Horizontal,
			TranslatesAutoresizingMaskIntoConstraints = false,
			Distribution = NSStackViewDistribution.FillProportionally,
			Spacing = _spacing
		};

		stackView.AddArrangedSubview(_minusButton);
		stackView.AddArrangedSubview(_intervalTextBox);
		stackView.AddArrangedSubview(_plusButton);
		stackView.AddArrangedSubview(_unitButton);

		secondRow.AddSubview(_startButton);
		secondRow.AddSubview(stackView);
		secondRow.AddSubview(_urlTextView);
		secondRow.AddSubview(_visitButton);

		// Add the rows to the view controller's view
		View.AddSubview(_firstRow);
		View.AddSubview(secondRow);

		// Set up constraints for the first row to take up the remaining space
		NSLayoutConstraint.ActivateConstraints(new[]
		{
			_firstRow.TopAnchor.ConstraintEqualTo(View.TopAnchor),
			_firstRow.LeftAnchor.ConstraintEqualTo(View.LeftAnchor),
			_firstRow.RightAnchor.ConstraintEqualTo(View.RightAnchor),
			_firstRow.BottomAnchor.ConstraintEqualTo(secondRow.TopAnchor)
		});

		// Set up constraints for the second row
		NSLayoutConstraint.ActivateConstraints(new[]
		{
			secondRow.LeftAnchor.ConstraintEqualTo(View.LeftAnchor),
			secondRow.RightAnchor.ConstraintEqualTo(View.RightAnchor),
			secondRow.BottomAnchor.ConstraintEqualTo(View.BottomAnchor),
			secondRow.HeightAnchor.ConstraintEqualTo(60)
		});

		// Set up constraints for the buttons and textbox in the second row
		NSLayoutConstraint.ActivateConstraints(new[]
		{
			_startButton.LeftAnchor.ConstraintEqualTo(secondRow.LeftAnchor, _spacing),
			_startButton.CenterYAnchor.ConstraintEqualTo(secondRow.CenterYAnchor, -_padding),
			_startButton.RightAnchor.ConstraintEqualTo(stackView.LeftAnchor, -_spacing),

			stackView.RightAnchor.ConstraintEqualTo(secondRow.RightAnchor, -_spacing),
			stackView.CenterYAnchor.ConstraintEqualTo(secondRow.CenterYAnchor, -_padding),
			stackView.WidthAnchor.ConstraintLessThanOrEqualTo(200), // Set maximum width for stackView

			_urlTextView.LeftAnchor.ConstraintEqualTo(secondRow.LeftAnchor, _spacing),
			_urlTextView.TopAnchor.ConstraintEqualTo(_startButton.BottomAnchor, _spacing),
			_urlTextView.RightAnchor.ConstraintEqualTo(_visitButton.LeftAnchor, -_spacing),
			_visitButton.RightAnchor.ConstraintEqualTo(secondRow.RightAnchor, -_spacing),
			_visitButton.CenterYAnchor.ConstraintEqualTo(_urlTextView.CenterYAnchor)
		});
		_urlTextView.HeightAnchor.ConstraintEqualTo(24).Active = true;

		// Add a minimum width constraint to the start button
		var _startButtonMinWidthConstraint = _startButton.WidthAnchor.ConstraintGreaterThanOrEqualTo(50);
		_startButtonMinWidthConstraint.Priority = 999; // Set a high priority but lower than required constraints
		_startButtonMinWidthConstraint.Active = true;

		var _unitButtonWidthConstraint = _unitButton.WidthAnchor.ConstraintEqualTo(100);
		_unitButtonWidthConstraint.Active = true;

		var _intervalTextBoxWidthConstraint = _intervalTextBox.WidthAnchor.ConstraintEqualTo(32);
		_intervalTextBoxWidthConstraint.Active = true;

		_startButton.Activated += StartButton_Activated;
		_minusButton.Activated += MinusButton_Activated;
		_plusButton.Activated += PlusButton_Activated;
		_unitButton.Activated += UnitButton_Activated;
		_visitButton.Activated += VisitButton_Activated;
	}
}