using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Models.Json.Links
{
    public class Link
    {
        public int LinkId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public Nullable<bool> InNewWindow { get; set; }

        public Link()
        {
            this.InNewWindow = true;
        }

        public Link(DB.Link link)
        {
            this.LinkId = link.LinkId;
            this.Name = link.Name;
            this.Description = link.Description;
            this.Url = link.Url;
            this.InNewWindow = link.InNewWindow;
        }
    }
}