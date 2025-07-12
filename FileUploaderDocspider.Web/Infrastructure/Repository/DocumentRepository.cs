using FileUploaderDocspider.Infrastructure.Data;
using FileUploaderDocspider.Interfaces;
using FileUploaderDocspider.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploaderDocspider.Infrastructure.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<DocumentRepository> _logger;

        public DocumentRepository(AppDbContext dbContext, ILogger<DocumentRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Document> CreateAsync(Document document)
        {
            _logger.LogInformation("Creating a new document: {Title}", document.Title);

            try
            {
                _dbContext.Documents.Add(document);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Document created successfully with ID: {DocumentId}", document.Id);
                return document;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Failed to create document: {Title}", document.Title);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting document with ID: {DocumentId}", id);

            try
            {
                var document = await _dbContext.Documents.FindAsync(id);
                if (document is null)
                {
                    _logger.LogWarning("Document not found for deletion. ID: {DocumentId}", id);
                    return false;
                }

                _dbContext.Documents.Remove(document);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Document with ID {DocumentId} deleted successfully", id);
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error deleting document with ID: {DocumentId}", id);
                throw;
            }
        }

        public async Task<bool> ExistsByTitleAsync(string title, int? excludeId = null)
        {
            _logger.LogInformation("Checking if document exists by title: {Title}, ExcludeId: {ExcludeId}", title, excludeId);

            try
            {
                IQueryable<Document> query = _dbContext.Documents.Where(d => d.Title
                    .ToLower() == title
                    .ToLower()
                );

                if (excludeId.HasValue)
                    query = query.Where(d => d.Id != excludeId.Value);

                var exists = await query.AnyAsync();
                _logger.LogInformation("Document exists check result: {Exists} for title: {Title}", exists, title);

                return exists;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error checking document existence by title: {Title}", title);
                throw;
            }
        }

        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all documents...");

            try
            {
                var documents = await _dbContext.Documents
                    .OrderByDescending(d => d.CreatedAt)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} documents", documents.Count);
                return documents;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all documents");
                throw;
            }
        }

        public async Task<Document> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching document with ID: {DocumentId}", id);

            try
            {
                var document = await _dbContext.Documents.FindAsync(id);

                if (document != null)
                    _logger.LogInformation("Document retrieved: {DocumentId} - {Title}", document.Id, document.Title);
                else
                    _logger.LogWarning("Document not found with ID: {DocumentId}", id);

                return document;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document with ID: {DocumentId}", id);
                throw;
            }
        }

        public async Task<Document> UpdateAsync(Document document)
        {
            _logger.LogInformation("Updating document with ID: {DocumentId}", document.Id);

            try
            {
                _dbContext.Documents.Update(document);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Document with ID {DocumentId} updated successfully", document.Id);
                return document;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error updating document with ID: {DocumentId}", document.Id);
                throw;
            }
        }
    }
}