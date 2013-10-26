using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.ObjectModel
{
    public class NoTrackAttribute : Attribute { };

    public interface ITracking
    {
        List<PropertyTrack> GetChanges();
        string Name { get; }
        string Id { get; }
    }
}
