using System;
using System.IO;
using Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server.ApplicationContext
{
    public class ChatContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }
        public string DbPath { get; }

        public ChatContext()
        {
            var settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "settings.json");
            
            dynamic settingsObject = JObject.Parse(File.ReadAllText(settingsPath));
            DbPath = Path.Combine(Directory.GetCurrentDirectory(), settingsObject.dbName.ToString());

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source = {DbPath}");
        }
    }
}