using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.ModelViews
{
    public class ShoppingVM
    {
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "Vui Lòng Nhập Họ Tên")]
        public string FullName { get; set; }
        public string  Email { get; set; }
        [Required(ErrorMessage = "Vui Lòng Nhập Số Điện Thoại")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Vui Lòng Nhập Địa Chỉ Giao Hàng")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Vui Lòng Nhập Tên Tỉnh/Thành")]
        public int TinhThanh { get; set; }
        [Required(ErrorMessage = "Vui Lòng Nhập Tên Quận/Huyện")]
        public int QuanHuyen { get; set; }
        [Required(ErrorMessage = "Vui Lòng Nhập Tên Phường/Xã")]
        public int PhuongXa { get; set; }
        public int PaymentID { get; set; }
        public string Note { get; set; }
    }
}
