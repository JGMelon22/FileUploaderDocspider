using FileUploaderDocspider.Models;
using FileUploaderDocspider.Shared;
using FileUploaderDocspider.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileUploaderDocspider.Interfaces
{
    public interface IDocumentService
    {

        Task<Result<IEnumerable<Document>>> GetAllDocumentsAsync();
        Task<Result<Document>> GetDocumentByIdAsync(int id);
        Task<Result<Document>> CreateDocumentAsync(DocumentViewModel model);
        Task<Result<Document>> UpdateDocumentAsync(DocumentEditViewModel model);
        Task<Result<bool>> DeleteDocumentAsync(int id);
        Result<bool> ValidateFileAsync(IFormFile file);
    }
}
