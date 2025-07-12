using FileUploaderDocspider.Interfaces;
using FileUploaderDocspider.Mappings;
using FileUploaderDocspider.Shared;
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

        public async Task<Result<IEnumerable<DocumentViewModel>>> GetAllDocumentsAsync()
        {
            try
            {
                var documents = await _documentRepository.GetAllAsync();
                var viewModels = documents.ToViewModel();
                return Result<IEnumerable<DocumentViewModel>>.Success(viewModels);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<DocumentViewModel>>.Failure($"Erro ao buscar documentos: {ex.Message}");
            }
        }

        public async Task<Result<DocumentViewModel>> GetDocumentByIdAsync(int id)
        {
            try
            {
                var document = await _documentRepository.GetByIdAsync(id);
                if (document == null)
                    return Result<DocumentViewModel>.Failure("Documento não encontrado.");

                return Result<DocumentViewModel>.Success(document.ToViewModel());
            }
            catch (Exception ex)
            {
                return Result<DocumentViewModel>.Failure($"Erro ao buscar documento: {ex.Message}");
            }
        }

        public async Task<Result<DocumentViewModel>> CreateDocumentAsync(DocumentCreateViewModel model)
        {
            try
            {
                if (await _documentRepository.ExistsByTitleAsync(model.Title))
                    return Result<DocumentViewModel>.Failure("Já existe um documento com este título.");

                var validationResult = ValidateFile(model.File);
                if (!validationResult.IsSuccess)
                    return Result<DocumentViewModel>.Failure(validationResult.Message);

                var fileName = await SaveFileAsync(model.File);

                var document = model.ToDomain();
                document.FileName = model.File.FileName;
                document.FilePath = fileName;
                document.FileSize = model.File.Length;
                document.ContentType = model.File.ContentType;
                document.CreatedAt = DateTime.Now;

                var createdDocument = await _documentRepository.CreateAsync(document);
                return Result<DocumentViewModel>.Success(createdDocument.ToViewModel());
            }
            catch (Exception ex)
            {
                return Result<DocumentViewModel>.Failure($"Erro ao criar documento: {ex.Message}");
            }
        }

        public async Task<Result<DocumentViewModel>> UpdateDocumentAsync(DocumentEditViewModel model)
        {
            try
            {
                var document = await _documentRepository.GetByIdAsync(model.Id);
                if (document == null)
                    return Result<DocumentViewModel>.Failure("Documento não encontrado.");

                if (await _documentRepository.ExistsByTitleAsync(model.Title, model.Id))
                    return Result<DocumentViewModel>.Failure("Já existe um documento com este título.");

                document.UpdateFromViewModel(model);

                if (model.File != null)
                {
                    var validationResult = ValidateFile(model.File);
                    if (!validationResult.IsSuccess)
                        return Result<DocumentViewModel>.Failure(validationResult.Message);

                    DeleteFile(document.FilePath);

                    var fileName = await SaveFileAsync(model.File);
                    document.FileName = model.File.FileName;
                    document.FilePath = fileName;
                    document.FileSize = model.File.Length;
                    document.ContentType = model.File.ContentType;
                }

                var updatedDocument = await _documentRepository.UpdateAsync(document);
                return Result<DocumentViewModel>.Success(updatedDocument.ToViewModel());
            }
            catch (Exception ex)
            {
                return Result<DocumentViewModel>.Failure($"Erro ao atualizar documento: {ex.Message}");
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

        private Result<bool> ValidateFile(IFormFile file)
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