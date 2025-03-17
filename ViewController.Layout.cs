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
    private NSView _firstRow;
    private int _spacing = 5;
    private int _padding = 10;
    private NSButton _startButton;
       public override void ViewWillAppear()
    {
        base.ViewWillAppear();

        View.Window.BackgroundColor = NSColor.Clear;
        View.Window.IsOpaque = false;
        View.Window.HasShadow = false;
    }

public override void ViewDidLoad()
{
    base.ViewDidLoad();

    // Create the first row with a border
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
        TranslatesAutoresizingMaskIntoConstraints = false
    };

    var minusButton = new NSButton
    {
        Title = "-",
        TranslatesAutoresizingMaskIntoConstraints = false
    };

    var plusButton = new NSButton
    {
        Title = "+",
        TranslatesAutoresizingMaskIntoConstraints = false
    };

    var textBox = new NSTextField
    {
        StringValue = "1",
        TranslatesAutoresizingMaskIntoConstraints = false
    };

    var minutesButton = new NSButton
    {
        Title = "Minutes",
        TranslatesAutoresizingMaskIntoConstraints = false
    };

    var bottomTextBox = new NSTextField
    {
        PlaceholderString = "Enter text",
        TranslatesAutoresizingMaskIntoConstraints = false
    };

    var smallButton = new NSButton
    {
        Title = "Small",
        TranslatesAutoresizingMaskIntoConstraints = false
    };

    // Create a stack view for the minus, textbox, plus, and minutes button
    var stackView = new NSStackView
    {
        Orientation = NSUserInterfaceLayoutOrientation.Horizontal,
        TranslatesAutoresizingMaskIntoConstraints = false,
        Distribution = NSStackViewDistribution.FillProportionally,
        Spacing = _spacing
    };

    stackView.AddArrangedSubview(minusButton);
    stackView.AddArrangedSubview(textBox);
    stackView.AddArrangedSubview(plusButton);
    stackView.AddArrangedSubview(minutesButton);

    secondRow.AddSubview(_startButton);
    secondRow.AddSubview(stackView);
    secondRow.AddSubview(bottomTextBox);
    secondRow.AddSubview(smallButton);

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

        bottomTextBox.LeftAnchor.ConstraintEqualTo(secondRow.LeftAnchor, _spacing),
        bottomTextBox.TopAnchor.ConstraintEqualTo(_startButton.BottomAnchor, _spacing),
        bottomTextBox.RightAnchor.ConstraintEqualTo(smallButton.LeftAnchor, -_spacing),

        smallButton.RightAnchor.ConstraintEqualTo(secondRow.RightAnchor, -_spacing),
        smallButton.CenterYAnchor.ConstraintEqualTo(bottomTextBox.CenterYAnchor)
    });

    // Add a minimum width constraint to the start button
    var _startButtonMinWidthConstraint = _startButton.WidthAnchor.ConstraintGreaterThanOrEqualTo(50);
    _startButtonMinWidthConstraint.Priority = 999; // Set a high priority but lower than required constraints
    _startButtonMinWidthConstraint.Active = true;

    _startButton.Activated += StartButton_Activated;
    minusButton.Activated += MinusButton_Activated;
    plusButton.Activated += PlusButton_Activated;
    smallButton.Activated += SmallButton_Activated;
}

private void MinusButton_Activated(object sender, EventArgs e)
{
    // Handle Minus button click
}

private void PlusButton_Activated(object sender, EventArgs e)
{
    // Handle Plus button click
}

private void SmallButton_Activated(object sender, EventArgs e)
{
    // Handle Small button click
}
}