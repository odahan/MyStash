using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;
using MyStash.ResX;
using Xamarin.Forms;

namespace MyStash.Service
{
    public class DialogService : IDialogService
    {
        private Page dialogPage;

        public void Initialize(Page page)
        {
            dialogPage = page;
        }

        public async Task ShowError(string message,
            string title,
            string buttonText,
            Action afterHideCallback)
        {
            await dialogPage.DisplayAlert(
                title,
                message,
                buttonText);

            afterHideCallback?.Invoke();
        }

        public async Task ShowError(
            Exception error,
            string title,
            string buttonText,
            Action afterHideCallback)
        {
            await dialogPage.DisplayAlert(
                title,
                error.Message,
                buttonText);

            afterHideCallback?.Invoke();
        }

        public async Task ShowMessage(
            string message,
            string title)
        {
            await dialogPage.DisplayAlert(
                title,
                message,
                AppResources.DialogService_ShowMessage_Ok);
        }

        public async Task ShowMessage(
            string message,
            string title,
            string buttonText,
            Action afterHideCallback)
        {
            await dialogPage.DisplayAlert(
                title,
                message,
                buttonText);

            afterHideCallback?.Invoke();
        }

        public async Task<bool> ShowMessage(
            string message,
            string title,
            string buttonConfirmText,
            string buttonCancelText,
            Action<bool> afterHideCallback)
        {
            var result = await dialogPage.DisplayAlert(
                title,
                message,
                buttonConfirmText,
                buttonCancelText);

            afterHideCallback?.Invoke(result);

            return result;
        }

        public async Task ShowMessageBox(
            string message,
            string title)
        {
            await dialogPage.DisplayAlert(
                title,
                message,
                AppResources.DialogService_ShowMessage_Ok);
        }
    }
}
