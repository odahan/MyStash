using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyStash.Service
{
    public interface INavigationService2
    {
        Task ModalNavigateTo(string pageKey);
        Task ModalNavigateTo(string pageKey, object parameter,bool animate = true);
        Task ModalDismiss();
        Task NavigateTo(string pageKey, object parameter, bool animate);
        void Initialize(Page navigationPage);
        string CurrentPageKey { get; }
        Task GoBack();
        Task NavigateTo(string pageKey);
        Task NavigateTo(string pageKey, object parameter);
    }
}
