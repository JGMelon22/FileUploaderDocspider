using FileUploaderDocspider.Core.Domains.Models;
using FileUploaderDocspider.Infrastructure.Data;
using FileUploaderDocspider.Infrastructure.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploaderDocspider.Infrastructure.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _dbContext;

        public DocumentRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Document> CreateAsync(Document document)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                _dbContext.Documents.Add(document);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return document;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var document = await _dbContext.Documents.FindAsync(id);
                if (document is null)
                {
                    return false;
                }

                _dbContext.Documents.Remove(document);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                _dbContext.Documents.Update(document);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return document;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}