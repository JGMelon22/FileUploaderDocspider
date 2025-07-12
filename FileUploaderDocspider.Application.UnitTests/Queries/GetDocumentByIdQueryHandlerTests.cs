using FileUploaderDocspider.Application.Queries;
using FileUploaderDocspider.Application.Queries.Handlers;
using FileUploaderDocspider.Core.Domains.Models;
using FileUploaderDocspider.Infrastructure.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FileUploaderDocspider.Application.UnitTests.Queries
{
    public class GetDocumentByIdQueryHandlerTests
    {
        [Fact]
        public async Task Should_ReturnSingleDocument_When_ProvidedIdIsFound()
        {
            // Arrange
            var repository = new Mock<IDocumentRepository>();
            var logger = new Mock<ILogger<GetDocumentByIdQueryHandler>>();

            var document = new Document
            {
                Id = 1,
                Title = "Test Document",
                Description = "Description",
                FileName = "test.pdf",
                FilePath = "/uploads/test.pdf",
                CreatedAt = DateTime.Now,
                FileSize = 1024,
                ContentType = "application/pdf"
            };

            var query = new GetDocumentByIdQuery(1);

            repository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(document);

            var handler = new GetDocumentByIdQueryHandler(repository.Object, logger.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Id);
            Assert.Equal("Test Document", result.Data.Title);
            Assert.Equal("Description", result.Data.Description);
            Assert.Equal("test.pdf", result.Data.FileName);
            Assert.Equal("/uploads/test.pdf", result.Data.FilePath);
            Assert.Equal(1024, result.Data.FileSize);
            Assert.Equal("application/pdf", result.Data.ContentType);

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Message);

            repository.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task Should_ReturnFailureMessage_When_ProvidedIdIsNotFound()
        {
            // Arrange
            var repository = new Mock<IDocumentRepository>();
            var logger = new Mock<ILogger<GetDocumentByIdQueryHandler>>();

            var query = new GetDocumentByIdQuery(2);

            repository
                .Setup(x => x.GetByIdAsync(2))
                .ReturnsAsync((Document)null);

            var handler = new GetDocumentByIdQueryHandler(repository.Object, logger.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result.Data);
            Assert.False(result.IsSuccess);
            Assert.Equal("Document with Id 2 not found!", result.Message);

            repository.Verify(x => x.GetByIdAsync(2), Times.Once);
        }

        [Fact]
        public async Task Should_ReturnFailureResult_When_RepositoryThrowsException()
        {
            // Arrange
            var repository = new Mock<IDocumentRepository>();
            var logger = new Mock<ILogger<GetDocumentByIdQueryHandler>>();

            var exceptionMessage = "Database error";
            var query = new GetDocumentByIdQuery(3);

            repository
                .Setup(x => x.GetByIdAsync(3))
                .ThrowsAsync(new Exception(exceptionMessage));

            var handler = new GetDocumentByIdQueryHandler(repository.Object, logger.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result.Data);
            Assert.False(result.IsSuccess);
            Assert.Equal($"Erro ao buscar documento: {exceptionMessage}", result.Message);

            repository.Verify(x => x.GetByIdAsync(3), Times.Once);
        }
    }
}