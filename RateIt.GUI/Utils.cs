using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using RateIt.GUI.Models;

namespace RateIt.GUI
{
    class Utils
    {
        internal static string TagsListToString(List<Tag> tags, string nullValue = "(All)")
        {
            if (tags == null || tags.Count == 0)
                return nullValue;
            else
                return string.Join(", ", from x in tags
                                         orderby x.Name ascending
                                         select x.Name);
        }

        public static string ObjectToString(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, obj);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static object StringToObject(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;
                return new BinaryFormatter().Deserialize(ms);
            }
        }
    }
}
