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
    public partial class ViewAllPage : ContentPage
    {
        // List of all memory entries to display 
        List<MemoryModel> allMems; 
        public ViewAllPage()
        {
            InitializeComponent();
            
            // Fetch all memory entries from database
            allMems = App.dbAccess.getAllMemories(); 

            // For each memory, append a new frame to the page with appropriate information 
            foreach(var mem in allMems)
            {
                // Determine how to identify the memory; default to name, then description if there is no name
                string memoryLabel;

                if (String.IsNullOrEmpty(mem.memoryName))
                {
                    memoryLabel = mem.memoryText; 
                }
                else
                {
                    memoryLabel = mem.memoryName; 
                }

                if(memoryLabel.Length > 16)
                {
                    memoryLabel = memoryLabel.Substring(0, 16) + "...";
                }

                // Create and style new frame 
                Frame newFrame = new Frame
                {
                    Content = new Label
                    {
                        Text = memoryLabel,
                        FontFamily = "Yanone",
                        FontSize = 24,
                        TextColor = Color.FromHex("06324B")
                    },
                    CornerRadius=20,
                    WidthRequest=280,
                    HorizontalOptions=LayoutOptions.CenterAndExpand,
                    ClassId=mem.ID.ToString()
                };

                // Add a tap event listener to the frame
                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.Tapped += openViewMemoryPage; 
                newFrame.GestureRecognizers.Add(tap); 
                
                // Append to StackLayout
                memContainer.Children.Add(newFrame);
            }
        }


        private async void openViewMemoryPage(object sender, EventArgs e)
        {
            // Get the ID corresponding to the memory that triggered the event
            string memID = ((Frame)sender).ClassId;

            // Open the view memory page corresponding to that ID
            await Navigation.PushAsync(new ViewMemoryPage(memID)); 
        }
    }
}