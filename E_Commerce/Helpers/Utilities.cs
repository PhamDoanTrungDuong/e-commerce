using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blogs.Helpers
{
    public static class Utilities
    {

        public static bool IsValidEmail(string email)
        {
            if (email.Trim().EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public static int PAGE_SIZE = 20;
        public static string ToVnd(this double donGia)
        {
            return donGia.ToString("#,##0") + " đ";
        }
        public static string ToUrlFriendly(this string url)
        {
            url = url.ToLower();
            url = Regex.Replace(url, @"[áàạảãâấầậẩẫăắằặẳẵ]", "a");
            url = Regex.Replace(url, @"[éèẹẻẽêếềệểễ]", "e");
            url = Regex.Replace(url, @"[óòọỏõôốồộổỗơớờợởỡ]", "o");
            url = Regex.Replace(url, @"[úùụủũưứừựửữ]", "u");
            url = Regex.Replace(url, @"[íìịỉĩ]", "i");
            url = Regex.Replace(url, @"[ýỳỵỷỹ]", "y");
            url = Regex.Replace(url, @"[đ]", "d");
            //Chỉ nhận [^0-9a-z-\s]
            url = Regex.Replace(url.Trim(), @"[^0-9a-z-\s]", "").Trim();
            //xử lý nhiều hơn 1 khoảng trắng
            url = Regex.Replace(url.Trim(), @"\s+", "-");
            //thay khoảng trắng bằng -
            url = Regex.Replace(url, @"\s", "-");
            while (true)
            {
                if(url.IndexOf("--") != -1)
                {
                    url = url.Replace("--", "-");
                }
                else
                {
                    break;
                }
            }

            return url;
        }
        public static bool IsInterger(string str)
        {
            Regex regex = new Regex(@"[^a-z0-9-]+$");

            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    return false;
                }
                if(!regex.IsMatch(str))
                {
                    return false;
                }
                return true;
            }
            catch
            {

            }
            return true;
        }
        public static string GetRandomKey(int length = 5)
        {
            //chuỗi mẫu (pattern)
            string pattern = @"0123456789zxcvbnmasdfghjklqwertyuiop[]{}:~!@#$%^&*()+";
            Random rd = new Random();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }

            return sb.ToString();
        }
        public static async Task<string> UploadFile(Microsoft.AspNetCore.Http.IFormFile file, string sDirectory, string newname = null)
        {
            try
            {
                if (newname == null) newname = file.FileName;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDirectory);
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt.ToLower())) // Khác các file định nghĩa
                {
                    return null;
                }
                else
                {
                    string fullPath = path + "//" + newname;
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return newname;
                }
            }
            catch
            {
                return null;
            }
        }
        public static void CreateIfMissing(string path)
        {
            bool folderExists = Directory.Exists(path);
            if(!folderExists)
            {
                Directory.CreateDirectory(path);
            }
        }
        public static string ToTitleCase(string str)
        {
            string result = str;
            if (!string.IsNullOrEmpty(str))
            {
                var words = str.Split(' ');
                for(int i=0; i<words.Length; i++)
                {
                    var s = words[i];
                    if (s.Length > 0)
                    {
                        words[i] = s[0].ToString().ToUpper() + s.Substring(1);
                    }
                }
                result = string.Join(" ", words);
            }
            return result;
        }
        public static string GeneratePassword(int length) //length of salt    
        {
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            var randNum = new Random();
            var chars = new char[length];
            var allowedCharCount = allowedChars.Length;
            for (var i = 0; i <= length - 1; i++)
            {
                chars[i] = allowedChars[Convert.ToInt32((allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }
        public static string EncodePassword(string pass, string salt) //encrypt password    
        {
            byte[] bytes = Encoding.Unicode.GetBytes(pass);
            byte[] src = Encoding.Unicode.GetBytes(salt);
            byte[] dst = new byte[src.Length + bytes.Length];
            System.Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            System.Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            HashAlgorithm algorithm = HashAlgorithm.Create("SHA1");
            byte[] inArray = algorithm.ComputeHash(dst);
            //return Convert.ToBase64String(inArray);    
            return EncodePasswordMd5(Convert.ToBase64String(inArray));
        }
        public static string EncodePasswordMd5(string pass) //Encrypt using MD5    
        {
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;
            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)    
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(pass);
            encodedBytes = md5.ComputeHash(originalBytes);
            //Convert encoded bytes back to a 'readable' string    
            return BitConverter.ToString(encodedBytes);
        }
        public static string base64Encode(string sData) // Encode    
        {
            try
            {
                byte[] encData_byte = new byte[sData.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(sData);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }
        public static string base64Decode(string sData) //Decode    
        {
            try
            {
                var encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();
                byte[] todecodeByte = Convert.FromBase64String(sData);
                int charCount = utf8Decode.GetCharCount(todecodeByte, 0, todecodeByte.Length);
                char[] decodedChar = new char[charCount];
                utf8Decode.GetChars(todecodeByte, 0, todecodeByte.Length, decodedChar, 0);
                string result = new String(decodedChar);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Decode" + ex.Message);
            }
        }
    }
}
