using FileUploaderDocspider.Shared;
using FileUploaderDocspider.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileUploaderDocspider.Interfaces
{
    public interface IDocumentService
    {
        Task<Result<IEnumerable<DocumentViewModel>>> GetAllDocumentsAsync();
        Task<Result<DocumentViewModel>> GetDocumentByIdAsync(int id);
        Task<Result<DocumentViewModel>> CreateDocumentAsync(DocumentCreateViewModel model);
        Task<Result<DocumentViewModel>> UpdateDocumentAsync(DocumentEditViewModel model);
        Task<Result<bool>> DeleteDocumentAsync(int id);
    }
}