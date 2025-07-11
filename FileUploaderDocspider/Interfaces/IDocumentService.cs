using FileUploaderDocspider.Dtos.Requests;
using FileUploaderDocspider.Dtos.Responses;
using FileUploaderDocspider.Shared;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileUploaderDocspider.Interfaces
{
    public interface IDocumentService
    {

        Task<Result<IEnumerable<DocumentResponse>>> GetAllDocumentsAsync();
        Task<Result<DocumentResponse>> GetDocumentByIdAsync(int id);
        Task<Result<DocumentResponse>> CreateDocumentAsync(DocumentRequest request, IFormFile file);
        Task<Result<DocumentResponse>> UpdateDocumentAsync(int id, DocumentRequest request, IFormFile file);
        Task<Result<bool>> DeleteDocumentAsync(int id);
        Result<bool> ValidateFileAsync(IFormFile file);
    }
}
