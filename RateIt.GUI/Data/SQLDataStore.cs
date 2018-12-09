﻿using Innouvous.Utils.Data;
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

        public Item AddItem(Item item)
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

                    client.ExecuteScalar(cmd);
                    item.ID = SQLUtils.GetLastInsertRow(client);
                }
                else
                {
                    cmd = string.Format("update {0} set name='{1}', category_id={2} where id = {3}",
                        ItemsTable,
                        SQLUtils.ToSQLDateTime(DateTime.Now),
                        SQLUtils.SQLEncode(item.Name),
                        item.Category.ID,
                        item.ID
                        );
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

                //TODO: Upsert Other Attributes

                txn.Commit();

                return item;
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

            return item;
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

        public Category GetCategory(int id)
        {
            string cmd = string.Format("select * from {0} where category_id = {1}", CategoryTable, id);

            var dt = client.ExecuteSelect(cmd);

            if (dt.Rows.Count == 0)
                return null;

            Category cat = new Category(
                Convert.ToInt32(dt.Rows[0]["category_id"]),
                dt.Rows[0]["category_name"].ToString()
                );

            return cat;
        }

    }
}
