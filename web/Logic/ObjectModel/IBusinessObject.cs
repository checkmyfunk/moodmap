using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.ObjectModel
{
    public interface IBusinessObject : ITracking
    {
        IBusinessObject Save();
        IBusinessObject Update();
        void Delete();
    }

}
