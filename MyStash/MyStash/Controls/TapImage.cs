using System;
using System.Windows.Input;
using FFImageLoading.Forms;
using JetBrains.Annotations;
using Xamarin.Forms;

namespace MyStash.Controls
{
    public class TapImage : CachedImage
    {
        private readonly TapGestureRecognizer gesture;
        public TapImage()
        {
            gesture = new TapGestureRecognizer();
            gesture.Tapped += gestureTapped;
            GestureRecognizers.Clear();
            GestureRecognizers.Add(gesture);
            CacheType=FFImageLoading.Cache.CacheType.Memory;
        }

        [LocalizationRequired(false)]
        public static readonly BindableProperty CommandProperty =
           BindableProperty.Create(
               "Command",                      // nom de la propriété
               typeof(ICommand),               // type retourné
               typeof(TapImage),               // type déclarant
               null,                           // valeur par défaut
               BindingMode.OneWay,             // binding mode
               null,                           // delegate de validation de la valeur
               (bindable, value, newValue) =>
               {
                   if (bindable == null) return;
                   var l = (TapImage)bindable;
                   l.updateIsEnabled();
                   if (value != null) ((ICommand)value).CanExecuteChanged -= l.canExecuteChanged;
                   if (newValue != null) ((ICommand)newValue).CanExecuteChanged += l.canExecuteChanged;
               }
               );

        [LocalizationRequired(false)]
        public static readonly BindableProperty RelatedViewProperty =
           BindableProperty.Create(
               "RelatedView",                       // nom de la propriété
               typeof(View),                        // type retourné
               typeof(TapImage),                    // type déclarant
               null,                                // valeur par défaut
               BindingMode.OneWay,                  // binding mode
               (bindable, value) => value is View  // delegate de validation de la valeur
               );

        private void canExecuteChanged(object sender, EventArgs e)
        {
            updateIsEnabled();
        }


        private void updateIsEnabled()
        {
            IsEnabled = Command != null && Command.CanExecute(CommandParameter);
        }

        [LocalizationRequired(false)]
        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(
                "CommandParameter",             // nom de la propriété
                typeof(object),                 // type retourné
                typeof(TapImage),               // type déclarant
                null,                           // delegate de validation de la valeur
                BindingMode.OneWay,
                null,
                (bindable, value, newValue) =>
                {
                    if (bindable == null) return;
                    var l = (TapImage)bindable;
                    l.updateIsEnabled();
                }
                );

        [LocalizationRequired(false)]
        public static readonly BindableProperty NumberOfTapsRequiredProperty =
            BindableProperty.Create(
                "NumberOfTapsRequired",
                typeof(int),
                typeof(TapImage),
                1,
                BindingMode.OneWay,
                (bindable, value) =>            // validation de la valeur
                {
                    var tc = (int)value;
                    return tc > 0 && tc < 3;
                },
                (bindable, value, newValue) =>  // property changed
                {
                    if (bindable == null) return;
                    var l = (TapImage)bindable;
                    l.gesture.NumberOfTapsRequired = (int)newValue;
                }
                );

        [LocalizationRequired(false)]
        public static readonly BindableProperty AnimateProperty =
           BindableProperty.Create(
               "Animate",
               typeof(bool),
               typeof(TapImage),
               true);

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public int NumberOfTapsRequired
        {
            get { return (int)GetValue(NumberOfTapsRequiredProperty); }
            set { SetValue(NumberOfTapsRequiredProperty, value); }
        }

        public bool Animate
        {
            get { return (bool)GetValue(AnimateProperty); }
            set { SetValue(AnimateProperty, value); }
        }

        public View RelatedView
        {
            get { return (View)GetValue(RelatedViewProperty); }
            set { SetValue(RelatedViewProperty, value); }
        }

        private void gestureTapped(object sender, EventArgs e)
        {
            if (Animate) PlayAnimation();
            if (Command == null) return;
            if (!Command.CanExecute(CommandParameter)) return;
            Command.Execute(CommandParameter);
        }

        protected void playAnimation(View view)
        {
            if (view == null) return;
            view.ScaleTo(1.5d, 100, Easing.CubicOut);
            view.FadeTo(0, 100).ContinueWith(task =>
            {
                view.ScaleTo(1d, 1, Easing.CubicIn);
                view.FadeTo(1d, 50);
            });
        }

        protected virtual void InternalPlayAnimation()
        {
            playAnimation(this);
            if (RelatedView != null) playAnimation(RelatedView);
        }

        public void PlayAnimation()
        {
            InternalPlayAnimation();
        }
    }
}
