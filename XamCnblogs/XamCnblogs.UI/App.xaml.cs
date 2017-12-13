﻿using FormsToolkit;
using Newtonsoft.Json;
using System;

using Xamarin.Forms;
using XamCnblogs.Portable.Helpers;
using XamCnblogs.Portable.Model;
using XamCnblogs.Portable.ViewModel;
using XamCnblogs.UI.Pages.Account;

namespace XamCnblogs.UI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            ViewModelBase.Init();

            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    MainPage = new Pages.Android.RootPage();
                    break;
                case Device.iOS:
                    break;
            }
        }
        protected override void OnStart()
        {
            OnResume();
        }

        public void SecondOnResume()
        {
            OnResume();
        }
        bool registered;
        protected override void OnResume()
        {
            if (registered)
                return;
            registered = true;

            MessagingService.Current.Subscribe(MessageKeys.NavigateLogin, async m =>
            {
                Page page = new NavigationPage(new AuthorizePage());

                var nav = Application.Current?.MainPage?.Navigation;
                if (nav == null)
                    return;

                await NavigationService.PushModalAsync(nav, page);
            });
            MessagingService.Current.Subscribe<string>(MessageKeys.NavigateToken, async (m, q) =>
            {
                var result = await TokenHttpClient.Current.PostTokenAsync(q);
                if (result.Success)
                {
                    var token = JsonConvert.DeserializeObject<Token>(result.Message.ToString());
                    token.RefreshTime = DateTime.Now;
                    UserTokenSettings.Current.UpdateUserToken(token);

                    var userResult = await UserHttpClient.Current.GetAsyn(Apis.Users);
                    if (userResult.Success)
                    {
                        var user = JsonConvert.DeserializeObject<User>(userResult.Message.ToString());

                        UserSettings.Current.UpdateUser(user);

                        var nav = Application.Current?.MainPage?.Navigation;
                        if (nav == null)
                            return;
                        await nav.PopModalAsync();
                    }
                }
            });
        }
        protected override void OnSleep()
        {
            if (!registered)
                return;

            registered = false;
            MessagingService.Current.Unsubscribe(MessageKeys.NavigateLogin);
            MessagingService.Current.Unsubscribe(MessageKeys.NavigateToken);
        }

    }
}