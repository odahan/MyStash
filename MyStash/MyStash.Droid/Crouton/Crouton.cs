using System;
using Android.App;
using Android.Views;
using Android.Views.Animations;

namespace MyStash.Droid.Crouton
{
    public class Crouton : Java.Lang.Object, View.IOnClickListener
    {
        private readonly View customView;
        private Action<bool> onClick;

        private Activity activity;
        private Animation inAnimation;
        private Animation outAnimation;

        public long DurationInMilliseconds { get; private set; }

        public object DataContext { get; private set; }

        public Crouton(Activity activity, View customView, int durationInMs, Action<bool> onClick = null, object dataContext = null)
        {
            if (activity == null) throw new ArgumentNullException(nameof(activity));
            if (customView == null) throw new ArgumentNullException(nameof(customView));

            this.activity = activity;
            this.customView = customView;
            this.onClick = onClick;
            customView.SetOnClickListener(this);
            DurationInMilliseconds = durationInMs;
            DataContext = dataContext;
        }
        
        public void Show()
        {
            Manager.Instance.Add(this);
        }

        public Animation GetInAnimation()
        {
            if ((null == inAnimation) && (null != activity))
            {
                measureCroutonView();
                inAnimation = DefaultAnimationsBuilder.BuildDefaultSlideInDownAnimation(GetView());
            }
            return inAnimation;
        }

        public Animation GetOutAnimation()
        {
            if ((null == outAnimation) && (null != activity))
            {
                outAnimation = DefaultAnimationsBuilder.BuildDefaultSlideOutUpAnimation(GetView());
            }
            return outAnimation;
        }
        
        public bool IsShowing()
        {
            return (null != activity) && isCustomViewNotNull();
        }
        
        private bool isCustomViewNotNull()
        {
            return customView?.Parent != null;
        }

        public void DetachActivity()
        {
            activity = null;
        }

        public Activity GetActivity()
        {
            return activity;
        }
        
        public View GetView()
        {
            return customView;
        }

        private void measureCroutonView()
        {
            var view = GetView();
            var widthSpec = View.MeasureSpec.MakeMeasureSpec(activity.Window.DecorView.MeasuredWidth, MeasureSpecMode.AtMost);
            view.Measure(widthSpec, View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
        }

        public void OnRemoved()
        {
            onClick?.Invoke(false);
            onClick = null;
        }

        public void OnDisplayed()
        {
        }

        public void DetachLifecycleCallback()
        {
        }

        public void OnClick(View view)
        {
            onClick?.Invoke(true);
            Manager.Instance.RemoveCrouton(this);
            onClick = null;
        }
    }
}