using System;
using Kur.Models;
using Microsoft.EntityFrameworkCore;

namespace Kur.Storeges
{
    public class FoodDbContext : DbContext
    {
        public FoodDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Food> Foods { get; set; }
    }
}
