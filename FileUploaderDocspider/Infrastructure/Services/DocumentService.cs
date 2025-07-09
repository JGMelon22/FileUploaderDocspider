using FileUploaderDocspider.Interfaces;
using FileUploaderDocspider.Models;
using FileUploaderDocspider.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploaderDocspider.Infrastructure.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] _blockedExtensions = { ".exe", ".zip", ".bat" };

        public DocumentService(IDocumentRepository documentRepository, IWebHostEnvironment webHostEnvironment)
        {
            _documentRepository = documentRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<Document>> GetAllDocumentsAsync()
        {
            return await _documentRepository.GetAllAsync();
        }

        public async Task<Document> GetDocumentByIdAsync(int id)
        {
            return await _documentRepository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string Message)> CreateDocumentAsync(DocumentViewModel model)
        {
            // Validar título único
            if (await _documentRepository.ExistsByTitleAsync(model.Title))
            {
                return (false, "Já existe um documento com este título.");
            }

            // Validar arquivo
            if (!ValidateFileAsync(model.File))
            {
                return (false, "Tipo de arquivo não permitido. Arquivos .exe, .zip e .bat não são aceitos.");
            }

            // Salvar arquivo
            var fileName = await SaveFileAsync(model.File);

            var document = new Document
            {
                Title = model.Title,
                Description = model.Description,
                FileName = model.FileName ?? model.File.FileName,
                FilePath = fileName,
                FileSize = model.File.Length,
                ContentType = model.File.ContentType,
                CreatedAt = DateTime.Now
            };

            await _documentRepository.CreateAsync(document);
            return (true, "Documento criado com sucesso!");
        }

        public async Task<(bool Success, string Message)> UpdateDocumentAsync(DocumentEditViewModel model)
        {
            var document = await _documentRepository.GetByIdAsync(model.Id);
            if (document == null)
            {
                return (false, "Documento não encontrado.");
            }

            // Validar título único (excluindo o documento atual)
            if (await _documentRepository.ExistsByTitleAsync(model.Title, model.Id))
            {
                return (false, "Já existe um documento com este título.");
            }

            document.Title = model.Title;
            document.Description = model.Description;
            document.FileName = model.FileName;

            // Se um novo arquivo foi enviado
            if (model.File != null)
            {
                if (!ValidateFileAsync(model.File))
                {
                    return (false, "Tipo de arquivo não permitido. Arquivos .exe, .zip e .bat não são aceitos.");
                }

                // Deletar arquivo antigo
                DeleteFile(document.FilePath);

                // Salvar novo arquivo
                var fileName = await SaveFileAsync(model.File);
                document.FilePath = fileName;
                document.FileSize = model.File.Length;
                document.ContentType = model.File.ContentType;
            }

            await _documentRepository.UpdateAsync(document);
            return (true, "Documento atualizado com sucesso!");
        }

        public async Task<(bool Success, string Message)> DeleteDocumentAsync(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
            {
                return (false, "Documento não encontrado.");
            }

            // Deletar arquivo físico
            DeleteFile(document.FilePath);

            await _documentRepository.DeleteAsync(id);
            return (true, "Documento excluído com sucesso!");
        }

        public bool ValidateFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLower();
            return !_blockedExtensions.Contains(extension);
        }

        private async Task<string> SaveFileAsync(IFormFile file)
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

        private void DeleteFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}