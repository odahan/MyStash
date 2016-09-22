using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Xamarin.Forms;

namespace MyStash.Controls
{
    /// <summary>
	/// New WrapLayout
	/// </summary>
	/// <author>Jason Smith</author>
	public class WrapLayout : Layout<View>
    {
        private readonly Dictionary<View, SizeRequest> cache = new Dictionary<View, SizeRequest>();

        /// <summary>
        /// Horizontal spacing between children
        /// </summary>
        [LocalizationRequired(false)]
        public static readonly BindableProperty HorizontalSpacingProperty =
            BindableProperty.Create("HorizontalSpacing", typeof(double), typeof(WrapLayout), 4d,
                propertyChanged: (bindable, oldvalue, newvalue) => ((WrapLayout)bindable).cache.Clear());

        /// <summary>
        /// Vertical spacing between children
        /// </summary>
        [LocalizationRequired(false)]
        public static readonly BindableProperty VerticalSpacingProperty =
            BindableProperty.Create("VerticalSpacing", typeof(double), typeof(WrapLayout), 4d,
                propertyChanged: (bindable, oldvalue, newvalue) => ((WrapLayout)bindable).cache.Clear());

        /// <summary>
        /// Horizontal spacing added between children
        /// </summary>
        /// <value>The spacing.</value>
        public double HorizontalSpacing
        {
            get { return (double)GetValue(HorizontalSpacingProperty); }
            set { SetValue(HorizontalSpacingProperty, value); }
        }

        /// <summary>
        /// Vertical spacing added between children
        /// </summary>
        /// <value>The spacing.</value>
        public double VerticalSpacing
        {
            get { return (double)GetValue(VerticalSpacingProperty); }
            set { SetValue(VerticalSpacingProperty, value); }
        }

        public WrapLayout()
        {
            VerticalOptions = HorizontalOptions = LayoutOptions.FillAndExpand;
        }

        protected override void OnChildMeasureInvalidated()
        {
            base.OnChildMeasureInvalidated();
            cache.Clear();
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            double lastX;
            double lastY;
            alignControls(widthConstraint, heightConstraint, out lastX, out lastY);

            return new SizeRequest(new Size(lastX, lastY));
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            double lastX, lastY;
            var layout = alignControls(width, height, out lastX, out lastY);

            if (layout == null || layout.Count < 1) return;
            foreach (var t in layout)
            {
                // pour centrer les lignes qui comptent moins que le maximum
                //var offset = (int) ((width - t.Last().Item2.Right)/2);
                var offset = 0;
                foreach (var item in t)
                {
                    if (item.Item2.Left < 0 || item.Item2.Right < 0) LayoutChildIntoBoundingRegion(item.Item1, new Rectangle(100000, 100000, 0, 0));
                    else
                    {
                        var location = new Rectangle(item.Item2.X + x + offset, item.Item2.Y + y, item.Item2.Width, item.Item2.Height);
                        LayoutChildIntoBoundingRegion(item.Item1, location);
                    }
                }
            }
        }

        private List<List<Tuple<View, Rectangle>>> alignControls(double width, double height, out double lastX, out double lastY)
        {
            double startX = 0;
            double startY = 0;
            double right = width;
            double nextY = 0;

            lastX = 0;
            lastY = 0;

            var result = new List<List<Tuple<View, Rectangle>>>();
            var currentList = new List<Tuple<View, Rectangle>>();

            foreach (var child in Children)
            {
                SizeRequest sizeRequest;
                if (!cache.TryGetValue(child, out sizeRequest))
                {
                    cache[child] = sizeRequest = child.Measure(double.PositiveInfinity, double.PositiveInfinity);
                }

                var paddedWidth = sizeRequest.Request.Width + HorizontalSpacing;
                var paddedHeight = sizeRequest.Request.Height + VerticalSpacing;

                if (startX + sizeRequest.Request.Width > right)
                {
                    startX = 0;
                    startY += nextY;

                    if (currentList.Count > 0)
                    {
                        result.Add(currentList);
                        currentList = new List<Tuple<View, Rectangle>>();
                    }
                }
                //if (startY + sizeRequest.Request.Height >= height) Debugger.Break();

                if (startY + sizeRequest.Request.Height < height)
                {
                    currentList.Add(new Tuple<View, Rectangle>(child,
                        new Rectangle(startX, startY, sizeRequest.Request.Width, sizeRequest.Request.Height)));
                    lastX = Math.Max(lastX, startX + paddedWidth);
                    lastY = Math.Max(lastY, startY + paddedHeight);

                    nextY = Math.Max(nextY, paddedHeight);
                    startX += paddedWidth;
                }
                else
                {
                    currentList.Add(new Tuple<View, Rectangle>(child,
                        new Rectangle(-1, -1, -1, -1)));
                }


            }
            result.Add(currentList);
            return result;
        }
    }
}
