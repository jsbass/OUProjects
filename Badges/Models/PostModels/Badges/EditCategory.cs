using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Badges.Models.PostModels.Badges
{
    public class EditCategory
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<int> BadgeIds { get; set; }

        public string ImageSource { get; set; }

        public EditCategory()
        {
            BadgeIds = new List<int>();
        }
    }
}