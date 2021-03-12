using System;
using System.IO; 
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using SQLite;
using Xamarin.Essentials; 

using GoodMemories.Models; 

namespace GoodMemories
{
    public class DatabaseManager
    {
        private string dbPath;
        private SQLiteConnection conn; 

        // Constructor, builds default path for database
        public DatabaseManager(string pathName)
        {
            dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), pathName); 
        }

        // Establishes database connection
        public bool createDatabase(bool resetFlag=false)
        {
            try
            {
                conn = new SQLiteConnection(dbPath);

                if (resetFlag)
                {
                    Preferences.Clear(); 
                    resetDatabase();
                }
                conn.CreateTable<MemoryModel>();
                
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open database connection.\nError message: {ex.Message}");
                return false; 
            }

            return true; 
        }

        // Clears the database
        private bool resetDatabase()
        {
            try
            {
                conn.DropTable<MemoryModel>();
            }
            
            catch(Exception ex)
            {
                Console.WriteLine($"Failed to reset databse.\nError message: {ex.Message}");
                return false; 
            }

            return true; 
        }

        // Inserts the passed memory entry into the database
        public bool addMemory(MemoryModel newMemory)
        {
            try
            {
                conn.Insert(newMemory);
                Console.WriteLine("Successfully added memory!"); 
            }
            
            catch(Exception ex)
            {
                Console.WriteLine($"Failed to write new memory to database.\nError message: {ex.Message}");
                return false; 
            }

            return true; 

        }

        // Return a list of all the memories
        public List<MemoryModel> getAllMemories()
        {
            return conn.Table<MemoryModel>().ToList(); 
        }

        // Locates the memory with the passed ID
        public MemoryModel findMemory(string memID)
        {
            
            try
            {
                MemoryModel foundMem = conn.Find<MemoryModel>(Int32.Parse(memID));
                return foundMem; 
            }

            catch(Exception ex)
            {
                Console.WriteLine($"Error. Failed to find memory matching ID {memID}.\nError message: {ex.Message}");
                return new MemoryModel(); 
            }

        }

        // Updates the passed memory in the database
        public bool editMemory(MemoryModel editMem)
        {
            try
            {
                conn.Update(editMem);
                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error. Failed to update memory with ID {editMem.ID}.\nError message: {ex.Message}");
                return false;
            }
        }

      
    }
}
