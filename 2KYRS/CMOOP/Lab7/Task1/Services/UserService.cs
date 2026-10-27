using Lab7.Task1.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Lab7.Task1.Services
{
    public class UserService
    {
        private const string FilePath = "users.json";

        public static List<User> LoadUsers()
        {
            if (!File.Exists(FilePath))
                return new List<User>();

            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        public static bool SaveUser(User newUser, out string errorMessage)
        {
            errorMessage = string.Empty;
            var users = LoadUsers();

            if (users.Exists(u => u.Username.Equals(newUser.Username, StringComparison.OrdinalIgnoreCase)))
            {
                errorMessage = "Користувач з таким логіном вже існує!";
                return false;
            }

            users.Add(newUser);
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
            return true;
        }

        public static bool ValidateUser(string usernameOrEmail, string password)
        {
            var users = LoadUsers();
            return users.Exists(u => (u.Username == usernameOrEmail || u.Email == usernameOrEmail) && u.Password == password);
        }
    }
}