using FileUploaderDocspider.Application.Queries;
using FileUploaderDocspider.Application.Queries.Handlers;
using FileUploaderDocspider.Core.Domains.Models;
using FileUploaderDocspider.Infrastructure.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FileUploaderDocspider.Application.UnitTests.Queries
{
    public class GetAllDocumentsQueryHandlerTests
    {
        [Fact]
        public async Task Should_ReturnAllDocuments_When_DocumentsExistInRepository()
        {
            // Arrange
            var repository = new Mock<IDocumentRepository>();
            var logger = new Mock<ILogger<GetAllDocumentsQueryHandler>>();

            var documents = new List<Document>
            {
                new Document
                {
                    Id = 1,
                    Title = "Test Document 1",
                    Description = "Description 1",
                    FileName = "test1.pdf",
                    FilePath = "/uploads/test1.pdf",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    FileSize = 1024,
                    ContentType = "application/pdf"
                },
                new Document
                {
                    Id = 2,
                    Title = "Test Document 2",
                    Description = "Description 2",
                    FileName = "test2.docx",
                    FilePath = "/uploads/test2.docx",
                    CreatedAt = DateTime.Now,
                    FileSize = 2048,
                    ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                }
            };

            var query = new GetAllDocumentsQuery();

            repository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(documents);

            var handler = new GetAllDocumentsQueryHandler(repository.Object, logger.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count());
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Message);

            var documentsList = result.Data.ToList();

            Assert.Equal(1, documentsList[0].Id);
            Assert.Equal("Test Document 1", documentsList[0].Title);
            Assert.Equal("Description 1", documentsList[0].Description);
            Assert.Equal("test1.pdf", documentsList[0].FileName);
            Assert.Equal("/uploads/test1.pdf", documentsList[0].FilePath);
            Assert.Equal(1024, documentsList[0].FileSize);
            Assert.Equal("application/pdf", documentsList[0].ContentType);

            Assert.Equal(2, documentsList[1].Id);
            Assert.Equal("Test Document 2", documentsList[1].Title);
            Assert.Equal("Description 2", documentsList[1].Description);
            Assert.Equal("test2.docx", documentsList[1].FileName);
            Assert.Equal("/uploads/test2.docx", documentsList[1].FilePath);
            Assert.Equal(2048, documentsList[1].FileSize);
            Assert.Equal("application/vnd.openxmlformats-officedocument.wordprocessingml.document", documentsList[1].ContentType);

            repository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Should_ReturnEmptyCollection_When_NoDocumentsExistInRepository()
        {
            // Arrange
            var repository = new Mock<IDocumentRepository>();
            var logger = new Mock<ILogger<GetAllDocumentsQueryHandler>>();

            var emptyDocuments = new List<Document>();

            var query = new GetAllDocumentsQuery();

            repository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(emptyDocuments);

            var handler = new GetAllDocumentsQueryHandler(repository.Object, logger.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Message);

            repository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Should_ReturnFailureResult_When_RepositoryThrowsException()
        {
            // Arrange
            var repository = new Mock<IDocumentRepository>();
            var logger = new Mock<ILogger<GetAllDocumentsQueryHandler>>();

            var exceptionMessage = "Database connection failed";
            var query = new GetAllDocumentsQuery();

            repository
                .Setup(x => x.GetAllAsync())
                .ThrowsAsync(new Exception(exceptionMessage));

            var handler = new GetAllDocumentsQueryHandler(repository.Object, logger.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result.Data);
            Assert.False(result.IsSuccess);
            Assert.Equal($"Erro ao buscar documentos: {exceptionMessage}", result.Message);

            repository.Verify(x => x.GetAllAsync(), Times.Once);
        }
    }
}