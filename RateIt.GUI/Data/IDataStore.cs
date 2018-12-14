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
        Item UpsertItem(Item item);
        Item GetItem(int id);

        List<Item> GetItemsForTag(int tagId);
        Tag GetTag(int tagId);

        Category GetCategory(int id);
        List<Category> GetAllCategories();

        List<Item> Search(SearchCriteria criteria);
        List<Category> GetAllCategoriesWithCount();
        void DeleteCategory(int id);
        void UpdateCategory(Category category);
        void DeleteTag(int id);
        void UpdateTag(Tag selectedTag);
        List<Tag> GetAllTagsWithCount(int categoryId);
        void DeleteItem(Item selectedResultItem);
    }
}
