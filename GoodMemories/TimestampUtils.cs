using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

using GoodMemories.Models; 

namespace GoodMemories
{
    public class TimestampUtils
    {
        private string lastUsedTimestamp = "lastUsedTimestamp";
        private string currentTimestamp = "currentTimestamp";
        private string lastMemoryTimestamp = "lastMemoryTimestamp";
        private string lastMemoryID = "lastMemoryID"; 

        // Updates timestamps when the app is opened. 
        public void updateTimeStamps()
        {
            // Update timestamp keeping track of when application was last opened
            // Set the current timestamp, and then move the previous current timestamp
            // to the "lastUsedTimestamp" field
            long oldTimestamp = Preferences.Get(currentTimestamp, DateTime.Now.Ticks);
            Preferences.Set(currentTimestamp, DateTime.Now.Ticks);
            Preferences.Set(lastUsedTimestamp, oldTimestamp);

            //string currStampString = Preferences.Get(currentTimestamp, DateTime.Now.Ticks).ToString();
            //string oldStampString = Preferences.Get(lastUsedTimestamp, DateTime.Now.Ticks).ToString();

        }

        // Determines whether or not it has been 24 hours since the last memory has been displayed. 
        // Will also return true if a new memory has never been displayed. 
        public bool dayPassedSinceLastMemory()
        {
            // Check to see if it has been 24 hours since a new memory has been displayed 
            long ticksSinceLastMemory = Preferences.Get(lastMemoryTimestamp, (long)-1);
            
            // If the last memory timestamp has been set, check to see if 24 hours have passed
            // between now and the time it was set to 
            if (ticksSinceLastMemory != -1)
            {
                DateTime lastMemoryDate = new DateTime(ticksSinceLastMemory);
                TimeSpan timePassed = DateTime.Now - lastMemoryDate;

                if (timePassed.TotalHours >= 24)
                {
                    Console.WriteLine("display new memory");
                    return true;
                }
            }

            // If the last memory timestamp has never been set, then assume a new memory has never been
            // set and go straight to setting the first one 
            else
            {
                Console.WriteLine("display first memory");
                return true; 
            }

            return false; 
        }

        /* Returns a TimeSpan representing how much time has passed since this memory has been 
        served to the user. If the memory has been used before, this will be the difference between
        now and the memory's lastUsedTimestamp field. If it has never been used, this will be the difference
        between now and the memory's createdTimestamp field. 
        */ 
        public TimeSpan timePassedSinceMemoryUsed(MemoryModel mem)
        {

            DateTime memLastUsed; 

            if(mem.lastUsedTimestamp != -1)
            {
                memLastUsed = new DateTime(mem.lastUsedTimestamp); 
            }
            else
            {
                memLastUsed = new DateTime(mem.createdTimeStamp); 
            }

            TimeSpan timePassed = DateTime.Now - memLastUsed;

            return timePassed; 

        }

        // Prints information to the console on the current states of all the timestamps. 
        public void timeReport()
        {
            long lastUsedTS = Preferences.Get(lastUsedTimestamp, (long)-1);
            long currTS = Preferences.Get(currentTimestamp, (long)-1);
            long lastMemTS = Preferences.Get(lastMemoryTimestamp, (long)-1);

            DateTime lastUsedDate;
            DateTime currentDate;
            DateTime lastMemDate; 

            if(lastUsedTS == -1)
            {
                Console.WriteLine("No last used timestamp.");
            }
            else
            {
                Console.WriteLine("Last used timestamp: ");
                lastUsedDate = new DateTime(lastUsedTS);
                Console.WriteLine(lastUsedDate.ToString());
            }

            if(currTS == -1)
            {
                Console.WriteLine("No current timestamp."); 
            }
            else
            {
                Console.WriteLine("Current timestamp: ");
                currentDate = new DateTime(currTS);
                Console.WriteLine(currentDate.ToString()); 
            }

            if(lastMemTS == -1)
            {
                Console.WriteLine("No last memory timestamp."); 
            }
            else
            {
                Console.WriteLine("Last memory timestamp: ");
                lastMemDate = new DateTime(lastMemTS);
                Console.WriteLine(lastMemDate.ToString()); 
            }
        }


        // Updates the timestamp of the passed memory to the current time- for use when a new memory is served
        // to the user. Also updates the memory's timestmap in the database. 
        public void updateMemoryTimestamp(MemoryModel memToUpdate)
        {
            long currTicks = DateTime.Now.Ticks;
            memToUpdate.lastUsedTimestamp = currTicks;
            
            Preferences.Set(lastMemoryTimestamp, currTicks);
            Preferences.Set(lastMemoryID, memToUpdate.ID); 

            App.dbAccess.editMemory(memToUpdate); 
        }
    
        
    }   
}
