using FileUploaderDocspider.Models;
using FileUploaderDocspider.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileUploaderDocspider.Interfaces
{
    public interface IDocumentService
    {
        Task<IEnumerable<Document>> GetAllDocumentsAsync();
        Task<Document> GetDocumentByIdAsync(int id);
        Task<(bool Success, string Message)> CreateDocumentAsync(DocumentViewModel model);
        Task<(bool Success, string Message)> UpdateDocumentAsync(DocumentEditViewModel model);
        Task<(bool Success, string Message)> DeleteDocumentAsync(int id);
        bool ValidateFileAsync(IFormFile file);
    }
}
