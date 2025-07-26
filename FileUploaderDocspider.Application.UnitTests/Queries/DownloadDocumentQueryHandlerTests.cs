using FileUploaderDocspider.Application.Queries;
using FileUploaderDocspider.Application.Queries.Handlers;
using FileUploaderDocspider.Core.Domains.Models;
using FileUploaderDocspider.Infrastructure.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FileUploaderDocspider.Application.UnitTests.Queries
{
    public class DownloadDocumentQueryHandlerTests
    {
        [Fact]
        public async Task Should_ReturnSuccess_When_DocumentAndFileExist()
        {
            // Arrange
            var documentId = 1;
            var filePath = "testfile.txt";
            var fileBytes = new byte[] { 1, 2, 3 };
            var document = new Document
            {
                Id = documentId,
                Title = "Test",
                FilePath = filePath,
                FileName = "testfile.txt",
                ContentType = "text/plain"
            };

            var documentRepoMock = new Mock<IDocumentRepository>();
            documentRepoMock.Setup(r => r.GetByIdAsync(documentId)).ReturnsAsync(document);

            var loggerMock = new Mock<ILogger<DownloadDocumentQueryHandler>>();

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", filePath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            await File.WriteAllBytesAsync(fullPath, fileBytes);

            var handler = new DownloadDocumentQueryHandler(documentRepoMock.Object, loggerMock.Object);
            var query = new DownloadDocumentQuery(documentId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(fileBytes, result.Data.FileBytes);
            Assert.Equal(document.FileName, result.Data.FileName);
            Assert.Equal(document.ContentType, result.Data.ContentType);

            // Cleanup
            File.Delete(fullPath);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_DocumentNotFound()
        {
            // Arrange
            var documentId = 2;
            var documentRepoMock = new Mock<IDocumentRepository>();
            documentRepoMock.Setup(r => r.GetByIdAsync(documentId)).ReturnsAsync((Document)null);

            var loggerMock = new Mock<ILogger<DownloadDocumentQueryHandler>>();
            var handler = new DownloadDocumentQueryHandler(documentRepoMock.Object, loggerMock.Object);
            var query = new DownloadDocumentQuery(documentId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.Contains("not found", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_FileNotFound()
        {
            // Arrange
            var documentId = 3;
            var filePath = "nonexistent.txt";
            var document = new Document
            {
                Id = documentId,
                Title = "Test",
                FilePath = filePath,
                FileName = "nonexistent.txt",
                ContentType = "text/plain"
            };

            var documentRepoMock = new Mock<IDocumentRepository>();
            documentRepoMock.Setup(r => r.GetByIdAsync(documentId)).ReturnsAsync(document);

            var loggerMock = new Mock<ILogger<DownloadDocumentQueryHandler>>();
            var handler = new DownloadDocumentQueryHandler(documentRepoMock.Object, loggerMock.Object);
            var query = new DownloadDocumentQuery(documentId);

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", filePath);
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.Contains("file not found", result.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
