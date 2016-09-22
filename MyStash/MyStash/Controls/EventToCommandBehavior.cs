using System;
using System.Reflection;
using System.Windows.Input;
using JetBrains.Annotations;
using MyStash.ResX;
using Xamarin.Forms;

namespace MyStash.Controls
{
    public class EventToCommandBehavior : BehaviorBase<View>
    {
        Delegate eventHandler;

        [LocalizationRequired(false)]
        public static readonly BindableProperty EventNameProperty = BindableProperty.Create("EventName", typeof(string), typeof(EventToCommandBehavior), null, propertyChanged: onEventNameChanged);
        [LocalizationRequired(false)]
        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(EventToCommandBehavior), null);
        [LocalizationRequired(false)]
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(EventToCommandBehavior), null);
        [LocalizationRequired(false)]
        public static readonly BindableProperty InputConverterProperty = BindableProperty.Create("Converter", typeof(IValueConverter), typeof(EventToCommandBehavior), null);

        public string EventName
        {
            get { return (string)GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }
        }

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

        public IValueConverter Converter
        {
            get { return (IValueConverter)GetValue(InputConverterProperty); }
            set { SetValue(InputConverterProperty, value); }
        }

        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);
            registerEvent(EventName);
        }

        protected override void OnDetachingFrom(View bindable)
        {
            deregisterEvent(EventName);
            base.OnDetachingFrom(bindable);
        }

        
        private void registerEvent(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            var eventInfo = AssociatedObject.GetType().GetRuntimeEvent(name);
            if (eventInfo == null)
            {
                throw new ArgumentException(string.Format(AppResources.EventToCommandBehavior_CantRegisterEvent, EventName));
            }

            var methodInfo = typeof(EventToCommandBehavior).GetTypeInfo().GetDeclaredMethod( nameof(onEvent));
            eventHandler = methodInfo.CreateDelegate(eventInfo.EventHandlerType, this);
            eventInfo.AddEventHandler(AssociatedObject, eventHandler);
        }

        private void deregisterEvent(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            if (eventHandler == null)
            {
                return;
            }
            var eventInfo = AssociatedObject.GetType().GetRuntimeEvent(name);
            if (eventInfo == null)
            {
                throw new ArgumentException(string.Format(AppResources.EventToCommandBehavior_CantDeRegisterEvent, EventName));
            }
            eventInfo.RemoveEventHandler(AssociatedObject, eventHandler);
            eventHandler = null;
        }

        private void onEvent(object sender, object eventArgs)
        {
            if (Command == null)
            {
                return;
            }

            object resolvedParameter;
            if (CommandParameter != null)
            {
                resolvedParameter = CommandParameter;
            }
            else if (Converter != null)
            {
                resolvedParameter = Converter.Convert(eventArgs, typeof(object), null, null);
            }
            else
            {
                resolvedParameter = eventArgs;
            }

            if (Command.CanExecute(resolvedParameter))
            {
                Command.Execute(resolvedParameter);
            }
        }

        private static void onEventNameChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var behavior = (EventToCommandBehavior)bindable;
            if (behavior.AssociatedObject == null)
            {
                return;
            }

            var oldEventName = (string)oldValue;
            var newEventName = (string)newValue;

            behavior.deregisterEvent(oldEventName);
            behavior.registerEvent(newEventName);
        }
    }

}
