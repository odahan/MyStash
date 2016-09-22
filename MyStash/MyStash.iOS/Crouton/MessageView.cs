//
// MessageView.cs
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
using System.Drawing;
using CoreGraphics;
using Foundation;
using MyStash.Crouton;
using UIKit;

namespace MyStash.iOS.Crouton
{
    public sealed class MessageView : UIView
    {
        private static readonly UIFont titleFont;
        private static readonly UIFont descriptionFont;
        private static readonly UIColor titleColor;
        private static readonly UIColor descriptionColor;
        private const float Padding = 12.0f;
        private const float IconSize = 36.0f;
        private const float TextOffset = 2.0f;
        private float height;
        private float width;

        static MessageView()
        {
            titleFont = UIFont.BoldSystemFontOfSize(16.0f);
            descriptionFont = UIFont.SystemFontOfSize(14.0f);
            titleColor = UIColor.FromWhiteAlpha(1.0f, 1.0f);
            descriptionColor = UIColor.FromWhiteAlpha(1.0f, 1.0f);
        }

        internal MessageView(string title, string description, ToastNotificationType type, Action<bool> onDismiss, TimeSpan duration)
            : this((NSString)title, (NSString)description, type)
        {
            OnDismiss = onDismiss;
            DisplayDelay = duration.TotalSeconds;
        }

        private MessageView(NSString title, NSString description, ToastNotificationType type)
            : base(RectangleF.Empty)
        {
            BackgroundColor = UIColor.Clear;
            ClipsToBounds = false;
            UserInteractionEnabled = true;
            this.title = title;
            this.description = description;
            messageType = type;
            Height = 0.0f;
            Width = 0.0f;
            Hit = false;

            NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification, orientationChanged);
        }

        public Action<bool> OnDismiss { get; set; }

        public bool Hit { get; set; }

        public float Height
        {
            get
            {
                if (Math.Abs(height) < 0.0001)
                {
                    CGSize titleLabelSize = titleSize();
                    CGSize descriptionLabelSize = descriptionSize();
                    height = (float) Math.Max((Padding*2) + titleLabelSize.Height + descriptionLabelSize.Height, (Padding*2) + IconSize);
                }
                return height;
            }
            private set { height = value; }
        }

        public float Width
        {
            get
            {
                if (Math.Abs(width) < 0.0001)
                    width = getStatusBarFrame().Width;
                return width;
            }
            private set { width = value; }
        }
        public double DisplayDelay { get; }

        internal MessageBarStyleSheet stylesheetProvider { private get; set; }

        private NSString title { get; }

        private NSString description { get; }

        private ToastNotificationType messageType { get; }

        private float availableWidth
        {
            get
            {
                float maxWidth = (Width - (Padding*3) - IconSize);
                return maxWidth;
            }
        }


        private void orientationChanged(NSNotification notification)
        {
            Frame = new RectangleF((float) Frame.X, (float) Frame.Y, getStatusBarFrame().Width, (float) Frame.Height);
            SetNeedsDisplay();
        }

        private RectangleF getStatusBarFrame()
        {
            var windowFrame = orientFrame(UIApplication.SharedApplication.KeyWindow.Frame);
            var statusFrame = orientFrame(UIApplication.SharedApplication.StatusBarFrame);

            return new RectangleF((float) windowFrame.X, (float) windowFrame.Y, (float) windowFrame.Width,
                (float) statusFrame.Height);
        }

        private CGRect orientFrame(CGRect frame)
        {
            if ((IsDeviceLandscape(UIDevice.CurrentDevice.Orientation) ||
                IsStatusBarLandscape(UIApplication.SharedApplication.StatusBarOrientation)) &&
                !isRunningOnIosVersionOrLater(8) /*http://stackoverflow.com/questions/24150359/is-uiscreen-mainscreen-bounds-size-becoming-orientation-dependent-in-ios8*/)
            {
                frame = new RectangleF((float) frame.X, (float) frame.Y, (float) frame.Height, (float) frame.Width);
            }
            return frame;
        }

