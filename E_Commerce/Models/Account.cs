using System;
using System.Collections.Generic;

#nullable disable

namespace E_Commerce.Models
{
    public partial class Account
    {
        public Account()
        {
            Posts = new HashSet<Post>();
            Products = new HashSet<Product>();
        }

        public int AccountId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public bool Active { get; set; }
        public string FullName { get; set; }
        public int? RoleId { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? CreateDate { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
