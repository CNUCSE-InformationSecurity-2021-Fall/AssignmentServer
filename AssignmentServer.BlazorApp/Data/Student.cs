using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Data
{
    public class Student
    {
        public string    Id { get; set; }

        public string    Password { get; set; }
        public string    PasswordSalt { get; set; }

        public DateTime? LastPasswordSet { get; set; }

        public string    Name { get; set; }

        private byte[]   MakeHash(byte[] plainText, byte[] salt) 
        {
            using var sha = SHA256.Create();

            var mixed = new List<byte>(salt[0..15]);
            mixed.AddRange(plainText);
            mixed.AddRange(salt[16..31]);

            return sha.ComputeHash(mixed.ToArray());
        }

        public bool      PasswordMatches(string plainText) 
        {
            return ToPassword(plainText, newSalt: false, updateEntity: false)
                   == Password;
        }

        public string    ToPassword(string plainText, bool newSalt, bool updateEntity) 
        { 
            var bytes = Encoding.UTF8.GetBytes(plainText);
            byte[] salt;

            if (newSalt || string.IsNullOrEmpty(PasswordSalt))
            {
                salt = new byte[32];

                RandomNumberGenerator.Create()
                                     .GetBytes(salt);
            }
            else
            {
                salt = Convert.FromBase64String(PasswordSalt);
            }

            var result  = MakeHash(bytes, salt);

            var password = Convert.ToBase64String(result);
            var passwordSalt = Convert.ToBase64String(salt);

            if (updateEntity) 
            {
                Password = password;
                PasswordSalt = passwordSalt;
            }

            return password;
        }
    }
}
