using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateIt.GUI.Models;

namespace RateIt.GUI.Data
{
    interface IDataStore
    {
        Category AddCategory(string name);
        Tag AddTag(int categoryId, string name);
        Item AddItem(Item item);
        Item GetItem(int id);

        Category GetCategory(int id);
    }
}
