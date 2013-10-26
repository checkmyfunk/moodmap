using Logic.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Entities
{
    public class User : BusinessObject<User>
    {
        public string FacebookId { get; set; }

        public int? MoodId { get; set; }
        [NonSerialized]
        private Mood mood;
        public Mood Mood
        {
            get { return mood ?? (mood = MoodId.HasValue ? Mood.GetById(MoodId) : (null as Mood)); }
            set
            {
                mood = value;
                MoodId = value.Id;
            }
        }

        public int? CountryId { get; set; }
        [NonSerialized]
        private Country country;
        public Country Country
        {
            get { return country ?? (country = CountryId.HasValue ? Country.GetById(CountryId) : (null as Country)); }
            set
            {
                country = value;
                CountryId = value.Id;
            }
        }

        public bool IsManualCountrySelection { get; set; }
    }
}
