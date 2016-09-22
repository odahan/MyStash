using CoreGraphics;
using UIKit;

namespace MyStash.iOS.Crouton
{
    public class MessageWindow : UIWindow
    {
        public override UIView HitTest(CGPoint point, UIEvent uievent)
        {
            var hitView = base.HitTest(point, uievent);
            if (Equals(hitView, RootViewController.View))
                hitView = null;
            return hitView;
        }
    }
}

