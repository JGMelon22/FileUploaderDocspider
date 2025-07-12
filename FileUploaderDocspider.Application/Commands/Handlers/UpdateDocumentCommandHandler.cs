using FileUploaderDocspider.Core.Domains.ViewModels;
using FileUploaderDocspider.Core.Shared;
using FileUploaderDocspider.Core.Domains.Mappings;
using FileUploaderDocspider.Infrastructure.Interfaces.Repositories;
using FileUploaderDocspider.Infrastructure.Interfaces.Services;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FileUploaderDocspider.Application.Commands.Handlers
{
    public class UpdateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand, Result<DocumentViewModel>>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentService _documentService;
        private readonly ILogger<UpdateDocumentCommandHandler> _logger;

        public UpdateDocumentCommandHandler(IDocumentRepository documentRepository, IDocumentService documentService,
            ILogger<UpdateDocumentCommandHandler> logger)
        {
            _documentRepository = documentRepository;
            _documentService = documentService;
            _logger = logger;
        }

        public async Task<Result<DocumentViewModel>> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating document: {DocumentId} - {Title}", request.Model.Id, request.Model.Title);

            try
            {
                var document = await _documentRepository.GetByIdAsync(request.Model.Id);
                if (document == null)
                {
                    _logger.LogWarning("Document not found for update: {DocumentId}", request.Model.Id);
                    return Result<DocumentViewModel>.Failure("Documento não encontrado.");
                }

                if (await _documentRepository.ExistsByTitleAsync(request.Model.Title, request.Model.Id))
                {
                    _logger.LogWarning("Document update failed - title already exists: {Title}, ExcludingId: {DocumentId}", request.Model.Title, request.Model.Id);
                    return Result<DocumentViewModel>.Failure("Já existe um documento com este título.");
                }

                document.UpdateFromViewModel(request.Model);

                if (request.Model.File != null)
                {
                    _logger.LogInformation("Updating file for document: {DocumentId}", request.Model.Id);

                    string validationError;
                    var isValid = _documentService.ValidateFile(request.Model.File, out validationError);
                    if (!isValid)
                    {
                        _logger.LogWarning("File validation failed for document: {Title}, Error: {Error}", request.Model.Title, validationError);
                        return Result<DocumentViewModel>.Failure(validationError);
                    }

                    _documentService.DeleteFile(document.FilePath);
                    _logger.LogInformation("Old file deleted for document: {DocumentId}", request.Model.Id);

                    var fileName = await _documentService.SaveFileAsync(request.Model.File);
                    document.FileName = request.Model.File.FileName;
                    document.FilePath = fileName;
                    document.FileSize = request.Model.File.Length;
                    document.ContentType = request.Model.File.ContentType;

                    _logger.LogInformation("New file saved for document: {DocumentId}, FileName: {FileName}", request.Model.Id, fileName);
                }

                var updatedDocument = await _documentRepository.UpdateAsync(document);

                _logger.LogInformation("Document updated successfully: {DocumentId} - {Title}", updatedDocument.Id, updatedDocument.Title);
                return Result<DocumentViewModel>.Success(updatedDocument.ToViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document: {DocumentId} - {Title}", request.Model.Id, request.Model.Title);
                return Result<DocumentViewModel>.Failure($"Erro ao atualizar documento: {ex.Message}");
            }
        }
    }
}
