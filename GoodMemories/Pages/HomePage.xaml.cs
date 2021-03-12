using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using GoodMemories.Models; 

namespace GoodMemories.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        List<MemoryModel> allCurrentMems; 
        public HomePage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);


    
        }

        protected override void OnAppearing()
        {

            // Display new memory if 24 hours have passed since last new memory was displayed
            if (App.timeStampManager.dayPassedSinceLastMemory())
            {

                // Get all current memories from the database and sort them in order
                // of newest date to oldest date
                allCurrentMems = App.dbAccess.getAllMemories();

                // Break if there are no memories to display; ie, user hasn't added any
                if(allCurrentMems.Count == 0)
                {
                    return; 
                }

                sortMemories(allCurrentMems, 0, allCurrentMems.Count);

                Console.WriteLine("Current memories: ");
                foreach (MemoryModel mem in allCurrentMems)
                {
                    Console.WriteLine(mem.memoryText);
                }

                // Pick a random memory from the list
                MemoryModel pickedMem = pickMemory(allCurrentMems);
                Console.WriteLine("Picked " + pickedMem.memoryText);

                // Update the memory's timestamp
                App.timeStampManager.updateMemoryTimestamp(pickedMem);

                // Display new memory on the page
                // Title of memory 
                if (!String.IsNullOrEmpty(pickedMem.memoryName))
                {
                    string headerText = pickedMem.memoryName; 

                    if(headerText.Length > 20)
                    {
                        headerText = headerText.Substring(0, 20)  + "..."; 
                    }

                    todaysMemoryHeader.Text = headerText;
                    todaysMemoryHeader.IsVisible = true;

                }

                // Memory description 
                string bodyText = pickedMem.memoryText; 
                if(bodyText.Length > 80)
                {
                    bodyText = bodyText.Substring(0, 80) + "..."; 
                }

                todaysMemoryBody.Text = bodyText;
                todaysMemoryBody.IsVisible = true;

                // Update view memory button to match current memory 
                viewTodaysMemoryButton.ClassId = pickedMem.ID.ToString();
                viewTodaysMemoryButton.IsVisible = true; 
            }

            // If 24 hours have not passed, display the last memory that was used
            else
            {
                int lastMemID = Preferences.Get("lastMemoryID", -1); 
                if(lastMemID != -1)
                {
                    // Find matching memory 
                    MemoryModel pickedMem = App.dbAccess.findMemory(lastMemID.ToString());

                    // Display new memory on the page
                    // Title of memory 
                    if (!String.IsNullOrEmpty(pickedMem.memoryName))
                    {
                        string headerText = pickedMem.memoryName;

                        if (headerText.Length > 16)
                        {
                            headerText = headerText.Substring(0, 16) + "...";
                        }

                        todaysMemoryHeader.Text = headerText;
                        todaysMemoryHeader.IsVisible = true;

                    }

                    // Memory description 
                    string bodyText = pickedMem.memoryText;
                    if (bodyText.Length > 200)
                    {
                        bodyText = bodyText.Substring(0, 200) + "...";
                    }

                    todaysMemoryBody.Text = bodyText;
                    todaysMemoryBody.IsVisible = true;


                    // Update view memory button to match current memory 
                    viewTodaysMemoryButton.ClassId = pickedMem.ID.ToString();
                    viewTodaysMemoryButton.IsVisible = true;
                }
            }
        }

        /* Given a list of all the current memories in the database, this function will
        sort the list inplace with the newest memories at the beginning of the list and
        the oldest memories at the end of the list. 

        Note: "Newest" would mean the memory that has been most recently served to the user. 
        Unless the memory has never been used, its last use date will override its date of
        creation in determining its newness. 
        */ 
        private void sortMemories(List<MemoryModel> allMems, int left, int right)
        {
            
            // Base case
            if(Math.Abs(left-right) <= 1)
            {
                return; 
            }


            // Sort each half of the list 
            int med = (left + right) / 2;

            sortMemories(allMems, 0, med);
            sortMemories(allMems, med, right);

            // Merge two sorted halves 
            List<MemoryModel> leftList = new List<MemoryModel>();
            List<MemoryModel> rightList = new List<MemoryModel>(); 

            for(int i=0; i < med; i++)
            {
                leftList.Add(allMems[i]); 
            }
            for(int i=med; i < allMems.Count; i++)
            {
                rightList.Add(allMems[i]); 
            }

            int leftListIdx = 0;
            int rightListIdx = 0;
            int mainIdx = 0; 

            while(leftListIdx < leftList.Count || rightListIdx < rightList.Count)
            {
                // If either list has already been depleted, then add an element from the other list
                // by default 
                if(leftListIdx >= leftList.Count)
                {
                    allMems[mainIdx] = rightList[rightListIdx];
                    ++mainIdx;
                    ++rightListIdx;
                }
                else if (rightListIdx >= rightList.Count)
                {
                    allMems[mainIdx] = leftList[leftListIdx];
                    ++mainIdx;
                    ++leftListIdx;
                }

                // If both lists still have elements remaining, compare the times represented by each
                // and insert the newer one before the older one
                else
                {
                    DateTime currTime = DateTime.Now;
                    TimeSpan tLeft = App.timeStampManager.timePassedSinceMemoryUsed(leftList[leftListIdx]);
                    TimeSpan tRight = App.timeStampManager.timePassedSinceMemoryUsed(rightList[rightListIdx]); 

                    if(tLeft <= tRight)
                    {
                        allMems[mainIdx] = leftList[leftListIdx];
                        ++mainIdx;
                        ++leftListIdx; 
                    }
                    else
                    {
                        allMems[mainIdx] = rightList[rightListIdx];
                        ++mainIdx;
                        ++rightListIdx; 
                    }

                }

            }

        }

        /* 
        Returns a weighted random memory from the passed list of all memories. Memories that are older (have
        not been used in a long time) are weighted more heavily than memoreis that are newer. s
        */ 
        private MemoryModel pickMemory(List<MemoryModel> allMems)
        {
            int currWeight = allMems.Count;
            List<MemoryModel> weightedList = new List<MemoryModel>(); 

            // Add each memory from the original list into the weighted list, starting with 
            // the heaviest weight for the oldest memories at the end of the list
            for (int i=allMems.Count - 1; i >= 0; i--)
            {
                for(int j=0; j < currWeight; j++)
                {
                    weightedList.Add(allMems[i]); 
                }

                // Decrease weight unless it is already at the lowest possible weight
                if (currWeight > 1)
                {
                    currWeight /= 2;
                }
            }

            // Pick a random entry from the weighted list
            Random rand = new Random();
            int randIdx = rand.Next(0, weightedList.Count);

            return weightedList[randIdx]; 
        }
      
        // Navigate to page to add a new memory 

        private async void AddMemoryButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddMemoryPage()); 
        }
        
        // Navigate to page to view all memories

        private async void ViewAllButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ViewAllPage()); 
        }

        // Opens a view memory page for the memory with the passed ID. 
        private async void OpenCurrentMemory(object sender, EventArgs e)
        {
            Button sentBtn = (Button)sender;
            await Navigation.PushAsync(new ViewMemoryPage(sentBtn.ClassId)); 
        }
    }
}