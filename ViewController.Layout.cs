namespace GuckGuck;

public partial class ViewController : NSViewController
{
    private NSView _firstRow;
    private int _spacing = 5;
    private int _padding = 10;
    private NSButton _startButton;
    private NSButton _minusButton;
    private NSButton _plusButton;
    private NSTextField _textBox;
    private NSButton _unitButton;
    private NSTextField _urlTextBox;
    private NSButton _visitButton;

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

        _minusButton = new NSButton
        {
            Title = "-",
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        _plusButton = new NSButton
        {
            Title = "+",
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        _textBox = new NSTextField
        {
            StringValue = "1",
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        _unitButton = new NSButton
        {
            Title = "Minutes",
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        _urlTextBox = new NSTextField
        {
            PlaceholderString = "Enter text",
            TranslatesAutoresizingMaskIntoConstraints = false
        };

        _visitButton = new NSButton
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

        stackView.AddArrangedSubview(_minusButton);
        stackView.AddArrangedSubview(_textBox);
        stackView.AddArrangedSubview(_plusButton);
        stackView.AddArrangedSubview(_unitButton);

        secondRow.AddSubview(_startButton);
        secondRow.AddSubview(stackView);
        secondRow.AddSubview(_urlTextBox);
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

            _urlTextBox.LeftAnchor.ConstraintEqualTo(secondRow.LeftAnchor, _spacing),
            _urlTextBox.TopAnchor.ConstraintEqualTo(_startButton.BottomAnchor, _spacing),
            _urlTextBox.RightAnchor.ConstraintEqualTo(_visitButton.LeftAnchor, -_spacing),

            _visitButton.RightAnchor.ConstraintEqualTo(secondRow.RightAnchor, -_spacing),
            _visitButton.CenterYAnchor.ConstraintEqualTo(_urlTextBox.CenterYAnchor)
        });

        // Add a minimum width constraint to the start button
        var _startButtonMinWidthConstraint = _startButton.WidthAnchor.ConstraintGreaterThanOrEqualTo(50);
        _startButtonMinWidthConstraint.Priority = 999; // Set a high priority but lower than required constraints
        _startButtonMinWidthConstraint.Active = true;

        _startButton.Activated += StartButton_Activated;
        _minusButton.Activated += MinusButton_Activated;
        _plusButton.Activated += PlusButton_Activated;
        _unitButton.Activated += UnitButton_Activated;
        _visitButton.Activated += VisitButton_Activated;
    }
}