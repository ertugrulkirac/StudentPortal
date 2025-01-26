namespace StudentPortal.Web.Models
{
    public class EditStudentViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool Subscribed { get; set; }
        public IFormFile ImgUrl { get; set; } // Dosya yükleme için
    }

}
