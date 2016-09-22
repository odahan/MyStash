using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using MyStash.ResX;
using Xamarin.Forms;

namespace MyStash.Service
{
    public class NavigationService : INavigationService2
    {
        private readonly Dictionary<string, Type> pages = new Dictionary<string, Type>();
        private INavigation navigation;
        private object currentPage;

        public string CurrentPageKey
        {
            get
            {
                lock (pages)
                {
                    if (currentPage == null) return null;
                    var pageType = currentPage.GetType();
                    return pages.ContainsValue(pageType)
                        ? pages.FirstOrDefault(p => p.Value == pageType).Key
                        : null;
                }
            }
        }

        public Task GoBack()
        {
            return navigation?.PopAsync();
        }


        public Task NavigateTo(string pageKey)
        {
            return NavigateTo(pageKey, null);
        }

        public Task NavigateTo(string pageKey, object parameter)
        {
            return NavigateTo(pageKey, parameter, true);
        }

        public Task NavigateTo(string pageKey, object parameter, bool animate)
        {
            var page = getPage(pageKey, parameter);
            if (page == null || navigation == null) return Task.FromResult(false);
            return navigation.PushAsync(page);
        }


        private Page getPage(string pageKey, object parameter)
        {
            lock (pages)
            {
                if (!pages.ContainsKey(pageKey))
                {
                    throw new ArgumentException(
                        string.Format(AppResources.NavigationService_getPage_Page_inconnueConfigure, pageKey), nameof(pageKey));
                }
                var type = pages[pageKey];
                ConstructorInfo constructor;
                object[] parameters;

                if (parameter == null)
                {
                    constructor = type.GetTypeInfo()
                        .DeclaredConstructors
                        .FirstOrDefault(c => !c.GetParameters().Any());

                    parameters = new object[] { };
                }
                else
                {
                    constructor = type.GetTypeInfo()
                        .DeclaredConstructors
                        .FirstOrDefault(
                            c =>
                            {
                                var p = c.GetParameters();
                                return p.Length == 1
                                       && p[0].ParameterType == parameter.GetType();
                            });

                    parameters = new[] { parameter };
                }

                if (constructor == null)
                    // ReSharper disable once HeuristicUnreachableCode
                {
                    throw new InvalidOperationException(
                        string.Format(AppResources.NavigationService_getPage_Aucun_constructeur_approprié_trouvé, pageKey));
                }
                try
                {
                    var page = constructor.Invoke(parameters) as Page;
                    return page;
                }
                catch (Exception ex)
                {
                    ServiceLocator.Current.GetInstance<IDialogService>().ShowMessage(ex.Message, ex.InnerException?.Message);
                    return null;
                }
                
            }
        }

        public Task ModalNavigateTo(string pageKey)
        {
            return ModalNavigateTo(pageKey, null);
        }

        public Task ModalNavigateTo(string pageKey, object parameter, bool animate = true)
        {
            var page = getPage(pageKey, parameter);
            return page != null ? navigation?.PushModalAsync(page, animate) : Task.FromResult<Page>(null);
        }

        public void Configure(string pageKey, Type pageType)
        {
            lock (pages)
            {
                if (pages.ContainsKey(pageKey)) pages[pageKey] = pageType;
                else
                    pages.Add(pageKey, pageType);
            }
        }

        public Task ModalDismiss()
        {
            return navigation?.PopModalAsync();
        }


        public void Initialize(Page navigationPage)
        {
            navigation = navigationPage.Navigation;
            currentPage = navigationPage;
        }
    }
}
