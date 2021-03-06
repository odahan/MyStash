//
// DefaultMessageBarStyleSheet.cs
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


using MyStash.Crouton;
using UIKit;

namespace MyStash.iOS.Crouton
{
    public sealed class MessageBarStyleSheet 
    {
        private const float Alpha = 0.96f;
        private const string ErrorIcon = "icon-error.png";
        private const string SuccessIcon = "icon-success.png";
        private const string InfoIcon = "icon-info.png";
        private const string WarningIcon = "icon-warning.png";

        private readonly UIColor errorBackgroundColor;
        private readonly UIColor successBackgroundColor;
        private readonly UIColor infoBackgroundColor;
        private readonly UIColor errorStrokeColor;
        private readonly UIColor successStrokeColor;
        private readonly UIColor infoStrokeColor;
        private readonly UIColor warningBackgroundColor;
        private readonly UIColor warningStrokeColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBarStyleSheet"/> class.
        /// </summary>
        public MessageBarStyleSheet ()
        {
            errorBackgroundColor = UIColor.FromRGBA(206 / 255.0f, 24 / 255.0f, 24 / 255.0f, Alpha);
            successBackgroundColor = UIColor.FromRGBA(69 / 255.0f, 145 / 255.0f, 34 / 255.0f, Alpha);
            infoBackgroundColor = UIColor.FromRGBA(42 / 255.0f, 112 / 255.0f, 153 / 255.0f, Alpha);
            warningBackgroundColor = UIColor.FromRGBA(180 / 255.0f, 125 / 255.0f, 1 / 255.0f, Alpha);

            errorStrokeColor = UIColor.FromRGBA (0.949f, 0.580f, 0.0f, 1.0f);
            successStrokeColor = UIColor.FromRGBA (0.0f, 0.772f, 0.164f, 1.0f);
            infoStrokeColor = UIColor.FromRGBA(0.0f, 0.415f, 0.803f, 1.0f);
            warningStrokeColor = UIColor.FromRGBA(0.0f, 0.415f, 0.803f, 1.0f);
        }

        /// <summary>
        /// Provides the background colour for message type
        /// </summary>
        /// <returns>The background colour for message type.</returns>
        /// <param name="type">Message type</param>
        public UIColor BackgroundColorForMessageType(ToastNotificationType type)
        {
            UIColor backgroundColor = null;
            switch (type)
            {
                case ToastNotificationType.Error:
                    backgroundColor = errorBackgroundColor;
                    break;
                case ToastNotificationType.Success:
                    backgroundColor = successBackgroundColor;
                    break;
                case ToastNotificationType.Info:
                    backgroundColor = infoBackgroundColor;
                    break;
                case ToastNotificationType.Warning:
                    backgroundColor = warningBackgroundColor;
                    break;
            }

            return backgroundColor;
        }

        /// <summary>
        /// Provides the stroke colour for message type
        /// </summary>
        /// <returns>The stroke colour for message type.</returns>
        /// <param name="type">Message type</param>
        public UIColor StrokeColorForMessageType(ToastNotificationType type)
        {
            UIColor strokeColor = null;
            switch (type)
            {
                case ToastNotificationType.Error:
                    strokeColor = errorStrokeColor;
                    break;
                case ToastNotificationType.Success:
                    strokeColor = successStrokeColor;
                    break;
                case ToastNotificationType.Info:
                    strokeColor = infoStrokeColor;
                    break;
                case ToastNotificationType.Warning:
                    strokeColor = warningStrokeColor;
                    break;
            }

            return strokeColor;
        }

        /// <summary>
        /// Provides the icon for message type
        /// </summary>
        /// <returns>The icon for message type.</returns>
        /// <param name="type">Message type</param>
        public UIImage IconImageForMessageType(ToastNotificationType type)
        {
            UIImage iconImage = null;
            switch (type)
            {
                case ToastNotificationType.Error:
                    iconImage = UIImage.FromBundle(ErrorIcon);
                    break;
                case ToastNotificationType.Success:
                    iconImage = UIImage.FromBundle(SuccessIcon);
                    break;
                case ToastNotificationType.Info:
                    iconImage = UIImage.FromBundle(InfoIcon);
                    break;
                case ToastNotificationType.Warning:
                    iconImage = UIImage.FromBundle(WarningIcon);
                    break;
            }

            return iconImage;
        }
    }
}
