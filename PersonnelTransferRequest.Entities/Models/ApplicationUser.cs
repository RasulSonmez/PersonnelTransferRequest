
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PersonnelTransferRequest.Common.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonnelTransferRequest.Entities.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Required(ErrorMessage = "Zorunlu alan")]
        [MinLength(1, ErrorMessage = "Sicil alanı en az 1 karakter olmalıdır.")]
        [MaxLength(10, ErrorMessage = "Sicil alanı en fazla 10 karakter olmalıdır.")]
        [Display(Name = "Sicil")]
        public string RegistrationNumber { get; set; }


        [Required(ErrorMessage = "Zorunlu alan")]      
        [Display(Name = "Unvan")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Zorunlu alan")]
        [MinLength(3, ErrorMessage = "İsim alanı en az 3 karakter olmalıdır.")]
        [MaxLength(128, ErrorMessage = "İsim alanı en fazla 128 karakter olmalıdır.")]
        [Display(Name = "İsim")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Zorunlu alan")]
        [MinLength(3, ErrorMessage = "Soyisim en az 3 karakter olmalıdır.")]
        [MaxLength(200, ErrorMessage = "Soyisim en fazla 200 karakter olmalıdır.")]
        [Display(Name = "Soyisim")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Zorunlu alan")]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "Hatalı TC Kimlik No formatı!")]
        [Display(Name = "TC Kimlik No")]
    
        public string? TCKN { get; set; }

        [Required(ErrorMessage = "Zorunlu alan")]
        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Only digits allowed. Must be between 10-15 digits.")]
        [Display(Name = "Telefon Numarası")]
        public string GSM { get; set; }


        [Required(ErrorMessage = "Zorunlu alan")]
        [Display(Name = "Görev Yeri")]
        public string DutyStation { get; set; }

        [Display(Name = "Fotoğraf")]
        public string? ProfilPhotoPath { get; set; }

        [Display(Name = "Oluşturulma Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public bool IsDelete { get; set; }

        //Not mapped
        [NotMapped]
        [Display(Name = "Fotoğraf")]
        [DataType(DataType.Upload)]
        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".svg", "jpeg" })]
        public IFormFile? ProfilPhotoFile { get; set; }
    }
}
