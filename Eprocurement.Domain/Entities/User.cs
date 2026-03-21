using Eprocurement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;
using System.Xml;
using BCrypt.Net;

namespace Eprocurement.Domain.Entities
{
    [Table("User")]
    public class User
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public UserRolesEnum Role { get; private set; }
        public bool IsActive { get; private set; }

        private User()
        {
            Name = string.Empty;
            Email = string.Empty;
            PasswordHash = string.Empty;
        }

        public User(string name, string email, string passwordHash, UserRolesEnum role)
        {
            Name = name;
            Email = email;
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordHash);
            Role = role;
            IsActive = true;
        }
        public void Create(string name, string email, string passwordHash, UserRolesEnum role)
        {
            Name = name;
            Email = email;
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordHash);
            Role = role;
            IsActive = true;
        }
        public void ChangeRole(UserRolesEnum roleEnum) => Role = roleEnum;
        public void Deactivate() => IsActive = false;
    }
}
