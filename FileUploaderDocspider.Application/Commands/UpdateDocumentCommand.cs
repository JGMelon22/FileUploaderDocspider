using FileUploaderDocspider.Core.Domains.ViewModels;
using FileUploaderDocspider.Core.Shared;
using NetDevPack.SimpleMediator;

namespace FileUploaderDocspider.Application.Commands
{
    public sealed class UpdateDocumentCommand : IRequest<Result<DocumentViewModel>>
    {
        public int Id { get; }
        public DocumentEditViewModel Model { get; }

        public UpdateDocumentCommand(int id, DocumentEditViewModel model)
        {
            Id = id;
            Model = model;
        }
    }
}
