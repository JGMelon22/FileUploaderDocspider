using FileUploaderDocspider.Interfaces;
using FileUploaderDocspider.Models;
using FileUploaderDocspider.Dtos.Requests;
using FileUploaderDocspider.Dtos.Responses;
using FileUploaderDocspider.Mappings;
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

        public async Task<Result<IEnumerable<DocumentResponse>>> GetAllDocumentsAsync()
        {
            try
            {
                var documents = await _documentRepository.GetAllAsync();
                var responses = documents.ToResponse();
                return Result<IEnumerable<DocumentResponse>>.Success(responses);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<DocumentResponse>>.Failure($"Erro ao buscar documentos: {ex.Message}");
            }
        }

        public async Task<Result<DocumentResponse>> GetDocumentByIdAsync(int id)
        {
            try
            {
                var document = await _documentRepository.GetByIdAsync(id);
                if (document == null)
                    return Result<DocumentResponse>.Failure("Documento não encontrado.");

                return Result<DocumentResponse>.Success(document.ToResponse());
            }
            catch (Exception ex)
            {
                return Result<DocumentResponse>.Failure($"Erro ao buscar documento: {ex.Message}");
            }
        }

        public async Task<Result<DocumentResponse>> CreateDocumentAsync(DocumentRequest request, IFormFile file)
        {
            try
            {
                if (await _documentRepository.ExistsByTitleAsync(request.Title))
                    return Result<DocumentResponse>.Failure("Já existe um documento com este título.");

                var validationResult = ValidateFileAsync(file);
                if (!validationResult.IsSuccess)
                    return Result<DocumentResponse>.Failure(validationResult.Message);

                var fileName = await SaveFileAsync(file);

                var document = request.ToDomain();
                document.FileName = file.FileName;
                document.FilePath = fileName;
                document.FileSize = file.Length;
                document.ContentType = file.ContentType;
                document.CreatedAt = DateTime.Now;

                var createdDocument = await _documentRepository.CreateAsync(document);
                return Result<DocumentResponse>.Success(createdDocument.ToResponse());
            }
            catch (Exception ex)
            {
                return Result<DocumentResponse>.Failure($"Erro ao criar documento: {ex.Message}");
            }
        }

        public async Task<Result<DocumentResponse>> UpdateDocumentAsync(int id, DocumentRequest request, IFormFile file)
        {
            try
            {
                var document = await _documentRepository.GetByIdAsync(id);
                if (document == null)
                    return Result<DocumentResponse>.Failure("Documento não encontrado.");

                if (await _documentRepository.ExistsByTitleAsync(request.Title, id))
                    return Result<DocumentResponse>.Failure("Já existe um documento com este título.");

                document.UpdateFromRequest(request);

                if (file != null)
                {
                    var validationResult = ValidateFileAsync(file);
                    if (!validationResult.IsSuccess)
                        return Result<DocumentResponse>.Failure(validationResult.Message);

                    DeleteFile(document.FilePath);

                    var fileName = await SaveFileAsync(file);
                    document.FileName = file.FileName;
                    document.FilePath = fileName;
                    document.FileSize = file.Length;
                    document.ContentType = file.ContentType;
                }

                var updatedDocument = await _documentRepository.UpdateAsync(document);
                return Result<DocumentResponse>.Success(updatedDocument.ToResponse());
            }
            catch (Exception ex)
            {
                return Result<DocumentResponse>.Failure($"Erro ao atualizar documento: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteDocumentAsync(int id)
        {
            try
            {
                var document = await _documentRepository.GetByIdAsync(id);
                if (document == null)
                    return Result<bool>.Failure("Documento não encontrado.");

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
                return Result<bool>.Failure("Tipo de arquivo não permitido. Arquivos .exe, .zip e .bat não são aceitos.");

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