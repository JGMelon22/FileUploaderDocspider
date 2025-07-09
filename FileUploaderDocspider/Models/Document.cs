using System;
using System.ComponentModel.DataAnnotations;

namespace FileUploaderDocspider.Models
{
    public class Document
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres")]
        public string Tile { get; set; }

        [StringLength(2000, ErrorMessage = "A descrição deve ter no máximo 2000 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "O arquivo é obrigatório")]
        public IFormFile File { get; set; }

        public string FileName { get; set; }
    }
}