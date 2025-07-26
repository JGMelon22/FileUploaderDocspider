using FileUploaderDocspider.Core.Domains.Mappings;
using FileUploaderDocspider.Core.Domains.ViewModels;
using FileUploaderDocspider.Core.Shared;
using FileUploaderDocspider.Infrastructure.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileUploaderDocspider.Application.Queries.Handlers
{
    public class DownloadDocumentQueryHandler : IRequestHandler<DownloadDocumentQuery, Result<DownloadDocumentResult>>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ILogger<DownloadDocumentQueryHandler> _logger;

        public DownloadDocumentQueryHandler(IDocumentRepository documentRepository, ILogger<DownloadDocumentQueryHandler> logger)
        {
            _documentRepository = documentRepository;
            _logger = logger;
        }

        public async Task<Result<DownloadDocumentResult>> Handle(DownloadDocumentQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving document by ID: {DocumentId}", request.Id);

            try
            {
                var document = await _documentRepository.GetByIdAsync(request.Id);
                if (document == null)
                {
                    _logger.LogWarning("Document not found with ID: {DocumentId}", request.Id);
                    return Result<DownloadDocumentResult>.Failure($"Document with Id {request.Id} not found!");
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", document.FilePath);
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("File not found for document: {DocumentId}", document.Id);
                    return Result<DownloadDocumentResult>.Failure("File not found.");
                }

                var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);

                _logger.LogInformation("Document retrieved successfully: {DocumentId} - {Title}", document.Id, document.Title);
                return Result<DownloadDocumentResult>.Success(document.ToDownloadDocumentResult(fileBytes));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document by ID: {DocumentId}", request.Id);
                return Result<DownloadDocumentResult>.Failure($"Erro ao buscar documento: {ex.Message}");
            }
        }
    }
}
