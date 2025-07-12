using FileUploaderDocspider.Core.Domains.ViewModels;
using FileUploaderDocspider.Core.Shared;
using NetDevPack.SimpleMediator;

namespace FileUploaderDocspider.Application.Commands
{
    public sealed class CreateDocumentCommand : IRequest<Result<DocumentViewModel>>
    {
        public DocumentCreateViewModel Model { get; }

        public CreateDocumentCommand(DocumentCreateViewModel model)
        {
            Model = model;
        }
    }
}
