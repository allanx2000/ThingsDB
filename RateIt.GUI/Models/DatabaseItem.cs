using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateIt.GUI.Models
{
    public abstract class DatabaseItem
    {
        public int ID { get; set; }
        public abstract string Value { get; }

        public override string ToString()
        {
            return GetType().Name + ": " + ID + ", " + Value;
        }

        public override bool Equals(object obj)
        {
            DatabaseItem other = obj as DatabaseItem;

            if (other == null || obj.GetType() != GetType())
                return false;
            else
                return other.ID == this.ID;
            
        }
    }
}
