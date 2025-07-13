using FileUploaderDocspider.Infrastructure.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploaderDocspider.Infrastructure.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] _blockedExtensions = { ".exe", ".zip", ".bat" };

        public DocumentService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public bool ValidateFile(IFormFile file, out string error)
        {
            error = null;
            if (file == null || file.Length == 0)
            {
                error = "Arquivo é obrigatório.";
                return false;
            }
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (_blockedExtensions.Contains(extension))
            {
                error = "Tipo de arquivo não permitido. Arquivos .exe, .zip e .bat não são aceitos.";
                return false;
            }
            return true;
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return fileName;
        }

        public void DeleteFile(string fileName)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}