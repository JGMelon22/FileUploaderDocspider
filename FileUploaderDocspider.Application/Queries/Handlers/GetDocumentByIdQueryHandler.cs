using FileUploaderDocspider.Core.Domains.Mappings;
using FileUploaderDocspider.Core.Domains.ViewModels;
using FileUploaderDocspider.Core.Shared;
using FileUploaderDocspider.Infrastructure.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using NetDevPack.SimpleMediator;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FileUploaderDocspider.Application.Queries.Handlers
{
    public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, Result<DocumentViewModel>>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ILogger<GetDocumentByIdQueryHandler> _logger;

        public GetDocumentByIdQueryHandler(IDocumentRepository documentRepository, ILogger<GetDocumentByIdQueryHandler> logger)
        {
            _documentRepository = documentRepository;
            _logger = logger;
        }

        public async Task<Result<DocumentViewModel>> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var document = await _documentRepository.GetByIdAsync(request.Id);
                if (document == null)
                {
                    _logger.LogWarning("Document not found with ID: {DocumentId}", request.Id);
                }

                return Result<DocumentViewModel>.Success(document.ToViewModel());
            }
            catch (Exception ex)
            {
                return Result<DocumentViewModel>.Failure($"Erro ao buscar documento: {ex.Message}");
            }
        }
    }
}
