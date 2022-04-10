using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageShareLikesEf.Data;

namespace ImageShareLikesEf.Web.Models
{
    public class ViewModel
    {
        public List<Image> Images { get; set; }
        public Image Image { get; set; }
        public bool AlreadyLiked { get; set; }
    }
}
