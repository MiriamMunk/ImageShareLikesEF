using ImageShareLikesEf.Data;
using ImageShareLikesEf.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageShareLikesEf.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _connectionString;
        public HomeController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(string title, IFormFile imageFile)
        {
            ImageRepository repo = new(_connectionString);

            string fileName = $"{Guid.NewGuid()}-{imageFile.FileName}";
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using var fs = new FileStream(filePath, FileMode.CreateNew);
            imageFile.CopyTo(fs);

            repo.AddImage(new Image
            {
                Path = fileName,
                Title = title,
                DateCreated = DateTime.Now
            });
            return RedirectToAction("Home");
        }

        public IActionResult Home()
        {
            ImageRepository repo = new(_connectionString);
            return View(new ViewModel { Images = repo.GetAllImages().OrderByDescending(x => x.DateCreated).ToList() });
        }

        public IActionResult Viewimage(int id)
        {
            using var context = new ImageDataContext(_connectionString);
            ViewModel vm = new();
            var image = context.Image.FirstOrDefault(x => x.Id == id);

            if (image == null)
            {
                return RedirectToAction("home");
            }
            else
            {
                vm.Image = image;
            }

            var LikedImageIds = HttpContext.Session.Get<List<int>>("ImageIds");
            if (LikedImageIds == null)
            {
                LikedImageIds = new List<int>();
            }
            if (LikedImageIds.Contains(id))
            {
                vm.AlreadyLiked = true;
            }
            return View(vm);
        }

        [HttpPost]
        public void AddLike(int id)
        {
            using var context = new ImageDataContext(_connectionString);
            context.Database.ExecuteSqlInterpolated($"UPDATE Image SET Likes = Likes + 1 WHERE id = {id}");
            context.SaveChanges();

            var LikedImageIds = HttpContext.Session.Get<List<int>>("ImageIds");
            if (LikedImageIds == null)
            {
                LikedImageIds = new List<int>();
            }
            LikedImageIds.Add(id);

            HttpContext.Session.Set("ImageIds", LikedImageIds);
        }

        [HttpPost]
        public IActionResult GetLikes(int id)
        {
            using var context = new ImageDataContext(_connectionString);
            return Json(context.Image.FirstOrDefault(x => x.Id == id).Likes);
        }
    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
    }
}
