using Logic.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Entities
{
    public class Mood : BusinessObject<Mood>
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public int Score { get; set; }
    }
}
