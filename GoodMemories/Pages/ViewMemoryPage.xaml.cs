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
    public partial class ViewMemoryPage : ContentPage
    {
        // Memory displayed on the page
        MemoryModel pageMemory; 
        public ViewMemoryPage()
        {
            InitializeComponent();
        }

        public ViewMemoryPage(string memID)
        {
            InitializeComponent(); 

            // Find memory that matches ID 
            pageMemory = App.dbAccess.findMemory(memID);

            // Fill in memory label with the name of the memory, if it has one
            if (!String.IsNullOrEmpty(pageMemory.memoryName))
            {
                string lblText = pageMemory.memoryName; 

                if(lblText.Length > 16)
                {
                    lblText = lblText.Substring(0, 16) + "..."; 
                }
                memoryLabel.Text = lblText; 
            }

            // Fill in date and description fields similarly
            if (!String.IsNullOrEmpty(pageMemory.memoryDate))
            {
                string dateText = pageMemory.memoryDate; 

                if(dateText.Length > 35)
                {
                    dateText = dateText.Substring(0, 35) + "..."; 
                }
                dateField.Text = dateText; 
            }
            else
            {
                dateField.Text = "No date."; 
            }

            if (!String.IsNullOrEmpty(pageMemory.memoryText))
            {
                string descripText = pageMemory.memoryText; 

                if(descripText.Length > 250)
                {
                    descripText = descripText.Substring(0, 250) + "..."; 
                }
                descriptionField.Text = descripText; 
            }
            else
            {
                descriptionField.Text = "No description."; 
            }
        }

        
        private async void EditMemoryButton_Clicked(object sender, EventArgs e)
        {
            // Navigate to new edit memory page with the current memory's ID
            await Navigation.PushAsync(new EditMemoryPage(pageMemory.ID.ToString())); 
        }
    }
}