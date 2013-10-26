using Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorldMoodMap.Models
{
    public class HomeModel
    {
        public MyAppUser User { get; set; }
        public Mood Mood { get; set; }
        public Country Country { get; set; }
    }
}