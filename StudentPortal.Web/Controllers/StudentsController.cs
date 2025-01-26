using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Web.Data;
using StudentPortal.Web.Models;
using StudentPortal.Web.Models.Entities;

namespace StudentPortal.Web.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IWebHostEnvironment _webhost;
  


        public StudentsController(ApplicationDbContext dbContext, IWebHostEnvironment webHost)
        {

            _webhost = webHost;
            this.dbContext = dbContext;
        }

        public async Task<string> SaveFileAsync(IFormFile file, IWebHostEnvironment webHostEnvironment, string folderName = "uploads")
        {
            if (file == null || file.Length == 0)
            {
                return null; // Dosya yok veya boş
            }

            // Upload klasörünün yolu
            string uploadPath = Path.Combine(webHostEnvironment.WebRootPath, folderName);

            // Klasör yoksa oluştur
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Benzersiz bir dosya adı oluştur
            string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            string filePath = Path.Combine(uploadPath, uniqueFileName);

            // Dosyayı kaydet
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Dosya URL'sini döndür
            return $"/{folderName}/{uniqueFileName}";
        }


        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Add(AddStudentViewModel viewModel)
        {

            string fileUrl = await SaveFileAsync(viewModel.ImgUrl, _webhost);

            var student = new Student
            {
                Name = viewModel.Name,
                Surname = viewModel.Surname,
                Email = viewModel.Email,
                Phone = viewModel.Phone,
                ImgUrl = fileUrl,
                Subscribed = viewModel.Subscribed
            };

       
            await dbContext.Students.AddAsync(student);
            await dbContext.SaveChangesAsync();

            ViewBag.Message = "Başarıyla kaydedildi.";

            return View();
        }

        //public async Task<IActionResult> Add(AddStudentViewModel viewModel)
        //{
        //   string filePath = null;
        //    if (viewModel.ImgUrl != null && viewModel.ImgUrl.Length > 0)
        //    {

        //        // Define the upload folder
        //        string uploadPath = Path.Combine(_webhost.WebRootPath, "uploads");
        //        // Create the folder if it doesn't exist
        //        if (!Directory.Exists(uploadPath))
        //        {
        //            Directory.CreateDirectory(uploadPath);
        //        }
        //        // Generate the file path
        //         filePath = Path.Combine(uploadPath, viewModel.ImgUrl.FileName).Replace("\\", "/"); ;
        //        // Save the file to the specified location
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await viewModel.ImgUrl.CopyToAsync(stream);
        //        }

        //    }

        //    var student = new Student
        //    {
        //        Name = viewModel.Name,
        //        Surname = viewModel.Surname,
        //        Email = viewModel.Email,
        //        Phone = viewModel.Phone,
        //        ImgUrl = filePath,
        //        Subscribed = viewModel.Subscribed
        //    };

        //    await dbContext.Students.AddAsync(student);
        //    await dbContext.SaveChangesAsync();
        //    ViewBag.Message = "Başarıyla kaydedildi.";

        //    return View();
        //}

        [HttpGet]

        public async Task<IActionResult> List()
        {
            var students = await dbContext.Students.ToListAsync();

            return View(students);

        }

        [HttpGet]

        public async Task<IActionResult> Edit(Guid id)
        {
            var student = await dbContext.Students.FindAsync(id);

            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditStudentViewModel viewModel)
        {
            var student = await dbContext.Students.FindAsync(viewModel.Id);

            if (student is not null)
            {
                string fileUrl = null;

                // Eğer yeni bir dosya yüklendiyse, kaydet ve URL'yi al
                if (viewModel.ImgUrl != null)
                {
                    fileUrl = await SaveFileAsync(viewModel.ImgUrl, _webhost);

                    // Eski dosyayı silmek isterseniz (isteğe bağlı)
                    if (!string.IsNullOrEmpty(student.ImgUrl))
                    {
                        string oldFilePath = Path.Combine(_webhost.WebRootPath, student.ImgUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                }

                // Mevcut öğrenci bilgilerini güncelle
                student.Name = viewModel.Name;
                student.Email = viewModel.Email;
                student.Phone = viewModel.Phone;

                // Yeni dosya URL'si varsa onu kullan, yoksa eski URL'yi koru
                student.ImgUrl = fileUrl ?? student.ImgUrl;

                student.Subscribed = viewModel.Subscribed;

                await dbContext.SaveChangesAsync();

                ViewBag.Message = "Başarıyla güncellendi.";
            }

            return RedirectToAction("List", "Students");
        }



        [HttpPost]

        public async Task<IActionResult> Delete(Student viewModel)
        {
            var student = await dbContext.Students.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == viewModel.Id);

            if (student is not null)
            {
                dbContext.Students.Remove(viewModel);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Students");
        }
    }
}
