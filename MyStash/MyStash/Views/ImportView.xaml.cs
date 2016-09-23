using MyStash.Controls;
using MyStash.ViewModels;
using Xamarin.Forms;

namespace MyStash.Views
{
    public partial class ImportView : StashContentPage
    {
        public ImportView(ImportViewDataType dataType)
        {
            InitializeComponent();
            Padding = new Thickness(8, Device.OnPlatform(20, 0, 0), 8, 8);
            BindingContext = App.Locator.ImportDataVM;
            (BindingContext as IParameter)?.SetParameter(dataType);
        }
    }
}
