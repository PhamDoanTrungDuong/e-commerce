﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.ModelViews
{
    public class LoginViewModel
    {
        [Key]
        [MaxLength(50)]
        [Required(ErrorMessage = "Vui lòng nhập hoặc Email")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Vui lòng nhập Email")]
        public string UserName { get; set; }

        [Display(Name = "Mật Khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MaxLength(30, ErrorMessage = "Mật khẩu không quá 30 kí tự")]
        public string Password { get; set; }
    }
}