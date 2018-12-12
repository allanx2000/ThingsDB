using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateIt.GUI.Models
{
    public class Item : DatabaseItem
    {
        public Item()
        {
        }

        public Item(Category category, string name)
        {
            Category = category;
            Name = name;
        }

        public string Name { get; set; }
        public DateTime CreatedDate {get; set;}

        public Category Category { get; set; }

        public List<Tag> Tags { get; set; }

        public string TagsString
        {
            get
            {
                if (Tags == null || Tags.Count == 0)
                    return null;

                return string.Join(", ", from t in Tags orderby t.Name ascending select t.Name);
            }
        }

        public override string Value => Name;

    }
}
