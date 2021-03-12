using System;
using System.Collections.Generic;
using System.Text;
using SQLite; 

namespace GoodMemories.Models
{
    public class MemoryModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [NotNull]
        public string memoryText { get; set; }
        [NotNull]
        public long createdTimeStamp { get; set; }
        public string memoryName { get; set; }
        public string memoryDate { get; set; }
        public long lastUsedTimestamp { get; set; }
      

    }
}
