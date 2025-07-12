using FileUploaderDocspider.Interfaces;
using FileUploaderDocspider.Mappings;
using FileUploaderDocspider.Shared;
using FileUploaderDocspider.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<DocumentService> _logger;
        private readonly string[] _blockedExtensions = { ".exe", ".zip", ".bat" };

        public DocumentService(IDocumentRepository documentRepository, IWebHostEnvironment webHostEnvironment, ILogger<DocumentService> logger)
        {
            _documentRepository = documentRepository;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<DocumentViewModel>>> GetAllDocumentsAsync()
        {
            _logger.LogInformation("Starting retrieval of all documents");

            try
            {
                var documents = await _documentRepository.GetAllAsync();
                var viewModels = documents.ToViewModel();

                _logger.LogInformation("Successfully retrieved {Count} documents", viewModels.Count());
                return Result<IEnumerable<DocumentViewModel>>.Success(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all documents");
                return Result<IEnumerable<DocumentViewModel>>.Failure($"Erro ao buscar documentos: {ex.Message}");
            }
        }

        public async Task<Result<DocumentViewModel>> GetDocumentByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving document by ID: {DocumentId}", id);

            try
            {
                var document = await _documentRepository.GetByIdAsync(id);
                if (document == null)
                {
                    _logger.LogWarning("Document not found with ID: {DocumentId}", id);
                    return Result<DocumentViewModel>.Failure("Documento não encontrado.");
                }

                _logger.LogInformation("Successfully retrieved document: {DocumentId} - {Title}", document.Id, document.Title);
                return Result<DocumentViewModel>.Success(document.ToViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document by ID: {DocumentId}", id);
                return Result<DocumentViewModel>.Failure($"Erro ao buscar documento: {ex.Message}");
            }
        }

        public async Task<Result<DocumentViewModel>> CreateDocumentAsync(DocumentCreateViewModel model)
        {
            _logger.LogInformation("Creating new document: {Title}", model.Title);

            try
            {
                if (await _documentRepository.ExistsByTitleAsync(model.Title))
                {
                    _logger.LogWarning("Document creation failed - title already exists: {Title}", model.Title);
                    return Result<DocumentViewModel>.Failure("Já existe um documento com este título.");
                }

                var validationResult = ValidateFile(model.File);
                if (!validationResult.IsSuccess)
                {
                    _logger.LogWarning("File validation failed for document: {Title}, Error: {Error}", model.Title, validationResult.Message);
                    return Result<DocumentViewModel>.Failure(validationResult.Message);
                }

                var fileName = await SaveFileAsync(model.File);
                _logger.LogInformation("File saved successfully: {FileName} for document: {Title}", fileName, model.Title);

                var document = model.ToDomain();
                document.FileName = model.File.FileName;
                document.FilePath = fileName;
                document.FileSize = model.File.Length;
                document.ContentType = model.File.ContentType;
                document.CreatedAt = DateTime.Now;

                var createdDocument = await _documentRepository.CreateAsync(document);

                _logger.LogInformation("Document created successfully: {DocumentId} - {Title}, FileSize: {FileSize}",
                    createdDocument.Id, createdDocument.Title, createdDocument.FileSize);

                return Result<DocumentViewModel>.Success(createdDocument.ToViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document: {Title}", model.Title);
                return Result<DocumentViewModel>.Failure($"Erro ao criar documento: {ex.Message}");
            }
        }

        public async Task<Result<DocumentViewModel>> UpdateDocumentAsync(DocumentEditViewModel model)
        {
            _logger.LogInformation("Updating document: {DocumentId} - {Title}", model.Id, model.Title);

            try
            {
                var document = await _documentRepository.GetByIdAsync(model.Id);
                if (document == null)
                {
                    _logger.LogWarning("Document not found for update: {DocumentId}", model.Id);
                    return Result<DocumentViewModel>.Failure("Documento não encontrado.");
                }

                if (await _documentRepository.ExistsByTitleAsync(model.Title, model.Id))
                {
                    _logger.LogWarning("Document update failed - title already exists: {Title}, ExcludingId: {DocumentId}", model.Title, model.Id);
                    return Result<DocumentViewModel>.Failure("Já existe um documento com este título.");
                }

                document.UpdateFromViewModel(model);

                if (model.File != null)
                {
                    _logger.LogInformation("Updating file for document: {DocumentId}", model.Id);

                    var validationResult = ValidateFile(model.File);
                    if (!validationResult.IsSuccess)
                    {
                        _logger.LogWarning("File validation failed during update for document: {DocumentId}, Error: {Error}", model.Id, validationResult.Message);
                        return Result<DocumentViewModel>.Failure(validationResult.Message);
                    }

                    DeleteFile(document.FilePath);
                    _logger.LogInformation("Old file deleted for document: {DocumentId}", model.Id);

                    var fileName = await SaveFileAsync(model.File);
                    document.FileName = model.File.FileName;
                    document.FilePath = fileName;
                    document.FileSize = model.File.Length;
                    document.ContentType = model.File.ContentType;

                    _logger.LogInformation("New file saved for document: {DocumentId}, FileName: {FileName}", model.Id, fileName);
                }

                var updatedDocument = await _documentRepository.UpdateAsync(document);

                _logger.LogInformation("Document updated successfully: {DocumentId} - {Title}", updatedDocument.Id, updatedDocument.Title);
                return Result<DocumentViewModel>.Success(updatedDocument.ToViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document: {DocumentId} - {Title}", model.Id, model.Title);
                return Result<DocumentViewModel>.Failure($"Erro ao atualizar documento: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteDocumentAsync(int id)
        {
            _logger.LogInformation("Deleting document: {DocumentId}", id);

            try
            {
                var document = await _documentRepository.GetByIdAsync(id);
                if (document == null)
                {
                    _logger.LogWarning("Document not found for deletion: {DocumentId}", id);
                    return Result<bool>.Failure("Documento não encontrado.");
                }

                DeleteFile(document.FilePath);
                _logger.LogInformation("File deleted for document: {DocumentId}, FilePath: {FilePath}", id, document.FilePath);

                var deleted = await _documentRepository.DeleteAsync(id);

                _logger.LogInformation("Document deleted successfully: {DocumentId} - {Title}", id, document.Title);
                return Result<bool>.Success(deleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document: {DocumentId}", id);
                return Result<bool>.Failure($"Erro ao excluir documento: {ex.Message}");
            }
        }

        private Result<bool> ValidateFile(IFormFile file)
        {
            _logger.LogInformation("Validating file: {FileName}, Size: {FileSize}", file?.FileName, file?.Length);

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("File validation failed - file is null or empty");
                return Result<bool>.Failure("Arquivo é obrigatório.");
            }

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (_blockedExtensions.Contains(extension))
            {
                _logger.LogWarning("File validation failed - blocked extension: {Extension}, FileName: {FileName}", extension, file.FileName);
                return Result<bool>.Failure("Tipo de arquivo não permitido. Arquivos .exe, .zip e .bat não são aceitos.");
            }

            _logger.LogInformation("File validation successful: {FileName}", file.FileName);
            return Result<bool>.Success(true);
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            _logger.LogInformation("Saving file: {FileName}, Size: {FileSize}", file.FileName, file.Length);

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            _logger.LogInformation("File saved successfully: {GeneratedFileName} from original: {OriginalFileName}", fileName, file.FileName);
            return fileName;
        }

        private void DeleteFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                _logger.LogWarning("Attempted to delete file with null or empty filename");
                return;
            }

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("File deleted successfully: {FileName}", fileName);
            }
            else
            {
                _logger.LogWarning("File not found for deletion: {FileName}", fileName);
            }
        }
    }
}