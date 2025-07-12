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
    public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, Result<DocumentViewModel>>
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentService _documentService;
        private readonly ILogger<CreateDocumentCommandHandler> _logger;

        public CreateDocumentCommandHandler(IDocumentRepository documentRepository,
            IDocumentService documentService, ILogger<CreateDocumentCommandHandler> logger)
        {
            _documentRepository = documentRepository;
            _documentService = documentService;
            _logger = logger;
        }

        public async Task<Result<DocumentViewModel>> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (await _documentRepository.ExistsByTitleAsync(request.Model.Title))
                {
                    _logger.LogWarning("Document creation failed - title already exists: {Title}", request.Model.Title);
                    return Result<DocumentViewModel>.Failure("Já existe um documento com este título.");
                }

                string validationError;
                var isValid = _documentService.ValidateFile(request.Model.File, out validationError);
                if (!isValid)
                {
                    _logger.LogWarning("File validation failed for document: {Title}, Error: {Error}", request.Model.Title, validationError);
                    return Result<DocumentViewModel>.Failure(validationError);
                }

                var fileName = await _documentService.SaveFileAsync(request.Model.File);

                var document = request.Model.ToDomain();
                document.FileName = request.Model.File.FileName;
                document.FilePath = fileName;
                document.FileSize = request.Model.File.Length;
                document.ContentType = request.Model.File.ContentType;
                document.CreatedAt = DateTime.Now;

                var createdDocument = await _documentRepository.CreateAsync(document);

                _logger.LogInformation("Document created successfully: {DocumentId} - {Title}, FileSize: {FileSize}",
                    createdDocument.Id, createdDocument.Title, createdDocument.FileSize);

                return Result<DocumentViewModel>.Success(createdDocument.ToViewModel());
            }
            catch (Exception ex)
            {
                return Result<DocumentViewModel>.Failure($"Erro ao criar documento: {ex.Message}");
            }
        }
    }
}
