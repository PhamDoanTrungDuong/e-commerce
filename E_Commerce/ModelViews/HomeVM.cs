using E_Commerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.ModelViews
{
    public class HomeVM
    {
        public List<ProductHomeVM> Products { get; set; }
        public List<Post> Posts { get; set; }

    }
}
