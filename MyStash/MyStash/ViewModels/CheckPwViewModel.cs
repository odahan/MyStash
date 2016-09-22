using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using MyStash.Helpers;
using MyStash.Service;
using Xamarin.Forms;

namespace MyStash.ViewModels
{
    class CheckPwViewModel : StashViewModelBase
    {
        protected override void IncomingParameter(object parameter)
        {
            if (parameter == null) parameter = string.Empty;
            Password = parameter as string;
        }

        public CheckPwViewModel()
        {
            Dismiss = new Command(() =>
                                  {
                                      UserInteraction();
                                      Navigation.ModalDismiss();
                                  });
            ForceAnalysis = new Command(
                () =>
                {
                    UserInteraction();
                    analyze();
                });
        }

        private string password;
        private int score;
        private int bonus;
        private int malus;
        private string strength;

        private List<AnalysisItem> items;

        public Command Dismiss { get; private set; }
        public Command ForceAnalysis { get; private set; }

        private void analyze()
        {
            PasswordChecker.Instance.Password = Password;
            Score = PasswordChecker.Instance.PasswordScore;
            Bonus = PasswordChecker.Instance.PasswordBonus;
            Malus = PasswordChecker.Instance.PasswordMalus;
            Strength = PasswordChecker.Instance.PasswordStrength;
            var i = new List<AnalysisItem>();
            i.AddRange(PasswordChecker.Instance.StrengthDetails);
            if (i.Count>0) i.RemoveAt(0);
            Items = i.OrderByDescending(x => // Malus first, then bonus, then null
                                        {
                                            if (x.IsMalus) return 10000 - x.Total;
                                            if (x.IsBonus) return 5000 + x.Total;
                                            return 0;
                                        }  ).ToList();
        }

        public string Password
        {
            get { return password; }
            set
            {
                UserInteraction();
                if (Set(ref password, value)) analyze();
            }
        }

        public int Score
        {
            get { return score; }
            set { Set(ref score, value); }
        }

        public int Bonus
        {
            get { return bonus; }
            set { Set(ref bonus, value); }
        }

        public int Malus
        {
            get { return malus; }
            set { Set(ref malus, value); }
        }

        public string Strength
        {
            get { return strength; }
            set { Set(ref strength, value); }
        }

        public List<AnalysisItem> Items
        {
            get { return items; }
            private set { Set(ref items, value); }
        }

        public bool IsLineVisible => Settings.DisplayLinesInLists;
    }
}
