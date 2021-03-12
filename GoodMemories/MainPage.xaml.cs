using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GoodMemories
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false); 
        }

        protected async override void OnAppearing()
        {
            await title.FadeTo(1, 1500, Easing.CubicInOut);
            await Task.Delay(1000);

            await Navigation.PushAsync(new Pages.HomePage()); 
            
        }
    }
}
