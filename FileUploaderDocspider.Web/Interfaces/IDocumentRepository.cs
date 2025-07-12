using FileUploaderDocspider.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileUploaderDocspider.Interfaces
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<Document>> GetAllAsync();
        Task<Document> GetByIdAsync(int id);
        Task<Document> CreateAsync(Document document);
        Task<Document> UpdateAsync(Document document);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsByTitleAsync(string title, int? excludeId = null);
    }
}
