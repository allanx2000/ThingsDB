using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateIt.GUI.Models
{
    public class Category : DatabaseItem
    {
        public Category(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public string Name { get; set; }

        public override string Value => Name;

        //Only used for *Manager
        public int ItemCount { get; set; }
    }
}
