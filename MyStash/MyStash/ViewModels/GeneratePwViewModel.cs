using System;
using System.Collections.Generic;
using System.Linq;
using MyStash.Crouton;
using MyStash.Helpers;
using MyStash.ResX;
using Plugin.Share;
using Xamarin.Forms;

namespace MyStash.ViewModels
{
    public class GeneratePwViewModel : StashViewModelBase
    {

        private int pwLength = 4;
        private bool lettersDown = true;
        private bool lettersUp = true;
        private bool numbers = true;
        private bool symbols;
        private bool leetMode;
        private string originalWord;
        private readonly List<Tuple<LeetMaker.LeetStrength, string>> leetLevels;
        private Tuple<LeetMaker.LeetStrength, string> selectedStrength;
        private string generated = AppResources.GeneratePwViewModel_generated_click_to_generate_;


        public GeneratePwViewModel()
        {
            GenerateCommand = new Command(() =>
                                          {
                                              UserInteraction();
                                              generate();
                                          }, () => LettersDown || LettersUp || Numbers || Symbols);
            CopyToClipboardCommand = new Command(() =>
                                                 {
                                                     UserInteraction();
                                                     CrossShare.Current.SetClipboardText(Generated ?? string.Empty);
                                                     Toasts.Notify(ToastNotificationType.Success, AppResources.GeneratePwViewModel_GeneratePwViewModel_Copy_to_clipboard, AppResources.GeneratePwViewModel_GeneratePwViewModel_Generated_password_copied_to_the_clipboard, TimeSpan.FromSeconds(2));
                                                 },
                                                 () => !string.IsNullOrWhiteSpace(Generated));
            ModalDismissCommand = new Command(()=>Navigation.ModalDismiss());
            leetLevels = (from ls in LeetMaker.LeetStrengthStr
                          select new Tuple<LeetMaker.LeetStrength, string>(ls.Key, ls.Value)).ToList();
            selectedStrength = leetLevels[2];
        }

        public int PwLength
        {
            get { return pwLength; }
            set
            {
                UserInteraction();
                if (value > 3 && value < 21) Set(ref pwLength, value);
            }
        }

        public bool LettersDown
        {
            get { return lettersDown; }
            set
            {
                UserInteraction();
                Set(ref lettersDown, value);
                GenerateCommand.ChangeCanExecute();
            }
        }

        public bool LettersUp
        {
            get { return lettersUp; }
            set
            {
                UserInteraction();
                Set(ref lettersUp, value);
                GenerateCommand.ChangeCanExecute();
            }
        }

        public bool Numbers
        {
            get { return numbers; }
            set
            {
                UserInteraction();
                Set(ref numbers, value);
                GenerateCommand.ChangeCanExecute();
            }
        }

        public bool Symbols
        {
            get { return symbols; }
            set
            {
                UserInteraction();
                Set(ref symbols, value);
                GenerateCommand.ChangeCanExecute();
            }
        }

        public string Generated
        {
            get { return generated; }
            set
            {
                Set(ref generated, value);
                CopyToClipboardCommand.ChangeCanExecute();
            }
        }

        public bool IsLeetMode
        {
            get { return leetMode; }
            // ReSharper disable once ExplicitCallerInfoArgument
            set { if (Set(ref leetMode, value)) RaisePropertyChanged(nameof(IsRandomMode)); }
        }

        public bool IsRandomMode
        {
            get { return !leetMode; }
            // ReSharper disable once ExplicitCallerInfoArgument
            set { if (Set(ref leetMode, !value)) RaisePropertyChanged(nameof(IsLeetMode)); }
        }

        public string OriginalWord
        {
            get { return originalWord; }
            set { Set(ref originalWord, value); }
        }

        public Tuple<LeetMaker.LeetStrength, string> SelectedStrength
        {
            get { return selectedStrength; }
            set { Set(ref selectedStrength, value); }
        }

        public List<Tuple<LeetMaker.LeetStrength, string>> LeetLevels => leetLevels;

        public Command GenerateCommand { get; }

        public Command CopyToClipboardCommand { get; }

        private void generate()
        {
            Generated = IsRandomMode
                ? RandomStringGenerator.GetNewString(PwLength, LettersDown, LettersUp, Numbers, Symbols)
                : LeetMaker.Translate(OriginalWord, selectedStrength.Item1);
        }

        protected override void IncomingCommand(string commandName, object context)
        {
            if (commandName == Utils.GlobalMessages.CopyToClipbard.ToString())
                CrossShare.Current.SetClipboardText(Generated, AppResources.GeneratePwViewModel_IncomingCommand_My_Stash_generated_password);
            Toasts.Notify(ToastNotificationType.Success, AppResources.GeneratePwViewModel_GeneratePwViewModel_Copy_to_clipboard, AppResources.GeneratePwViewModel_GeneratePwViewModel_Generated_password_copied_to_the_clipboard, TimeSpan.FromSeconds(2));
        }

        public Command ModalDismissCommand { get; }
    }
}
