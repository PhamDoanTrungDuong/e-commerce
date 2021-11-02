using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.ModelViews
{
    public class RegisterVM
    {
        [Key]
        public int CustomerId { get; set; }

        [Display(Name = "Họ và Tên")]
        [Required(ErrorMessage = "Vui lòng nhập Họ và Tên")]
        public string FullName { get; set; }

        [MaxLength(150)]
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [Display(Name = "Địa chỉ Email")]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "ValidateEmail", controller: "Accounts")]
        public string Email { get; set; }

        [MaxLength(11)]
        [Required(ErrorMessage = "Vui lòng nhập Số Điện Thoại")]
        [Display(Name = "Số Điện Thoại")]
        [DataType(DataType.PhoneNumber)]
        [Remote(action: "ValidatePhone", controller: "Accounts")]
        public string Phone { get; set; }

        [Display(Name = "Mật Khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(5, ErrorMessage = "Mật khẩu tối thiểu 5 kí tự")]
        public string Password { get; set; }

        [Display(Name = "Nhập lại mật Khẩu")]
        [Required(ErrorMessage = "Nhập lại mật khẩu")]
        [MinLength(5, ErrorMessage = "Mật khẩu tối thiểu 5 kí tự")]
        [Compare("Password", ErrorMessage = "Vui lòng nhập mật giống nhau")]
        public string ConfirmPassword { get; set; }
    }
}
