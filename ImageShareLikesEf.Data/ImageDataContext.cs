using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageShareLikesEf.Data
{
    public class ImageDataContext : DbContext
    {
        private readonly string _ConnectionString;
        public ImageDataContext(string connectionString)
        {
            _ConnectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_ConnectionString);
        }
        public DbSet<Image> Image { get; set; }
    }
}
