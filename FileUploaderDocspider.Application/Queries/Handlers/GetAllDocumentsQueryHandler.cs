using FileUploaderDocspider.Core.Domains.Mappings;
using FileUploaderDocspider.Core.Domains.ViewModels;
using FileUploaderDocspider.Core.Shared;
using FileUploaderDocspider.Infrastructure.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileUploaderDocspider.Application.Queries.Handlers
{
    public class GetAllDocumentsQueryHandler : IRequestHandler<GetAllDocumentsQuery, Result<IEnumerable<DocumentViewModel>>>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ILogger<GetAllDocumentsQueryHandler> _logger;

        public GetAllDocumentsQueryHandler(IDocumentRepository documentRepository, ILogger<GetAllDocumentsQueryHandler> logger)
        {
            _documentRepository = documentRepository;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<DocumentViewModel>>> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
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
    }
}
