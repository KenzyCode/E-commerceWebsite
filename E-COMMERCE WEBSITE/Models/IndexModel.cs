using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_COMMERCE_WEBSITE.Models
{
    public class IndexModel
    {
        //the property that collects the search input 
        public string search { get; set; }
        // the property that keeps of the user's details
        public int UserId { get; set; }
    }
}