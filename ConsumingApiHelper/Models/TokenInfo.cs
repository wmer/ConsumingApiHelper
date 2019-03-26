using System;
using System.Collections.Generic;
using System.Text;

namespace ConsumingApiHelper.Models {
    public class TokenInfo {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
