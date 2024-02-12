using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Qbo
{
    public partial class App : Application
    {
        public static string apiKeyTMDB = "533d07bb864e98083907f0170fe33612";
        public static string tokenTMDB = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI1MzNkMDdiYjg2NGU5ODA4MzkwN2YwMTcwZmUzMzYxMiIsInN1YiI6IjY1YzUxNDkwMmJjZjY3MDE2MjkwY2U5NiIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.wE4Jm_OF3P48ujMT9naplbX0Fo2U5QLPtkABd7g_Gco";

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
