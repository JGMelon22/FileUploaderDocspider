using FileUploaderDocspider.Core.Domains.ViewModels;
using FileUploaderDocspider.Core.Shared;
using NetDevPack.SimpleMediator;
using System.Collections.Generic;

namespace FileUploaderDocspider.Application.Queries
{
    public sealed class GetDocumentByIdQuery : IRequest<Result<IEnumerable<DocumentViewModel>>>
    {
    }
}
