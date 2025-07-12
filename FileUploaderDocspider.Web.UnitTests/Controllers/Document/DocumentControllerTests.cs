using FileUploaderDocspider.Application.Commands;
using FileUploaderDocspider.Application.Queries;
using FileUploaderDocspider.Controllers;
using FileUploaderDocspider.Core.Domains.ViewModels;
using FileUploaderDocspider.Core.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NetDevPack.SimpleMediator;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FileUploaderDocspider.Web.UnitTests.Controllers.Document
{

    public class DocumentControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly DocumentController _controller;

        public DocumentControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new DocumentController(_mediatorMock.Object);
        }

        [Fact]
        public async Task Should_ReturnViewWithDocuments_When_IndexIsCalledAndSuccess()
        {
            // Arrange
            var documents = new List<DocumentViewModel> { new DocumentViewModel() };
            var response = Result<IEnumerable<DocumentViewModel>>.Success(documents);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllDocumentsQuery>(), default))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(documents, viewResult.Model);
        }

        [Fact]
        public async Task Should_ReturnViewWithEmptyList_When_IndexFails()
        {
            // Arrange
            var response = Result<IEnumerable<DocumentViewModel>>.Failure("Error");
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllDocumentsQuery>(), default))
                .ReturnsAsync(response);

            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<List<DocumentViewModel>>(viewResult.Model);
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_DetailsDocumentNotFound()
        {
            // Arrange
            var response = Result<DocumentViewModel>.Failure("Not found");
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetDocumentByIdQuery>(), default))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Should_RedirectToIndex_When_CreateDocumentSuccess()
        {
            // Arrange
            var model = new DocumentCreateViewModel { Title = "Test", Description = "Desc", FileName = "file.txt", File = new Mock<IFormFile>().Object };
            var response = Result<DocumentViewModel>.Success(new DocumentViewModel());
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateDocumentCommand>(), default))
                .ReturnsAsync(response);

            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );

            // Act
            var result = await _controller.Create(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Should_ReturnViewWithModel_When_CreateDocumentFails()
        {
            // Arrange
            var model = new DocumentCreateViewModel { Title = "Test", Description = "Desc", FileName = "file.txt", File = new Mock<IFormFile>().Object };
            var response = Result<DocumentViewModel>.Failure("Error");
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateDocumentCommand>(), default))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Create(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_EditDocumentNotFound()
        {
            // Arrange
            var response = Result<DocumentViewModel>.Failure("Not found");
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetDocumentByIdQuery>(), default))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Should_RedirectToIndex_When_EditDocumentSuccess()
        {
            // Arrange
            var model = new DocumentEditViewModel { Id = 1, Title = "Test", Description = "Desc", FileName = "file.txt" };
            var response = Result<DocumentViewModel>.Success(new DocumentViewModel());
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateDocumentCommand>(), default))
                .ReturnsAsync(response);

            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );

            // Act
            var result = await _controller.Edit(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Should_ReturnViewWithModel_When_EditDocumentFails()
        {
            // Arrange
            var model = new DocumentEditViewModel { Id = 1, Title = "Test", Description = "Desc", FileName = "file.txt" };
            var response = Result<DocumentViewModel>.Failure("Error");
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateDocumentCommand>(), default))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Edit(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task Should_RedirectToIndex_When_DeleteDocumentSuccess()
        {
            // Arrange
            var response = Result<bool>.Success(true);
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteDocumentCommand>(), default))
                .ReturnsAsync(response);

            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Should_ReturnNotFound_When_DownloadDocumentNotFound()
        {
            // Arrange
            var response = Result<DocumentViewModel>.Failure("Not found");
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetDocumentByIdQuery>(), default))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Download(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}