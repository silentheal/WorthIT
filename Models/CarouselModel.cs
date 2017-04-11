using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorthIt.Models
{
    public class CarouselModel
    {
        [Key]
        public int? CarouselId { get; set; }
        public string Image { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        public string Button { get; set; }       
        public string Link { get; set; }
        public bool? Active { get; set; }
    }
}