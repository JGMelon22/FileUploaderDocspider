using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FileUploaderDocspider.Infrastructure.Interfaces.Services
{
    public interface IDocumentService
    {
        bool ValidateFile(IFormFile file, out string error);
        Task<string> SaveFileAsync(IFormFile file);
        void DeleteFile(string fileName);
    }
}
