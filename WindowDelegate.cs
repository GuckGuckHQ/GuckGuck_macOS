using AppKit;
using Foundation;

namespace GuckGuck
{
    public class WindowDelegate : NSWindowDelegate
    {
        private readonly ViewController _viewController;

        public WindowDelegate(ViewController viewController)
        {
            _viewController = viewController;
        }

        public override void DidMove(NSNotification notification)
        {
            _viewController.OnWindowMoved();
        }
    }
}