        private bool IsDeviceLandscape(UIDeviceOrientation orientation)
        {
            return orientation == UIDeviceOrientation.LandscapeLeft || orientation == UIDeviceOrientation.LandscapeRight;
        }

        private bool IsStatusBarLandscape(UIInterfaceOrientation orientation)
        {
            return orientation == UIInterfaceOrientation.LandscapeLeft ||
                   orientation == UIInterfaceOrientation.LandscapeRight;
        }

        public override void Draw(CGRect rect)
        {
            var context = UIGraphics.GetCurrentContext ();

            var styleSheet = stylesheetProvider;
            context.SaveState ();

            styleSheet.BackgroundColorForMessageType (messageType).SetColor ();
            context.FillRect (rect);
            context.RestoreState ();
            context.SaveState ();

            context.BeginPath ();
            context.MoveTo (0, rect.Size.Height);
            context.SetStrokeColor(styleSheet.StrokeColorForMessageType (messageType).CGColor);
            context.SetLineWidth (1);
            context.AddLineToPoint (rect.Size.Width, rect.Size.Height);
            context.StrokePath ();
            context.RestoreState ();
            context.SaveState ();
            
            float xOffset = Padding;
            float yOffset = Padding;
            var icon = styleSheet.IconImageForMessageType(messageType);
            icon?.Draw(new RectangleF(xOffset, yOffset, IconSize, IconSize));
            context.SaveState ();
                
            yOffset -= TextOffset;
            xOffset += (icon == null ? 0 : IconSize) + Padding;
            var titleLabelSize = titleSize();
            if (string.IsNullOrEmpty (title) && !string.IsNullOrEmpty (description)) {
                yOffset = (float)(Math.Ceiling (rect.Size.Height * 0.5) - Math.Ceiling (titleLabelSize.Height * 0.5) - TextOffset);
            }

            titleColor.SetColor ();
                
            var titleRectangle = new RectangleF (xOffset, yOffset, (float) titleLabelSize.Width + 5, (float) titleLabelSize.Height + 5);
			title.DrawString (titleRectangle, new UIStringAttributes {Font=titleFont, ForegroundColor = titleColor});
            yOffset += (float)titleLabelSize.Height;

            var descriptionLabelSize = descriptionSize();
            descriptionColor.SetColor();
            var descriptionRectangle = new RectangleF(xOffset, yOffset, (float)descriptionLabelSize.Width + Padding, (float)descriptionLabelSize.Height);
			description.DrawString (descriptionRectangle, new UIStringAttributes {Font=descriptionFont, ForegroundColor = descriptionColor});
        }

        private CGSize titleSize()
        {
            var boundedSize = new SizeF(availableWidth, float.MaxValue);
            CGSize titleLabelSize;
            if (!isRunningOnIosVersionOrLater(7))
            {
                var attr = new UIStringAttributes(NSDictionary.FromObjectAndKey(titleFont, (NSString) titleFont.Name));
                titleLabelSize = title.GetBoundingRect(boundedSize, NSStringDrawingOptions.TruncatesLastVisibleLine, attr, null).Size;
            }
            else
            {
                titleLabelSize = title.StringSize(titleFont, boundedSize, UILineBreakMode.TailTruncation);
            }
            return titleLabelSize;
        }

        private CGSize descriptionSize()
        {
            var boundedSize = new SizeF(availableWidth, float.MaxValue);
            CGSize descriptionLabelSize;
            if (!isRunningOnIosVersionOrLater(7))
            {
                var attr = new UIStringAttributes(NSDictionary.FromObjectAndKey(titleFont, (NSString) titleFont.Name));
                descriptionLabelSize = description.GetBoundingRect(boundedSize, NSStringDrawingOptions.TruncatesLastVisibleLine, attr, null).Size;
            }
            else
            {
                descriptionLabelSize = description.StringSize(descriptionFont, boundedSize, UILineBreakMode.TailTruncation);
            }
            return descriptionLabelSize;
        }

        private bool isRunningOnIosVersionOrLater(int majorVersion)
        {
            Version version = new Version(UIDevice.CurrentDevice.SystemVersion);
            return version.Major >= majorVersion;
        }
    }
}
