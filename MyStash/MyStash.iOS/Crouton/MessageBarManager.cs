//
// MessageBarManager.cs
//
// Author:
//       Prashant Cholachagudda <pvc@outlook.com>
//
// Copyright (c) 2013 Prashant Cholachagudda
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Drawing;
using Foundation;
using MyStash.Crouton;
using UIKit;

namespace MyStash.iOS.Crouton
{
    public class MessageBarManager : NSObject
    {
        private const float DismissAnimationDuration = 0.25f;
        private MessageWindow messageWindow;
        private readonly Queue<MessageView> messageBarQueue;
        private static MessageBarManager instance;
        private readonly float messageBarOffset;
        private bool messageVisible;

        public static MessageBarManager SharedInstance => instance ?? (instance = new MessageBarManager());

        private MessageBarManager()
        {
            messageBarQueue = new Queue<MessageView>();
            messageVisible = false;
            messageBarOffset = 20;
        }

        private UIView messageWindowView => getMessageBarViewController().View;

        /// <summary>
        /// Shows the message
        /// </summary>
        /// <param name="title">Messagebar title</param>
        /// <param name="description">Messagebar description</param>
        /// <param name="type">Message type</param>
        /// <param name = "onDismiss">OnDismiss callback</param>
        /// <param name="duration"></param>
        /// <param name="styleSheet"></param>
        public void ShowMessage(string title, string description, ToastNotificationType type, Action<bool> onDismiss, TimeSpan duration, MessageBarStyleSheet styleSheet = null)
        {
            var messageView = new MessageView(title, description, type, onDismiss, duration)
                              {
                                  stylesheetProvider = styleSheet,
                                  Hidden = true
                              };

            messageWindowView.AddSubview(messageView);
            messageWindowView.BringSubviewToFront(messageView);

            messageBarQueue.Enqueue(messageView);

            if (!messageVisible)
            {
                showNextMessage();
            }
        }

        private void showNextMessage()
        {
            if (messageBarQueue.Count > 0)
            {
                messageVisible = true;
                MessageView messageView = messageBarQueue.Dequeue();
                messageView.Frame = new RectangleF(0, -messageView.Height, messageView.Width, messageView.Height);
                messageView.Hidden = false;
                messageView.SetNeedsDisplay();

                var gest = new UITapGestureRecognizer(messageTapped);
                messageView.AddGestureRecognizer(gest);

                UIView.Animate(DismissAnimationDuration, () => messageView.Frame = new RectangleF((float)messageView.Frame.X,
                        (float)(messageBarOffset + messageView.Frame.Y + messageView.Height), messageView.Width, messageView.Height));

                //Need a better way of dissmissing the method
                var timer = new System.Threading.Timer(dismissMessage, messageView, TimeSpan.FromSeconds(messageView.DisplayDelay), TimeSpan.FromMilliseconds(-1));
            }
        }

        /// <summary>
        /// Hides all messages
        /// </summary>
        public void HideAll()
        {
            var subviews = messageWindowView.Subviews;

            foreach (var subview in subviews)
            {
                var view = subview as MessageView;
                if (view == null) continue;
                var currentMessageView = view;
                currentMessageView.RemoveFromSuperview();
            }

            messageVisible = false;
            messageBarQueue.Clear();
            CancelPreviousPerformRequest(this);
        }

        private void messageTapped(UIGestureRecognizer recognizer)
        {
            var view = recognizer.View as MessageView;
            if (view != null)
                dismissMessage(view, true);
        }

        private void dismissMessage(object messageView)
        {
            var view = messageView as MessageView;
            if (view != null)
                InvokeOnMainThread(() => dismissMessage(view, false));
        }

        private void dismissMessage(MessageView messageView, bool clicked)
        {
            if (messageView != null && !messageView.Hit)
            {
                messageView.Hit = true;
                UIView.Animate(DismissAnimationDuration, () => messageView.Frame =
                    new RectangleF((float)messageView.Frame.X, (float)-(messageView.Frame.Height - messageBarOffset), (float)messageView.Frame.Width, (float)messageView.Frame.Height),
                    () =>
                    {
                        messageVisible = false;
                        messageView.RemoveFromSuperview();

                        var action = messageView.OnDismiss;
                        action?.Invoke(clicked);

                        if (messageBarQueue.Count > 0)
                            showNextMessage();
                    });
            }
        }

        private MessageBarViewController getMessageBarViewController()
        {
            if (messageWindow == null)
            {
                messageWindow = new MessageWindow
                {
                    Frame = UIApplication.SharedApplication.KeyWindow.Frame,
                    Hidden = false,
                    WindowLevel = UIWindowLevel.Normal,
                    BackgroundColor = UIColor.Clear,
                    RootViewController = new MessageBarViewController()
                };
            }

            return (MessageBarViewController)messageWindow.RootViewController;
        }
    }
}
