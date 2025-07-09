using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FileUploaderDocspider.ViewModels
{
    public class DocumentViewModel
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres")]
        public string Title { get; set; }

        [StringLength(2000, ErrorMessage = "A descrição deve ter no máximo 2000 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "O arquivo é obrigatório")]
        public IFormFile File { get; set; }

        public string FileName { get; set; }
    }
}