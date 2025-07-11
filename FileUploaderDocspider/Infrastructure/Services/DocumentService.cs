using FileUploaderDocspider.Interfaces;
using FileUploaderDocspider.Models;
using FileUploaderDocspider.ViewModels;
using FileUploaderDocspider.Shared;
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

        public async Task<Result<IEnumerable<Document>>> GetAllDocumentsAsync()
        {
            try
            {
                var documents = await _documentRepository.GetAllAsync();
                return Result<IEnumerable<Document>>.Success(documents);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Document>>.Failure($"Erro ao buscar documentos: {ex.Message}");
            }
        }

        public async Task<Result<Document>> GetDocumentByIdAsync(int id)
        {
            try
            {
                var document = await _documentRepository.GetByIdAsync(id);
                if (document == null)
                {
                    return Result<Document>.Failure("Documento não encontrado.");
                }
                return Result<Document>.Success(document);
            }
            catch (Exception ex)
            {
                return Result<Document>.Failure($"Erro ao buscar documento: {ex.Message}");
            }
        }

        public async Task<Result<Document>> CreateDocumentAsync(DocumentViewModel model)
        {
            try
            {
                // Validar título único
                if (await _documentRepository.ExistsByTitleAsync(model.Title))
                {
                    return Result<Document>.Failure("Já existe um documento com este título.");
                }

                // Validar arquivo
                var validationResult = ValidateFileAsync(model.File);
                if (!validationResult.IsSuccess)
                {
                    return Result<Document>.Failure(validationResult.Message);
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

                var createdDocument = await _documentRepository.CreateAsync(document);
                return Result<Document>.Success(createdDocument);
            }
            catch (Exception ex)
            {
                return Result<Document>.Failure($"Erro ao criar documento: {ex.Message}");
            }
        }

        public async Task<Result<Document>> UpdateDocumentAsync(DocumentEditViewModel model)
        {
            try
            {
                var document = await _documentRepository.GetByIdAsync(model.Id);
                if (document == null)
                {
                    return Result<Document>.Failure("Documento não encontrado.");
                }

                // Validar título único (excluindo o documento atual)
                if (await _documentRepository.ExistsByTitleAsync(model.Title, model.Id))
                {
                    return Result<Document>.Failure("Já existe um documento com este título.");
                }

                document.Title = model.Title;
                document.Description = model.Description;
                document.FileName = model.FileName;

                // Se um novo arquivo foi enviado
                if (model.File != null)
                {
                    var validationResult = ValidateFileAsync(model.File);
                    if (!validationResult.IsSuccess)
                    {
                        return Result<Document>.Failure(validationResult.Message);
                    }

                    // Deletar arquivo antigo
                    DeleteFile(document.FilePath);

                    // Salvar novo arquivo
                    var fileName = await SaveFileAsync(model.File);
                    document.FilePath = fileName;
                    document.FileSize = model.File.Length;
                    document.ContentType = model.File.ContentType;
                }

                var updatedDocument = await _documentRepository.UpdateAsync(document);
                return Result<Document>.Success(updatedDocument);
            }
            catch (Exception ex)
            {
                return Result<Document>.Failure($"Erro ao atualizar documento: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteDocumentAsync(int id)
        {
            try
            {
                var document = await _documentRepository.GetByIdAsync(id);
                if (document == null)
                {
                    return Result<bool>.Failure("Documento não encontrado.");
                }

                // Deletar arquivo físico
                DeleteFile(document.FilePath);

                var deleted = await _documentRepository.DeleteAsync(id);
                return Result<bool>.Success(deleted);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Erro ao excluir documento: {ex.Message}");
            }
        }

        public Result<bool> ValidateFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Result<bool>.Failure("Arquivo é obrigatório.");

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (_blockedExtensions.Contains(extension))
            {
                return Result<bool>.Failure("Tipo de arquivo não permitido. Arquivos .exe, .zip e .bat não são aceitos.");
            }

            return Result<bool>.Success(true);
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