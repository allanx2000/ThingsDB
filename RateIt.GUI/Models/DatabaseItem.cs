using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateIt.GUI.Models
{
    abstract class DatabaseItem
    {
        public int ID { get; set; }
        public abstract string Value { get; }

        public override string ToString()
        {
            return GetType().Name + ": " + ID + ", " + Value;
        }
    }
}
