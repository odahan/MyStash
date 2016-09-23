using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStash.ViewModels;
using Xamarin.Forms;

namespace MyStash.Service
{
    public interface INavigationAware
    {
        // can replace Loaded but will be called each time the VM is navigated to.
        void OnNavigatedTo();
        // the VM will become inactive. Raised just before the new VM is activated.
        void OnNavigatedFrom();
    }
}
