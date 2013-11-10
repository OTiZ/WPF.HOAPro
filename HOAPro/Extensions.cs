using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace HOAPro
{
    public static class Extensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> coll)
        {
            var c = new ObservableCollection<T>();
            foreach (var e in coll)
                c.Add(e);
            return c;
        }
    }
}
