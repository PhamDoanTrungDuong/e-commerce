using E_Commerce.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.ModelViews
{
    public class SeeDetails
    {
        [Key]
        public Order order { get; set; }
        public List<OrderDetail> orderDetails { get; set; }
    }
}
