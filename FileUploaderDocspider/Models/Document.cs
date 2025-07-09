using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FileUploaderDocspider.Models
{
    public class Document
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres")]
        public string Title { get; set; }

        [StringLength(2000, ErrorMessage = "A descrição deve ter no máximo 2000 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "O nome do arquivo é obrigatório")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "O caminho do arquivo é obrigatório")]
        public string FilePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public long FileSize { get; set; }

        public string ContentType { get; set; }
    }
}