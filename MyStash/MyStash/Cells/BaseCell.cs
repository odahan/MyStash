using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Xamarin.Forms;
using MyStash.Helpers;

namespace MyStash.Cells
{
    public class BaseCell : ViewCell
    {

        protected bool DrawLines { get; set; }
        protected bool DisplayDate { get; set; }

        public BaseCell()
        {
            DisplayDate=App.Locator.AppSettings.DisplayDateInLists;
            DrawLines = App.Locator.AppSettings.DisplayLinesInLists;
            Messenger.Default.Register<NotificationMessage>(this, n =>
            {
                if (n.Notification != Utils.GlobalMessages.SettingsChanged.ToString())
                return;
                DrawLines = App.Locator.AppSettings.DisplayLinesInLists;
                DisplayDate = App.Locator.AppSettings.DisplayDateInLists;
                OnSettingsChanged();
            });
        }

        protected virtual void OnSettingsChanged(){}
    }
}
