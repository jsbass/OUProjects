using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Models.Json.Links
{
    public class Category
    {
        [JsonProperty("categoryId")]
        public int CategoryId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("links")]
        public List<Link> Links { get; set; }
        public Category()
        {
            this.Links = new List<Link>();
        }

        public Category(DB.Category category):this()
        {
            this.CategoryId = category.CategoryId;
            this.Name = category.Name;
            this.Description = category.Description;
            
            foreach(var linkCate in category.LinkCategories)
            {
                this.Links.Add(new Link(linkCate.Link));
            }

        }
    }
}