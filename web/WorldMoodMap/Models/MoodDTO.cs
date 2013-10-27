using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorldMoodMap.Models
{
    public class MoodDTO
    {
        public int userId { get; set; }
        public int moodId { get; set; }
        public int countryId { get; set; }
    }
}