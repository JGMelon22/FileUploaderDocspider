using FileUploaderDocspider.Application.Commands;
using FileUploaderDocspider.Application.Commands.Handlers;
using FileUploaderDocspider.Core.Domains.Models;
using FileUploaderDocspider.Core.Domains.ViewModels;
using FileUploaderDocspider.Infrastructure.Interfaces.Repositories;
using FileUploaderDocspider.Infrastructure.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FileUploaderDocspider.Application.UnitTests.Commands
{
    public class UpdateDocumentCommandHandlerTests
    {
        [Fact]
        public async Task Should_ReturnSuccess_WhenDocumentIsUpdatedSuccessfully_WithoutFile()
        {
            // Arrange
            var repository = new Mock<IDocumentRepository>();
            var service = new Mock<IDocumentService>();
            var logger = new Mock<ILogger<UpdateDocumentCommandHandler>>();

            var editViewModel = new DocumentEditViewModel
            {
                Id = 1,
                Title = "Novo Título",
                Description = "Nova descrição",
                FileName = "novo_arquivo.pdf"
            };

            var existingDocument = new Document
            {
                Id = 1,
                Title = "Título Antigo",
                Description = "Descrição antiga",
                FileName = "arquivo_antigo.pdf",
                FilePath = "/uploads/arquivo_antigo.pdf",
                CreatedAt = DateTime.Now,
                FileSize = 1024,
                ContentType = "application/pdf"
            };

            var updatedDocument = new Document
            {
                Id = 1,
                Title = editViewModel.Title,
                Description = editViewModel.Description,
                FileName = editViewModel.FileName,
                FilePath = existingDocument.FilePath,
                CreatedAt = existingDocument.CreatedAt,
                FileSize = existingDocument.FileSize,
                ContentType = existingDocument.ContentType
            };

            repository.Setup(x => x.GetByIdAsync(editViewModel.Id)).ReturnsAsync(existingDocument);
            repository.Setup(x => x.ExistsByTitleAsync(editViewModel.Title, editViewModel.Id)).ReturnsAsync(false);
            repository.Setup(x => x.UpdateAsync(It.IsAny<Document>())).ReturnsAsync(updatedDocument);

            var handler = new UpdateDocumentCommandHandler(repository.Object, service.Object, logger.Object);
            var command = new UpdateDocumentCommand(editViewModel.Id, editViewModel);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(editViewModel.Title, result.Data.Title);
            Assert.Equal(editViewModel.Description, result.Data.Description);
            Assert.Equal(editViewModel.FileName, result.Data.FileName);
            Assert.Equal(string.Empty, result.Message);

            repository.Verify(x => x.GetByIdAsync(editViewModel.Id), Times.Once);
            repository.Verify(x => x.ExistsByTitleAsync(editViewModel.Title, editViewModel.Id), Times.Once);
            repository.Verify(x => x.UpdateAsync(It.IsAny<Document>()), Times.Once);
        }

        [Fact]
        public async Task Should_ReturnSuccess_WhenDocumentIsUpdatedSuccessfully_WithFile()
        {
            // Arrange
            var repository = new Mock<IDocumentRepository>();
            var service = new Mock<IDocumentService>();
            var logger = new Mock<ILogger<UpdateDocumentCommandHandler>>();

            byte[] filebytes = Encoding.UTF8.GetBytes("dummy pdf");
            IFormFile file = new FormFile(new MemoryStream(filebytes), 0, filebytes.Length, "Data", "documento.pdf")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            var editViewModel = new DocumentEditViewModel
            {
                Id = 2,
                Title = "Título com arquivo",
                Description = "Descrição",
                FileName = "documento.pdf",
                File = file
            };

            var existingDocument = new Document
            {
                Id = 2,
                Title = "Título antigo",
                Description = "Descrição antiga",
                FileName = "arquivo_antigo.pdf",
                FilePath = "/uploads/arquivo_antigo.pdf",
                CreatedAt = DateTime.Now,
                FileSize = 1024,
                ContentType = "application/pdf"
            };

            var updatedDocument = new Document
            {
                Id = 2,
                Title = editViewModel.Title,
                Description = editViewModel.Description,
                FileName = editViewModel.Title,
                FilePath = "/uploads/documento.pdf",
                CreatedAt = existingDocument.CreatedAt,
                FileSize = file.Length,
                ContentType = file.ContentType
            };

            repository.Setup(x => x.GetByIdAsync(editViewModel.Id)).ReturnsAsync(existingDocument);
            repository.Setup(x => x.ExistsByTitleAsync(editViewModel.Title, editViewModel.Id)).ReturnsAsync(false);
            service.Setup(x => x.ValidateFile(editViewModel.File, out It.Ref<string>.IsAny)).Returns(true);
            service.Setup(x => x.SaveFileAsync(editViewModel.File)).ReturnsAsync(updatedDocument.FilePath);
            repository.Setup(x => x.UpdateAsync(It.IsAny<Document>())).ReturnsAsync(updatedDocument);

            var handler = new UpdateDocumentCommandHandler(repository.Object, service.Object, logger.Object);
            var command = new UpdateDocumentCommand(editViewModel.Id, editViewModel);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(editViewModel.Title, result.Data.Title);
            Assert.Equal(editViewModel.Description, result.Data.Description);
            Assert.Equal(editViewModel.Title, result.Data.FileName);
            Assert.Equal(updatedDocument.FilePath, result.Data.FilePath);
            Assert.Equal(file.Length, result.Data.FileSize);
            Assert.Equal(file.ContentType, result.Data.ContentType);
            Assert.Equal(string.Empty, result.Message);

            repository.Verify(x => x.GetByIdAsync(editViewModel.Id), Times.Once);
            repository.Verify(x => x.ExistsByTitleAsync(editViewModel.Title, editViewModel.Id), Times.Once);
            service.Verify(x => x.ValidateFile(editViewModel.File, out It.Ref<string>.IsAny), Times.Once);
            service.Verify(x => x.SaveFileAsync(editViewModel.File), Times.Once);
            repository.Verify(x => x.UpdateAsync(It.IsAny<Document>()), Times.Once);
        }

        [Fact]
        public async Task Should_ReturnFailure_WhenDocumentNotFound()
        {
            // Arrange
            var repository = new Mock<IDocumentRepository>();
            var service = new Mock<IDocumentService>();
            var logger = new Mock<ILogger<UpdateDocumentCommandHandler>>();

            var editViewModel = new DocumentEditViewModel
            {
                Id = 99,
                Title = "Título",
                Description = "Descrição",
                FileName = "arquivo.pdf"
            };

            repository.Setup(x => x.GetByIdAsync(editViewModel.Id)).ReturnsAsync((Document)null);

            var handler = new UpdateDocumentCommandHandler(repository.Object, service.Object, logger.Object);
            var command = new UpdateDocumentCommand(editViewModel.Id, editViewModel);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.Equal("Documento não encontrado.", result.Message);

            repository.Verify(x => x.GetByIdAsync(editViewModel.Id), Times.Once);
        }

        [Fact]
        public async Task Should_ReturnFailure_WhenTitleAlreadyExists()
        {
            // Arrange
            var repository = new Mock<IDocumentRepository>();
            var service = new Mock<IDocumentService>();
            var logger = new Mock<ILogger<UpdateDocumentCommandHandler>>();

            var editViewModel = new DocumentEditViewModel
            {
                Id = 1,
                Title = "Título duplicado",
                Description = "Descrição",
                FileName = "arquivo.pdf"
            };

            var existingDocument = new Document
            {
                Id = 1,
                Title = "Título antigo",
                Description = "Descrição antiga",
                FileName = "arquivo_antigo.pdf",
                FilePath = "/uploads/arquivo_antigo.pdf",
                CreatedAt = DateTime.Now,
                FileSize = 1024,
                ContentType = "application/pdf"
            };

            repository.Setup(x => x.GetByIdAsync(editViewModel.Id)).ReturnsAsync(existingDocument);
            repository.Setup(x => x.ExistsByTitleAsync(editViewModel.Title, editViewModel.Id)).ReturnsAsync(true);

            var handler = new UpdateDocumentCommandHandler(repository.Object, service.Object, logger.Object);
            var command = new UpdateDocumentCommand(editViewModel.Id, editViewModel);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.Equal("Já existe um documento com este título.", result.Message);

            repository.Verify(x => x.GetByIdAsync(editViewModel.Id), Times.Once);
            repository.Verify(x => x.ExistsByTitleAsync(editViewModel.Title, editViewModel.Id), Times.Once);
        }

        [Fact]
        public async Task Should_ReturnFailure_WhenFileValidationFails()
        {
            // Arrange
            var repository = new Mock<IDocumentRepository>();
            var service = new Mock<IDocumentService>();
            var logger = new Mock<ILogger<UpdateDocumentCommandHandler>>();

            byte[] filebytes = Encoding.UTF8.GetBytes("dummy pdf");
            IFormFile file = new FormFile(new MemoryStream(filebytes), 0, filebytes.Length, "Data", "documento.pdf")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            var editViewModel = new DocumentEditViewModel
            {
                Id = 1,
                Title = "Título",
                Description = "Descrição",
                FileName = "documento.pdf",
                File = file
            };

            var existingDocument = new Document
            {
                Id = 1,
                Title = "Título antigo",
                Description = "Descrição antiga",
                FileName = "arquivo_antigo.pdf",
                FilePath = "/uploads/arquivo_antigo.pdf",
                CreatedAt = DateTime.Now,
                FileSize = 1024,
                ContentType = "application/pdf"
            };

            repository.Setup(x => x.GetByIdAsync(editViewModel.Id)).ReturnsAsync(existingDocument);
            repository.Setup(x => x.ExistsByTitleAsync(editViewModel.Title, editViewModel.Id)).ReturnsAsync(false);

            string validationError = "Arquivo inválido";
            service.Setup(x => x.ValidateFile(editViewModel.File, out validationError)).Returns(false);

            var handler = new UpdateDocumentCommandHandler(repository.Object, service.Object, logger.Object);
            var command = new UpdateDocumentCommand(editViewModel.Id, editViewModel);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.Equal(validationError, result.Message);

            repository.Verify(x => x.GetByIdAsync(editViewModel.Id), Times.Once);
            repository.Verify(x => x.ExistsByTitleAsync(editViewModel.Title, editViewModel.Id), Times.Once);
            service.Verify(x => x.ValidateFile(editViewModel.File, out validationError), Times.Once);
        }

        [Fact]
        public async Task Should_ReturnFailure_WhenRepositoryThrowsException()
        {
            // Arrange
            var repository = new Mock<IDocumentRepository>();
            var service = new Mock<IDocumentService>();
            var logger = new Mock<ILogger<UpdateDocumentCommandHandler>>();

            var editViewModel = new DocumentEditViewModel
            {
                Id = 1,
                Title = "Título",
                Description = "Descrição",
                FileName = "arquivo.pdf"
            };

            var existingDocument = new Document
            {
                Id = 1,
                Title = "Título antigo",
                Description = "Descrição antiga",
                FileName = "arquivo_antigo.pdf",
                FilePath = "/uploads/arquivo_antigo.pdf",
                CreatedAt = DateTime.Now,
                FileSize = 1024,
                ContentType = "application/pdf"
            };

            repository.Setup(x => x.GetByIdAsync(editViewModel.Id)).ReturnsAsync(existingDocument);
            repository.Setup(x => x.ExistsByTitleAsync(editViewModel.Title, editViewModel.Id)).ReturnsAsync(false);
            repository.Setup(x => x.UpdateAsync(It.IsAny<Document>())).ThrowsAsync(new Exception("Falha no banco"));

            var handler = new UpdateDocumentCommandHandler(repository.Object, service.Object, logger.Object);
            var command = new UpdateDocumentCommand(editViewModel.Id, editViewModel);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.Contains("Erro ao atualizar documento: Falha no banco", result.Message);

            repository.Verify(x => x.GetByIdAsync(editViewModel.Id), Times.Once);
            repository.Verify(x => x.ExistsByTitleAsync(editViewModel.Title, editViewModel.Id), Times.Once);
            repository.Verify(x => x.UpdateAsync(It.IsAny<Document>()), Times.Once);
        }
    }
}