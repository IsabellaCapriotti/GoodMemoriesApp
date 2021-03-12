using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials; 

namespace GoodMemories
{
    public partial class App : Application
    {
        public static DatabaseManager dbAccess;
        public static TimestampUtils timeStampManager;

        public App()
        {
            InitializeComponent();

            // Instantiate database management class
            dbAccess = new DatabaseManager("goodMemories.db3");
            dbAccess.createDatabase();

            // Instantiate timestamp management class, update timestamps
            timeStampManager = new TimestampUtils();
            timeStampManager.updateTimeStamps();
            timeStampManager.timeReport(); 


            // Start navigation stack with MainPage as the root
            MainPage = new NavigationPage(new MainPage()) {
                BarBackgroundColor = Color.FromHex("#468189"),
                BarTextColor = Color.White
            };

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
