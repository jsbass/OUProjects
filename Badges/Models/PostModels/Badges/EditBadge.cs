using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;

namespace Badges.Models.PostModels.Badges
{
    public class EditBadge
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int EstimatedHours { get; set; }
        public List<EditRequirement> Requirements { get; set; }
        public string ImageSource { get; set; }

        public EditBadge()
        {
            Requirements = new List<EditRequirement>();
        }
    }

    public class EditRequirement
    {
        public int? Id { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public List<EditParam> Params { get; set; }

        public EditRequirement()
        {
            Params = new List<EditParam>();
        }
    }

    public class EditParam
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}