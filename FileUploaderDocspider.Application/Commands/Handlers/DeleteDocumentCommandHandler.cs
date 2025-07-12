using FileUploaderDocspider.Core.Shared;
using FileUploaderDocspider.Infrastructure.Interfaces.Repositories;
using FileUploaderDocspider.Infrastructure.Interfaces.Services;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FileUploaderDocspider.Application.Commands.Handlers
{
    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, Result<bool>>
    {
        private readonly IDocumentService _documentService;
        private readonly IDocumentRepository _documentRepository;
        private readonly ILogger<DeleteDocumentCommandHandler> _logger;

        public DeleteDocumentCommandHandler(IDocumentService documentService, IDocumentRepository documentRepository,
            ILogger<DeleteDocumentCommandHandler> logger)
        {
            _documentService = documentService;
            _documentRepository = documentRepository;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting document: {DocumentId}", request.Id);

            try
            {
                var document = await _documentRepository.GetByIdAsync(request.Id);
                if (document == null)
                {
                    _logger.LogWarning("Document not found for deletion: {DocumentId}", request.Id);
                    return Result<bool>.Failure("Documento não encontrado.");
                }

                _documentService.DeleteFile(document.FilePath);
                _logger.LogInformation("File deleted for document: {DocumentId}, FilePath: {FilePath}", request.Id, document.FilePath);

                var deleted = await _documentRepository.DeleteAsync(request.Id);

                _logger.LogInformation("Document deleted successfully: {DocumentId} - {Title}", request.Id, document.Title);
                return Result<bool>.Success(deleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document: {DocumentId}", request.Id);
                return Result<bool>.Failure($"Erro ao excluir documento: {ex.Message}");
            }
        }
    }
}
