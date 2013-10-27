using Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorldMoodMap.Models
{
    public class UserFriend : MyAppUserFriend
    {
        public Mood Mood { get; set; }
    }
}