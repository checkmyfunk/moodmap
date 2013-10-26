using Logic.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Entities
{
    [Table("Country")]
    public class Country : BusinessObject<Country>
    {
        public Country() { }

        public Country(string name, string abbreviation, int? score)
        {
            this.Name = name;
            this.Abbreviation = abbreviation;
            this.Score = score;
        }

        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public int? Score { get; set; }
    }
}
