using Android.Views;
using Android.Views.Animations;

namespace MyStash.Droid.Crouton
{
    public static class DefaultAnimationsBuilder
    {
        private const long Duration = 400;
        private static Animation slideInDownAnimation, slideOutUpAnimation;
        private static int lastInAnimationHeight, lastOutAnimationHeight;

        public static Animation BuildDefaultSlideInDownAnimation(View croutonView)
        {
            if (lastInAnimationHeight == croutonView.MeasuredHeight && (null != slideInDownAnimation)) return slideInDownAnimation;
            slideInDownAnimation = new TranslateAnimation(0, 0, -croutonView.MeasuredHeight, 0) {Duration = Duration};
            lastInAnimationHeight = croutonView.MeasuredHeight;
            return slideInDownAnimation;
        }
        
        public static Animation BuildDefaultSlideOutUpAnimation(View croutonView)
        {
            if (lastOutAnimationHeight == croutonView.MeasuredHeight && (null != slideOutUpAnimation)) return slideOutUpAnimation;
            slideOutUpAnimation = new TranslateAnimation(0, 0, 0, -croutonView.MeasuredHeight) {Duration = Duration};
            lastOutAnimationHeight = croutonView.MeasuredHeight;
            return slideOutUpAnimation;
        }
    }
}