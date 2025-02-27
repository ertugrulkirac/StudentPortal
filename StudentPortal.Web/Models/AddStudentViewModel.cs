﻿namespace StudentPortal.Web.Models
{
    public class AddStudentViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public IFormFile ImgUrl { get; set; }
        public bool Subscribed { get; set; }

    }
}
