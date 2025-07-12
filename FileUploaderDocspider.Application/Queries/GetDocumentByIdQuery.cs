using FileUploaderDocspider.Core.Domains.ViewModels;
using FileUploaderDocspider.Core.Shared;
using NetDevPack.SimpleMediator;

namespace FileUploaderDocspider.Application.Queries
{
    public sealed class GetDocumentByIdQuery : IRequest<Result<DocumentViewModel>>
    {
        public int Id { get; set; }

        public GetDocumentByIdQuery(int id)
        {
            Id = id;
        }
    }
}
