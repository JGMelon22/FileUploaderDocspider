using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace FileUploaderDocspider.ViewModels
{
    public class DocumentEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres")]
        public string Title { get; set; }

        [StringLength(2000, ErrorMessage = "A descrição deve ter no máximo 2000 caracteres")]
        public string Description { get; set; }

        [Display(Name = "Arquivo atual")]
        public string FileName { get; set; }

        [Display(Name = "Novo arquivo (opcional)")]
        public IFormFile File { get; set; }

        public string ExistingFilePath { get; set; }

        [Display(Name = "Criado em")]
        public DateTime CreatedAt { get; set; }
    }
}