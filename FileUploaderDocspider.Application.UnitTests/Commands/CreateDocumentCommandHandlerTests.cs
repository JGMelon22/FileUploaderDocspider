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
    public class CreateDocumentCommandHandlerTests
    {
        [Fact]
        public async Task Should_ReturnDocumentViewModel_WhenSuccessfullyCreateDocument()
        {
            // Arrange
            var repository = new Mock<IDocumentRepository>();
            var service = new Mock<IDocumentService>();
            var logger = new Mock<ILogger<CreateDocumentCommandHandler>>();

            byte[] filebytes = Encoding.UTF8.GetBytes("dummy image");
            IFormFile file = new FormFile(new MemoryStream(filebytes), 0, filebytes.Length, "Data", "image.png")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };

            var documentCreateViewModel = new DocumentCreateViewModel
            {
                Title = "Contrato de Prestação de Serviços",
                Description = "Documento referente ao contrato firmado com o cliente XPTO Ltda.",
                FileName = "contrato_prestacao_servicos.pdf",
                File = file
            };

            var documentModel = new DocumentViewModel
            {
                Id = 1,
                Title = "Novo Documento",
                Description = "Descrição",
                FileName = "arquivo.pdf",
                FilePath = "/uploads/arquivo.pdf",
                CreatedAt = DateTime.Now,
                FileSize = 2048,
                ContentType = "application/pdf"
            };

            var command = new CreateDocumentCommand(documentCreateViewModel);

            repository
                .Setup(x => x.CreateAsync(It.IsAny<Document>()))
                .ReturnsAsync(new Document
                {
                    Id = documentModel.Id,
                    Title = documentModel.Title,
                    Description = documentModel.Description,
                    FileName = documentModel.FileName,
                    FilePath = documentModel.FilePath,
                    CreatedAt = documentModel.CreatedAt,
                    FileSize = documentModel.FileSize,
                    ContentType = documentModel.ContentType
                });

            service
                .Setup(x => x.ValidateFile(It.IsAny<IFormFile>(), out It.Ref<string>.IsAny))
                .Returns(true);

            service
                .Setup(x => x.SaveFileAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(documentModel.FileName);

            var handler = new CreateDocumentCommandHandler(repository.Object, service.Object, logger.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal(documentModel.Id, result.Data.Id);
            Assert.Equal(documentModel.Title, result.Data.Title);
            Assert.Equal(documentModel.Description, result.Data.Description);
            Assert.Equal(documentModel.FileName, result.Data.FileName);
            Assert.Equal(documentModel.FilePath, result.Data.FilePath);
            Assert.Equal(documentModel.CreatedAt, result.Data.CreatedAt);
            Assert.Equal(documentModel.FileSize, result.Data.FileSize);
            Assert.Equal(documentModel.ContentType, result.Data.ContentType);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Message);

            repository.Verify(x => x.CreateAsync(It.IsAny<Document>()), Times.Once);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_TitleAlreadyExists()
        {
            // Arrange
            var repository = new Mock<IDocumentRepository>();
            var service = new Mock<IDocumentService>();
            var logger = new Mock<ILogger<CreateDocumentCommandHandler>>();

            byte[] filebytes = Encoding.UTF8.GetBytes("dummy image");
            IFormFile file = new FormFile(new MemoryStream(filebytes), 0, filebytes.Length, "Data", "image.png");

            var documentCreateViewModel = new DocumentCreateViewModel
            {
                Title = "Contrato de Prestação de Serviços",
                Description = "Documento referente ao contrato firmado com o cliente XPTO Ltda.",
                FileName = "contrato_prestacao_servicos.pdf",
                File = file
            };

            var documentModel = new DocumentViewModel
            {
                Id = 1,
                Title = "Contrato de Prestação de Serviços",
                Description = "Documento referente ao contrato firmado com o cliente XPTO Ltda.",
                FileName = "contrato_prestacao_servicos.pdf",
                FilePath = "/uploads/2025/07/contrato_prestacao_servicos.pdf",
                CreatedAt = DateTime.Now,
                FileSize = 256034,
                ContentType = "application/pdf"
            };

            var command = new CreateDocumentCommand(documentCreateViewModel);

            repository
                .Setup(x => x.ExistsByTitleAsync(documentModel.Title, null))
                .ReturnsAsync(true);

            var handler = new CreateDocumentCommandHandler(repository.Object, service.Object, logger.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(result.Data);
            Assert.False(result.IsSuccess);
            Assert.Equal("Já existe um documento com este título.", result.Message);

            repository.Verify(x => x.ExistsByTitleAsync(documentModel.Title, null), Times.Once);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_FileValidationFails()
        {
            // Arrange
            var repository = new Mock<IDocumentRepository>();
            var service = new Mock<IDocumentService>();
            var logger = new Mock<ILogger<CreateDocumentCommandHandler>>();

            byte[] filebytes = Encoding.UTF8.GetBytes("dummy image");
            IFormFile file = new FormFile(new MemoryStream(filebytes), 0, filebytes.Length, "Data", "image.png");

            var documentCreateViewModel = new DocumentCreateViewModel
            {
                Title = "Contrato de Prestação de Serviços",
                Description = "Documento referente ao contrato firmado com o cliente XPTO Ltda.",
                FileName = "contrato_prestacao_servicos.pdf",
                File = file
            };

            var documentModel = new DocumentViewModel
            {
                Id = 1,
                Title = "Contrato de Prestação de Serviços",
                Description = "Documento referente ao contrato firmado com o cliente XPTO Ltda.",
                FileName = "contrato_prestacao_servicos.pdf",
                FilePath = "/uploads/2025/07/contrato_prestacao_servicos.pdf",
                CreatedAt = DateTime.Now,
                FileSize = 256034,
                ContentType = "application/pdf"
            };

            var command = new CreateDocumentCommand(documentCreateViewModel);

            string validationError = "Arquivo inválido";
            service
                .Setup(x => x.ValidateFile(It.IsAny<IFormFile>(), out validationError))
                .Returns(false);

            repository
                .Setup(x => x.ExistsByTitleAsync(documentModel.Title, null))
                .ReturnsAsync(false);

            var handler = new CreateDocumentCommandHandler(repository.Object, service.Object, logger.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(result.Data);
            Assert.False(result.IsSuccess);
            Assert.Equal(validationError, result.Message);

            repository.Verify(x => x.ExistsByTitleAsync(documentModel.Title, null), Times.Once);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_RepositoryThrowsException()
        {
            // Arrange
            var repository = new Mock<IDocumentRepository>();
            var service = new Mock<IDocumentService>();
            var logger = new Mock<ILogger<CreateDocumentCommandHandler>>();

            byte[] filebytes = Encoding.UTF8.GetBytes("dummy image");
            IFormFile file = new FormFile(new MemoryStream(filebytes), 0, filebytes.Length, "Data", "image.png");

            var documentCreateViewModel = new DocumentCreateViewModel
            {
                Title = "Contrato de Prestação de Serviços",
                Description = "Documento referente ao contrato firmado com o cliente XPTO Ltda.",
                FileName = "contrato_prestacao_servicos.pdf",
                File = file
            };

            var documentModel = new DocumentViewModel
            {
                Id = 1,
                Title = "Contrato de Prestação de Serviços",
                Description = "Documento referente ao contrato firmado com o cliente XPTO Ltda.",
                FileName = "contrato_prestacao_servicos.pdf",
                FilePath = "/uploads/2025/07/contrato_prestacao_servicos.pdf",
                CreatedAt = DateTime.Now,
                FileSize = 256034,
                ContentType = "application/pdf"
            };

            var command = new CreateDocumentCommand(documentCreateViewModel);

            repository
                .Setup(x => x.ExistsByTitleAsync(documentModel.Title, null))
                    .ReturnsAsync(false);

            service
                .Setup(x => x.ValidateFile(It.IsAny<IFormFile>(), out It.Ref<string>.IsAny))
                    .Returns(true);

            service
                .Setup(x => x.SaveFileAsync(It.IsAny<IFormFile>()))
                    .ThrowsAsync(new Exception("Falha ao salvar arquivo"));

            var handler = new CreateDocumentCommandHandler(repository.Object, service.Object, logger.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(result.Data);
            Assert.False(result.IsSuccess);
            Assert.Contains("Erro ao criar documento", result.Message);

            repository.Verify(x => x.ExistsByTitleAsync(documentModel.Title, null), Times.Once);
        }
    }
}