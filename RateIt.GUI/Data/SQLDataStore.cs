using Innouvous.Utils.Data;
using RateIt.GUI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateIt.GUI.Data
{
    class SQLDataStore : IDataStore
    {
        private SQLiteClient client;

        public SQLDataStore(string dbFile)
        {
            bool isNew = !File.Exists(dbFile);

            var args = new Dictionary<string, string>() {
                { "FKSupport", "True"}
            };

            client = new SQLiteClient(dbFile, isNew, args);

            CreateTables();
        }

        private const string ItemsTable = "tbl_items";
        private const string CategoryTable = "tbl_categories";
        private const string TagsTable = "tbl_tags";
        private const string ItemTagsTable = "tbl_item_tags";
        private const string ItemAttributesTable = "tbl_item_attrs";

        private const string ScriptsPath = "TableScripts";
        private const string ScriptsFormat = "txt";

        private string LoadFromText(string name, params object[] args)
        {
            return SQLUtils.LoadCommandFromText(ScriptsPath, name, ScriptsFormat, args);

        }

        #region Create Tables

        private void CreateTables()
        {
            if (!SQLUtils.CheckTableExists(CategoryTable, client))
            {
                CreateCategoriesTable();
            }

            if (!SQLUtils.CheckTableExists(ItemsTable, client))
            {
                CreateItemsTable();
            }

            if (!SQLUtils.CheckTableExists(TagsTable, client))
            {
                CreateTagsTable();
            }

            if (!SQLUtils.CheckTableExists(ItemTagsTable, client))
            {
                CreateItemTagsTable();
            }

            if (!SQLUtils.CheckTableExists(ItemAttributesTable, client))
            {
                CreateItemAttributesTable();
            }
        }


        private void CreateItemAttributesTable()
        {
            string cmd = LoadFromText("CreateItemAttributesTable", ItemAttributesTable, ItemsTable);
            client.ExecuteNonQuery(cmd);
        }

        private void CreateItemTagsTable()
        {
            string cmd = LoadFromText("CreateItemTagsTable", ItemTagsTable, ItemsTable, TagsTable);
            client.ExecuteNonQuery(cmd);
        }

        private void CreateTagsTable()
        {
            string cmd = LoadFromText("CreateTagsTable", TagsTable, CategoryTable);
            client.ExecuteNonQuery(cmd);
        }

        private void CreateItemsTable()
        {
            string cmd = LoadFromText("CreateItemsTable", ItemsTable, CategoryTable);
            client.ExecuteNonQuery(cmd);
        }

        private void CreateCategoriesTable()
        {
            string cmd = LoadFromText("CreateCategoriesTable", CategoryTable);
            client.ExecuteNonQuery(cmd);
        }

        #endregion

        public Category AddCategory(string name)
        {
            string cmd = string.Format("insert into {0} values(NULL,'{1}')", CategoryTable, 
                SQLUtils.SQLEncode(name));
            client.ExecuteNonQuery(cmd);

            int id = SQLUtils.GetLastInsertRow(client);
            Category c = new Category(id, name);

            return c;
        }

        public Tag AddTag(int categoryId, string name)
        {
            string cmd = string.Format("insert into {0} values(NULL,'{1}', {2})", TagsTable,
               SQLUtils.SQLEncode(name),
               categoryId);
            client.ExecuteNonQuery(cmd);

            int id = SQLUtils.GetLastInsertRow(client);
            Tag t = new Tag(id, name);

            return t;
        }

        public Item UpsertItem(Item item)
        {

            var txn = client.GetConnection().BeginTransaction();

            try
            {
                string cmd;

                //Upsert Item
                if (item.ID == 0)
                {
                    cmd = string.Format("insert into {0} values(NULL, '{1}', '{2}', {3})",
                        ItemsTable,
                        SQLUtils.ToSQLDateTime(DateTime.Now),
                        SQLUtils.SQLEncode(item.Name),
                        item.Category.ID
                        );

                    client.ExecuteNonQuery(cmd);
                    item.ID = SQLUtils.GetLastInsertRow(client);
                }
                else
                {
                    cmd = string.Format("update {0} set item_name='{1}', category_id={2} where item_id = {3}",
                        ItemsTable,
                        SQLUtils.SQLEncode(item.Name),
                        item.Category.ID,
                        item.ID
                        );

                    client.ExecuteNonQuery(cmd);
                }

                //Upsert Tags
                cmd = string.Format("delete from {0} where item_id = {1}", ItemTagsTable, item.ID);
                client.ExecuteNonQuery(cmd);

                if (item.Tags != null)
                {
                    foreach (var t in item.Tags)
                    {
                        cmd = string.Format("insert into {0} values({1},{2})", ItemTagsTable, 
                            item.ID, t.ID);
                        client.ExecuteNonQuery(cmd);
                    }
                }

                //Upsert Other Attributes
                cmd = $"delete from {ItemAttributesTable} where item_id = {item.ID}";
                client.ExecuteNonQuery(cmd);

                if (!string.IsNullOrEmpty(item.Notes))
                {
                    string b64 = Utils.ObjectToString(item.Notes);
                    client.ExecuteNonQuery($"insert into {ItemAttributesTable} values({item.ID},{(int)AttributeType.Notes},'{b64}')");
                }

                if (!string.IsNullOrEmpty(item.URL))
                {
                    string b64 = Utils.ObjectToString(item.URL);
                    client.ExecuteNonQuery($"insert into {ItemAttributesTable} values({item.ID},{(int)AttributeType.Url},'{b64}')");
                }

                if (item.Rating != 0)
                {
                    client.ExecuteNonQuery($"insert into {ItemAttributesTable} values({item.ID},{(int)AttributeType.Rating},{item.Rating})");
                }


                txn.Commit();

                return GetItem(item.ID);
            }
            catch (Exception e)
            {
                txn.Rollback();
                throw;
            }
        }

        public Item GetItem(int id)
        {
            string cmd = string.Format("select * from {0} where item_id={1}", ItemsTable, id);

            DataTable data = client.ExecuteSelect(cmd);
            if (data.Rows.Count == 0)
                return null;

            Item item = ParseBasicItem(data.Rows[0]);
            item.Tags = GetTagsForItem(id);

            AttachAttributes(item);

            return item;
        }

        private void AttachAttributes(Item item)
        {
            DataTable dt = client.ExecuteSelect($"select * from {ItemAttributesTable} where item_id ={item.ID}");

            foreach (DataRow r in dt.Rows)
            {
                AttributeType type = (AttributeType)Convert.ToInt32(r["attr_id"]);
                var value = r["value"];

                switch (type)
                {
                    case AttributeType.Notes:
                        item.Notes = (string) Utils.StringToObject(value.ToString());
                        break;
                    case AttributeType.Url:
                        item.URL = (string)Utils.StringToObject(value.ToString());
                        break;
                    case AttributeType.Rating:
                        item.Rating = Convert.ToInt32(value);
                        break;
                }
            }
        }

        private List<Tag> GetTagsForItem(int id)
        {
            List<Tag> tags = new List<Tag>();

            string cmd = string.Format("select t.* from {0} m join {1} t on m.tag_id = t.tag_id where m.item_id={2}",
                ItemTagsTable, TagsTable, id);

            DataTable data = client.ExecuteSelect(cmd);

            foreach (DataRow r in data.Rows)
            {
                tags.Add(ParseTag(r));
            }

            return tags;
        }

        private Item ParseBasicItem(DataRow r)
        {
            Item item = new Item();
            item.ID = Convert.ToInt32(r["item_id"]);
            item.Name = r["item_name"].ToString();
            item.CreatedDate = SQLUtils.ToDateTime(r["created_date"].ToString());

            if (!SQLUtils.IsNull(r["category_id"]))
            {
                Category category = GetCategory(Convert.ToInt32(r["category_id"]));
                item.Category = category;
            }

            return item;
        }

        private Tag ParseTag(DataRow r)
        {
            Tag t = new Tag(
                Convert.ToInt32(r["tag_id"]),
                r["tag_name"].ToString()
            );

            return t;
        }


        public List<Category> GetAllCategories()
        {
            string cmd = "select * from " + CategoryTable;

            var dt = client.ExecuteSelect(cmd);

            List<Category> categories = new List<Category>();

            foreach (DataRow row in dt.Rows)
            {
               categories.Add(ParseCategory(row));
            }
            return categories;
        }

        private Category ParseCategory(DataRow row)
        {
            return new Category(
                Convert.ToInt32(row["category_id"]),
                row["category_name"].ToString()
                );
        }

        public Category GetCategory(int id)
        {
            string cmd = string.Format("select * from {0} where category_id = {1}", CategoryTable, id);

            var dt = client.ExecuteSelect(cmd);

            if (dt.Rows.Count == 0)
                return null;
            else return ParseCategory(dt.Rows[0]);
        }

        public List<Item> GetItemsForTag(int tagId)
        {
            List<Item> items = new List<Item>();

            string cmd = string.Format("select item_id from {0} m where m.tag_id={1}",
                ItemTagsTable, tagId);

            DataTable data = client.ExecuteSelect(cmd);

            foreach (DataRow r in data.Rows)
            {
                items.Add(GetItem(Convert.ToInt32(r["item_id"])));
            }

            return items;
        }

        public Tag GetTag(int tagId)
        {
            var dt = client.ExecuteSelect(string.Format("select * from {0} where tag_id = {1}", TagsTable, tagId));

            if (dt.Rows.Count == 0)
                return null;

            return ParseTag(dt.Rows[0]);
        }

        public List<Item> Search(SearchCriteria criteria)
        {
            List<Item> items;
            if (criteria.HasTags)
            {
                items = GetItemsForTags(criteria.Tags);
                if (!string.IsNullOrEmpty(criteria.Filter))
                    items = (from i in items
                             where i.Name.ToLower().Contains(criteria.Filter.ToLower())
                             select i).ToList();

            }
            else
            {
                items = GetItemsForCategory(criteria.CategoryValue, criteria.Filter);
                //Utaged
            }

            //if (criteria.RatedOnly)

            return items;
        }

        private List<Item> GetItemsForCategory(Category category, string filter = null)
        {
            string cmd = $"SELECT item_id from {ItemsTable} WHERE category_id = {category.ID}";
            if (!string.IsNullOrEmpty(filter))
                cmd += " AND item_name LIKE '%"+ SQLUtils.SQLEncode(filter) + "%'";

            return GetFromItemIds(client.ExecuteSelect(cmd));
        }

        private List<Item> GetFromItemIds(DataTable itemIdsTable)
        {
            List<Item> items = new List<Item>();

            foreach (DataRow r in itemIdsTable.Rows)
            {
                items.Add(GetItem(Convert.ToInt32(r["item_id"])));
            }

            return items;
        }

        private List<Item> GetItemsForTags(List<Tag> tags)
        {
            List<Item> items = new List<Item>();

            string tagIds = string.Join(",", from t in tags select t.ID);
            string cmd = $"select distinct item_id from {ItemTagsTable} where tag_id in ({tagIds})";
            return GetFromItemIds(client.ExecuteSelect(cmd));
        }

        public List<Category> GetAllCategoriesWithCount()
        {
            string cmd = $"SELECT c.*, (SELECT count(*) from {ItemsTable} i where i.category_id = c.category_id) as items" +
                $" FROM {CategoryTable} c" +
                " ORDER BY items DESC";

            DataTable dt = client.ExecuteSelect(cmd);

            List<Category> results = new List<Category>();

            foreach (DataRow r in dt.Rows)
            {
                var c = ParseCategory(r);
                c.ItemCount = Convert.ToInt32(r["items"]);
                results.Add(c);
            }

            return results;
        }

        public void DeleteCategory(int id)
        {
            var count = client.ExecuteScalar($"select count(*) from {ItemsTable} where category_id={id}");

            if (Convert.ToInt32(count) > 0)
                throw new Exception("Cannot delete category with items in it.");

            client.ExecuteNonQuery($"delete from {CategoryTable} where category_id={id}");
        }

        public void UpdateCategory(Category category)
        {
            string cmd = $"UPDATE {CategoryTable}" +
                $" SET category_name='{SQLUtils.SQLEncode(category.Name)}'" +
                $" WHERE category_id={category.ID}";

            client.ExecuteNonQuery(cmd);
        }

        public void DeleteTag(int id)
        {
            client.ExecuteNonQuery($"delete from {TagsTable} where tag_id={id}");
        }

        public void UpdateTag(Tag tag)
        {
            string cmd = $"UPDATE {TagsTable}" +
                $" SET tag_name='{SQLUtils.SQLEncode(tag.Name)}'" +
                $" WHERE tag_id={tag.ID}";

            client.ExecuteNonQuery(cmd);
        }

        public List<Tag> GetAllTagsWithCount(int categoryId)
        {

            string cmd = $"SELECT t.*, (SELECT count(*) from {ItemTagsTable} it where it.tag_id = t.tag_id) as items" +
                $" FROM {TagsTable} t WHERE t.category_id = {categoryId}" +
                " ORDER BY items DESC";

            DataTable dt = client.ExecuteSelect(cmd);

            List<Tag> results = new List<Tag>();

            foreach (DataRow r in dt.Rows)
            {
                var tag = ParseTag(r);
                tag.ItemCount = Convert.ToInt32(r["items"]);
                results.Add(tag);
            }

            return results;
        }
    }
}
