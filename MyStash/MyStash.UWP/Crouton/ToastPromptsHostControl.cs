using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace MyStash.UWP.Crouton
{
    public class ToastPromptsHostControl : Grid
    {
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly Queue<NotificationItem> notificationQueue = new Queue<NotificationItem>();
        private readonly List<ToastItem> toastItems = new List<ToastItem>();
        private static ToastPromptsHostControl lastUsedInstance = null;
        private readonly ItemsControl activeItemsControl;

        public static int MaxToastCount { get; set; } = 3;

        public ToastPromptsHostControl()
        {
            activeItemsControl = new ItemsControl { Background = new SolidColorBrush(Colors.Transparent) };
            Children.Add(activeItemsControl);
            
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_OnTick;
            timer.Start();
            lastUsedInstance = this; //instance is defined in xaml
        }

        public static void Clear()
        {
            lastUsedInstance?.clearAll();
        }

        public static void EnqueueItem(UIElement content, Action<bool> submitAction, Brush bgBrush, bool tappable, TimeSpan timeout, bool showCloseButton)
        {
            if (lastUsedInstance == null)
                return;

            lastUsedInstance.notificationQueue.Enqueue(new NotificationItem(content, bgBrush, submitAction, tappable, timeout, showCloseButton));
            lastUsedInstance.tryDequeue();
        }
        
        private void Timer_OnTick(object sender, object o)
        {
            foreach (ToastItem toastItem in toastItems.ToArray())
            {
                if (!toastItem.IsFinalizing && toastItem.Started + toastItem.NotificationItem.ToastTimeout <= DateTime.Now)
                {
                    toastItem.NotificationItem.PerformAction(false);
                    removeToast(toastItem);
                }
            }
        }

        private void updateVisibility(bool forceVisible = false)
        {
            if (forceVisible)
            {
                Visibility = Visibility.Visible;
                return;
            }

            if (Visibility == Visibility.Visible)
            {
                if (!toastItems.Any() && !notificationQueue.Any())
                    Visibility = Visibility.Collapsed;
            }
            else
            {
                if (toastItems.Any() || notificationQueue.Any())
                    Visibility = Visibility.Visible;
            }
        }

        private void removeToast(ToastItem toastItem)
        {
            if (toastItem.IsFinalizing)
                return;

            toastItem.IsFinalizing = true;
            toastItem.FinalizingActions();

            var storyboard = new Storyboard();
            var projectionAnimation = new DoubleAnimation { Duration = new Duration(TimeSpan.FromSeconds(0.6)), To = 90 };
            storyboard.Children.Add(projectionAnimation);
            Storyboard.SetTargetProperty(projectionAnimation, new PropertyPath("(UIElement.Projection).(PlaneProjection.RotationX)").Path);
            Storyboard.SetTarget(projectionAnimation, toastItem.Element);
            var item = toastItem;

            EventHandler<object> completedHandler = null;
            completedHandler = (s, ea) =>
            {
                toastItems.Remove(item);
                activeItemsControl.Items?.Remove(item.Element);
                updateVisibility();
                tryDequeue();
                storyboard.Completed -= completedHandler;
            };

            storyboard.Completed += completedHandler;
            storyboard.Begin();
        }

        private void tryDequeue()
        {
            if (toastItems.Count < MaxToastCount && notificationQueue.Any())
            {
                var item = notificationQueue.Dequeue();
                updateVisibility(true);
                appendToast(item);
                tryDequeue();
            }
        }

        private void clearAll()
        {
            notificationQueue.Clear();
            foreach (var toastItem in toastItems)
            {
                removeToast(toastItem);
            }
        }

        private void appendToast(NotificationItem notification)
        {
            //root layout
            var layoutGrid = new Grid();
            var toastItem = new ToastItem(layoutGrid, DateTime.Now, notification);
            layoutGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            layoutGrid.Height = 70;
            layoutGrid.Margin = new Thickness(0, 0, 0, 2); //2 - a margin between toasts
            layoutGrid.Background = notification.Brush;
            layoutGrid.Projection = new PlaneProjection();
            layoutGrid.RenderTransformOrigin = new Point(0.5, 0.5);
            layoutGrid.RenderTransform = new CompositeTransform { TranslateX = -800 };
            layoutGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });


            Button closeButton = null;
            if (notification.ShowCloseButton)
            {
                layoutGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40.0) });
                //close button
                closeButton = new Button();
                SetColumn(closeButton, 1);
                closeButton.Tag = toastItem;
                closeButton.Width = 40;
                closeButton.Height = 90;
                closeButton.Content = "\u2716"; //X symbol
                closeButton.BorderThickness = new Thickness(0);
                closeButton.FontSize = 28;
                closeButton.Padding = new Thickness(0);
                closeButton.Opacity = 0.4;
                closeButton.Margin = new Thickness(-12, -15, -20, -8); //TODO: fix it
                closeButton.HorizontalAlignment = HorizontalAlignment.Right;
                closeButton.Click += CloseButton_OnTap;
                layoutGrid.Children.Add(closeButton);
            }

            //toast content
            var contentGrid = new Grid {Tag = toastItem};
            contentGrid.Children.Add(notification.Content);
            layoutGrid.Children.Add(contentGrid);
            contentGrid.Tapped += ContentGrid_OnTap;

            toastItem.FinalizingActions += () => contentGrid.Tapped -= ContentGrid_OnTap;

            if (closeButton != null)
            {
                toastItem.FinalizingActions += () => closeButton.Tapped -= CloseButton_OnTap;
            }

            //appear animation
            var animation = new DoubleAnimation { Duration = new Duration(TimeSpan.FromSeconds(0.2)), To = 0 };
            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, layoutGrid);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)").Path);
            storyboard.Begin();

            activeItemsControl.Items?.Add(layoutGrid);
            toastItems.Add(toastItem);
        }

        private void ContentGrid_OnTap(object sender, object e)
        {
            var toastItem = ((FrameworkElement)sender).Tag as ToastItem;
            if (toastItem == null)
                return;

            if (toastItem.IsFinalizing || !toastItem.NotificationItem.Tappable) return;
            removeToast(toastItem);
            toastItem.NotificationItem.PerformAction(true);
        }

        private void CloseButton_OnTap(object sender, object e)
        {
            var toastItem = ((FrameworkElement)sender).Tag as ToastItem;
            if (toastItem == null)
                return;

            toastItem.NotificationItem.PerformAction(false);

            removeToast(toastItem);
        }

        private class ToastItem
        {
            public FrameworkElement Element { get; set; }
            public DateTime Started { get; set; }
            public bool IsFinalizing { get; set; }
            public NotificationItem NotificationItem { get; set; }
            public Action FinalizingActions { get; set; }

            public ToastItem(FrameworkElement element, DateTime started, NotificationItem notification)
            {
                Element = element;
                Started = started;
                NotificationItem = notification;
            }
        }

        private class NotificationItem
        {
            public UIElement Content { get; set; }
            public Brush Brush { get; set; }
            public Action<bool> Action { get; set; }
            public bool Tappable { get; set; }
            public TimeSpan ToastTimeout { get; set; }
            public bool ShowCloseButton { get; set; }

            public NotificationItem(UIElement content, Brush brush, Action<bool> action, bool tappable, TimeSpan toastTimeout, bool showCloseButton)
            {
                Content = content;
                Brush = brush;
                Action = action;
                Tappable = tappable;
                ToastTimeout = toastTimeout;
                ShowCloseButton = showCloseButton;
            }

            public void PerformAction(bool result)
            {
                if (Action == null) return;
                Action(result);
                Action = null;
            }
        }
    }
}
