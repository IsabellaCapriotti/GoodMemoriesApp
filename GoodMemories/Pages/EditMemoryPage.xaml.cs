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
    public partial class EditMemoryPage : ContentPage
    {

        MemoryModel pageMemory; 
        public EditMemoryPage()
        {
            InitializeComponent();
        }

        public EditMemoryPage(string memID)
        {
            InitializeComponent();

            // Find memory that matches ID 
            pageMemory = App.dbAccess.findMemory(memID);
            Console.WriteLine(pageMemory.memoryText);

            // Fill in relevant information on page
            if (!String.IsNullOrEmpty(pageMemory.memoryName))
            {
                nameEntry.Text = pageMemory.memoryName; 
            }

            if (!String.IsNullOrEmpty(pageMemory.memoryDate))
            {
                dateEntry.Text = pageMemory.memoryDate; 
            }

            if (!String.IsNullOrEmpty(pageMemory.memoryText))
            {
                descriptionEntry.Text = pageMemory.memoryText;
            }
        }

        private async void AddMemoryButton_Clicked(object sender, EventArgs e)
        {
            // Check that description has not been left empty
            if (String.IsNullOrEmpty(descriptionEntry.Text))
            {
                await DisplayAlert("Oops!", "Please enter a description of your memory.", "OK");
                return;
            }

            // Update in database
            pageMemory.memoryText = descriptionEntry.Text;
            pageMemory.memoryDate = dateEntry.Text;
            pageMemory.memoryName = nameEntry.Text; 

            App.dbAccess.editMemory(pageMemory);

            // Send back to home page 

            // Get list of all pages currently in the navigation stack 
            List<Page> allPages = Navigation.NavigationStack.ToList<Page>(); 
            foreach(Page currPage in allPages)
            {
                // Delete every page, but break before the root 
                if(Navigation.NavigationStack.Count == 1)
                {
                    break; 
                }

                Navigation.RemovePage(currPage); 
            }

            // Navigate back to home page 
            await Navigation.PushAsync(new HomePage()); 
        }
    }
}