using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Qbo
{
    public class Usuario
    {
        public int id { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public DateTime lastAccess { get; set; }
        public DateTime createdAt { get; set; }
        public ImageSource pfp { get; set; }
        
        public Usuario(int id, string email, string username, string password, DateTime lastAccess, DateTime createdAt, ImageSource pfp = null) {
            this.id = id;
            this.email = email; 
            this.username = username;
            this.password = password;
            this.lastAccess = lastAccess;
            this.createdAt = createdAt;
            this.pfp = pfp;
        }
    }
}
