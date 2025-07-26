using FileUploaderDocspider.Core.Domains.ViewModels;
using FileUploaderDocspider.Core.Shared;
using NetDevPack.SimpleMediator;

namespace FileUploaderDocspider.Application.Queries
{
    public sealed class DownloadDocumentQuery : IRequest<Result<DownloadDocumentResult>>
    {
        public int Id { get; set; }

        public DownloadDocumentQuery(int id)
        {
            Id = id;
        }
    }
}
