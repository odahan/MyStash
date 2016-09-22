using System;
using System.Windows.Input;
using Xamarin.Forms;
using JetBrains.Annotations;

namespace MyStash.Controls
{
    public class AnimatedTapLabel : Label
    {
        [LocalizationRequired(false)]
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(
                "Command",                      // nom de la propriété
                typeof(ICommand),               // type retourné
                typeof(AnimatedTapLabel),             // type déclarant
                null,                           // valeur par défaut
                BindingMode.OneWay,             // binding mode
                null,                           // delegate de validation de la valeur
                (bindable, value, newValue) =>
                {
                    if (bindable == null) return;
                    var l = (AnimatedTapLabel)bindable;
                    l.updateIsEnabled();
                    if (value != null) ((ICommand)value).CanExecuteChanged -= l.canExecuteChanged;
                    if (newValue != null) ((ICommand)newValue).CanExecuteChanged += l.canExecuteChanged;
                }
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
                typeof(AnimatedTapLabel),        // type déclarant
                null,                           // delegate de validation de la valeur
                BindingMode.OneWay,
                null,
                (bindable, value, newValue) =>
                {
                    if (bindable == null) return;
                    var l = (AnimatedTapLabel)bindable;
                    l.updateIsEnabled();
                }
                );

        [LocalizationRequired(false)]
        public static readonly BindableProperty NumberOfTapsRequiredProperty =
            BindableProperty.Create(
                "NumberOfTapsRequired",
                typeof(int),
                typeof(AnimatedTapLabel),
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
                    var l = (AnimatedTapLabel)bindable;
                    l.tap.NumberOfTapsRequired = (int)newValue;
                }
                );


        [LocalizationRequired(false)]
        public static readonly BindableProperty AnimateProperty =
            BindableProperty.Create(
                "Animate",
                typeof(bool),
                typeof(AnimatedTapLabel),
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

        private readonly TapGestureRecognizer tap = new TapGestureRecognizer();

        public AnimatedTapLabel()
        {
            tap.Tapped += tapTapped;
            GestureRecognizers.Clear();
            GestureRecognizers.Add(tap);
        }

        private void tapTapped(object sender, EventArgs e)
        {
            if (Command == null) return;
            if (!Command.CanExecute(CommandParameter)) return;
            Command.Execute(CommandParameter);
            if (Animate) PlayAnimation();
        }

        public void PlayAnimation()
        {
            this.ScaleTo(2d, 100, Easing.CubicOut);
            this.FadeTo(0, 100).ContinueWith(task =>
            {
                this.ScaleTo(1d, 1, Easing.CubicIn);
                this.FadeTo(1d, 10);
            });
            // await this.RotateTo(360d, 250, Easing.CubicOut)
            //     .ContinueWith(
            //     t => Device.BeginInvokeOnMainThread(() => Rotation = 0d));
        }
    }
}
