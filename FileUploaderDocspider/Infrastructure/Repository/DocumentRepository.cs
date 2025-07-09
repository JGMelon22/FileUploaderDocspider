using FileUploaderDocspider.Infrastructure.Data;
using FileUploaderDocspider.Interfaces;
using FileUploaderDocspider.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploaderDocspider.Infrastructure.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _dbContext;
        public async Task<Document> CreateAsync(Document document)
        {
            _dbContext.Documents.Add(document);
            await _dbContext.SaveChangesAsync();
            return document;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var document = await _dbContext.Documents.FindAsync(id);
            if (document is null)
                return false;

            _dbContext.Documents.Remove(document);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByTitleAsync(string title, int? excludeId = null)
        {
            IQueryable<Document> query = _dbContext.Documents.Where(d => d.Title
                .ToLower() == title
                .ToLower()
            );

            if (excludeId.HasValue)
                query = query.Where(d => d.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            return await _dbContext.Documents
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<Document> GetByIdAsync(int id)
        {
            return await _dbContext.Documents.FindAsync(id);
        }

        public async Task<Document> UpdateAsync(Document document)
        {
            _dbContext.Documents.Update(document);
            await _dbContext.SaveChangesAsync();
            return document;
        }
    }
}
