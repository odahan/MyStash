using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStash.Service;
using Xamarin.Forms;

namespace MyStash.Controls
{
    public class StashContentPage : ContentPage
    {
        protected override void OnAppearing()
        {
            (BindingContext as INavigationAware)?.OnNavigatedTo();
        }

        protected override void OnDisappearing()
        {
            (BindingContext as INavigationAware)?.OnNavigatedFrom();
        }
    }

    public class StashTabbedPage : TabbedPage
    {
        protected override void OnAppearing()
        {
            (BindingContext as INavigationAware)?.OnNavigatedTo();
        }

        protected override void OnDisappearing()
        {
            (BindingContext as INavigationAware)?.OnNavigatedFrom();
        }
    }
}
