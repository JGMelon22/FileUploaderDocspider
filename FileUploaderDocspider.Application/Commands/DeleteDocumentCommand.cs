using FileUploaderDocspider.Core.Shared;
using NetDevPack.SimpleMediator;

namespace FileUploaderDocspider.Application.Commands
{
    public sealed class DeleteDocumentCommand : IRequest<Result<bool>>
    {
        public int Id { get; }

        public DeleteDocumentCommand(int id)
        {
            Id = id;
        }
    }
}