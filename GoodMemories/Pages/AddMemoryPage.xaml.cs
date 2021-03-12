using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GoodMemories.Models; 

namespace GoodMemories.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddMemoryPage : ContentPage
    {
        public AddMemoryPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, true); 
        }

        private async void AddMemoryButton_Clicked(object sender, EventArgs e)
        {
            // If no description was entered, display an error
            if (String.IsNullOrEmpty(descriptionEntry.Text))
            {
                await DisplayAlert("Oops!", "Please enter a description of your memory.", "OK");
                return; 
            }

            // Build a new memory database entry based on entered information
            MemoryModel newMemory = new MemoryModel()
            {
                memoryDate=dateEntry.Text,
                memoryName=nameEntry.Text,
                memoryText=descriptionEntry.Text,
                createdTimeStamp= DateTime.Now.Ticks,
                lastUsedTimestamp = -1
            };

            // Add memory to database
            App.dbAccess.addMemory(newMemory);

            // Redirect back to main page
            await Navigation.PopAsync(); 
        }
    }
}