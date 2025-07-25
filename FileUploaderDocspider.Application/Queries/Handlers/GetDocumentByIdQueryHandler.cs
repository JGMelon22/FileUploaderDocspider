﻿using FileUploaderDocspider.Core.Domains.Mappings;
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
            _logger.LogInformation("Retrieving document by ID: {DocumentId}", request.Id);

            try
            {
                var document = await _documentRepository.GetByIdAsync(request.Id);
                if (document == null)
                {
                    _logger.LogWarning("Document not found with ID: {DocumentId}", request.Id);
                    return Result<DocumentViewModel>.Failure($"Document with Id {request.Id} not found!");
                }

                _logger.LogInformation("Document retrieved successfully: {DocumentId} - {Title}", document.Id, document.Title);
                return Result<DocumentViewModel>.Success(document.ToViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document by ID: {DocumentId}", request.Id);
                return Result<DocumentViewModel>.Failure($"Erro ao buscar documento: {ex.Message}");
            }
        }
    }
}
