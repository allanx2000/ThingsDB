using RateIt.GUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateIt.GUI.Data
{
    class SearchCriteria
    {
        public Category CategoryValue { get; private set; }
        public List<Tag> Tags { get; private set; }
        public string Filter { get; private set; }
        public bool RatedOnly { get; private set; }
        public bool UntaggedOnly { get; private set; }
        public bool HasTags { get { return Tags != null && Tags.Count > 0; } }

        public SearchCriteria(Category category, List<Tag> tags = null, string filter = null,
            bool ratedOnly = false, bool untaggedOnly = false)
        {
            CategoryValue = category;
            Tags = tags;
            Filter = filter;
            RatedOnly = ratedOnly;
            UntaggedOnly = untaggedOnly;
        }

        public override string ToString()
        {
            List<string> parts = new List<string>();

            parts.Add("Category: " + CategoryValue.Name);

            if (!string.IsNullOrEmpty(Filter))
                parts.Add("Filter: " + Filter);

            if (HasTags)
                parts.Add("Tags: " + string.Join(", ", Tags));

            if (RatedOnly)
                parts.Add("Rated Only");

            return string.Join("; ", parts);
        }
    }
}
