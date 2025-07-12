using FileUploaderDocspider.Application.Commands;
using FileUploaderDocspider.Application.Commands.Handlers;
using FileUploaderDocspider.Core.Domains.Models;
using FileUploaderDocspider.Infrastructure.Interfaces.Repositories;
using FileUploaderDocspider.Infrastructure.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FileUploaderDocspider.Application.UnitTests.Commands
{
    public class DeleteDocumentCommandHandlerTets
    {
        [Fact]
        public async Task Should_ReturnSuccess_WhenDocumentIsDeletedSuccessfully()
        {
            // Arrange
            var service = new Mock<IDocumentService>();
            var repository = new Mock<IDocumentRepository>();
            var logger = new Mock<ILogger<DeleteDocumentCommandHandler>>();

            var document = new Document
            {
                Id = 1,
                Title = "Documento Teste",
                FilePath = "/uploads/teste.pdf"
            };

            repository.Setup(x => x.GetByIdAsync(document.Id)).ReturnsAsync(document);
            repository.Setup(x => x.DeleteAsync(document.Id)).ReturnsAsync(true);
            service.Setup(x => x.DeleteFile(document.FilePath));

            var handler = new DeleteDocumentCommandHandler(service.Object, repository.Object, logger.Object);
            var command = new DeleteDocumentCommand(document.Id);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Equal(string.Empty, result.Message);

            repository.Verify(x => x.GetByIdAsync(document.Id), Times.Once);
            service.Verify(x => x.DeleteFile(document.FilePath), Times.Once);
            repository.Verify(x => x.DeleteAsync(document.Id), Times.Once);
        }

        [Fact]
        public async Task Should_ReturnFailure_WhenDocumentNotFound()
        {
            // Arrange
            var service = new Mock<IDocumentService>();
            var repository = new Mock<IDocumentRepository>();
            var logger = new Mock<ILogger<DeleteDocumentCommandHandler>>();

            int documentId = 99;
            repository.Setup(x => x.GetByIdAsync(documentId)).ReturnsAsync((Document)null);

            var handler = new DeleteDocumentCommandHandler(service.Object, repository.Object, logger.Object);
            var command = new DeleteDocumentCommand(documentId);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.False(result.Data);
            Assert.Equal("Documento não encontrado.", result.Message);

            repository.Verify(x => x.GetByIdAsync(documentId), Times.Once);
            repository.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
            service.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Should_ReturnFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var service = new Mock<IDocumentService>();
            var repository = new Mock<IDocumentRepository>();
            var logger = new Mock<ILogger<DeleteDocumentCommandHandler>>();

            var document = new Document
            {
                Id = 2,
                Title = "Documento Teste",
                FilePath = "/uploads/teste.pdf"
            };

            repository.Setup(x => x.GetByIdAsync(document.Id)).ReturnsAsync(document);
            service.Setup(x => x.DeleteFile(document.FilePath));
            repository.Setup(x => x.DeleteAsync(document.Id)).ThrowsAsync(new Exception("Falha ao excluir"));

            var handler = new DeleteDocumentCommandHandler(service.Object, repository.Object, logger.Object);
            var command = new DeleteDocumentCommand(document.Id);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.False(result.Data);
            Assert.Contains("Erro ao excluir documento: Falha ao excluir", result.Message);

            repository.Verify(x => x.GetByIdAsync(document.Id), Times.Once);
            service.Verify(x => x.DeleteFile(document.FilePath), Times.Once);
            repository.Verify(x => x.DeleteAsync(document.Id), Times.Once);
        }
    }
}