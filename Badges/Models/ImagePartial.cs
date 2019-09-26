using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Badges.Models
{
    public partial class Image
    {
        public string ImageString => $"data:image/{Type},{Convert.ToBase64String(ImageData)}";
    }
}