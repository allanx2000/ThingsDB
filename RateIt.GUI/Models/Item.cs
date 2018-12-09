﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateIt.GUI.Models
{
    class Item
    {
        public Item()
        {
        }

        public Item(Category category, string name)
        {
            Category = category;
            Name = name;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate {get; set;}

        public Category Category { get; set; }

        public List<Tag> Tags { get; set; }
    }
}
