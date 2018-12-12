using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateIt.GUI.Models;

namespace RateIt.GUI
{
    class Utils
    {
        internal static string TagsListToString(List<Tag> tags)
        {
            if (tags == null || tags.Count == 0)
                return "(All)";
            else
                return string.Join(", ", from x in tags
                                         orderby x.Name ascending
                                         select x.Name);
        }
    }
}
