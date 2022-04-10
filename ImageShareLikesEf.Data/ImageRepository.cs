using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageShareLikesEf.Data
{
    public class ImageRepository
    {
        private readonly string _ConnectionString;
        public ImageRepository(string connectionString)
        {
            _ConnectionString = connectionString;
        }
        public List<Image> GetAllImages()
        {
            using var context = new ImageDataContext(_ConnectionString);
            return context.Image.ToList();
        }
        public void AddImage(Image i)
        {
            using var context = new ImageDataContext(_ConnectionString);
            context.Image.Add(i);
            context.SaveChanges();
        }
    }
}
