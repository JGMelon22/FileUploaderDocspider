using Microsoft.AspNetCore.Mvc;
using NetDevPack.SimpleMediator;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileUploaderDocspider.Application.Queries;
using FileUploaderDocspider.Application.Commands;
using FileUploaderDocspider.Core.Domains.ViewModels;

namespace FileUploaderDocspider.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IMediator _mediator;

        public DocumentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Displays the list of documents.
        /// </summary>
        /// <returns>The document list view.</returns>
        public async Task<IActionResult> Index()
        {
            var result = await _mediator.Send(new GetAllDocumentsQuery());
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Message;
                return View(new List<DocumentViewModel>());
            }
            return View(result.Data);
        }

        /// <summary>
        /// Displays the details of a document.
        /// </summary>
        /// <param name="id">The document ID.</param>
        /// <returns>The details view or NotFound if not found.</returns>
        public async Task<IActionResult> Details(int id)
        {
            var result = await _mediator.Send(new GetDocumentByIdQuery(id));
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }
            return View(result.Data);
        }

        /// <summary>
        /// Shows the document creation form.
        /// </summary>
        /// <returns>The create view.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates a new document.
        /// </summary>
        /// <param name="model">The document create view model.</param>
        /// <returns>Redirects to Index on success or returns the view with errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _mediator.Send(new CreateDocumentCommand(model));
                if (result.IsSuccess)
                {
                    TempData["Success"] = "Document created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            return View(model);
        }

        /// <summary>
        /// Shows the document edit form.
        /// </summary>
        /// <param name="id">The document ID.</param>
        /// <returns>The edit view or NotFound if not found.</returns>
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _mediator.Send(new GetDocumentByIdQuery(id));
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }
            var document = result.Data;
            var model = new DocumentEditViewModel
            {
                Id = document.Id,
                Title = document.Title,
                Description = document.Description,
                FileName = document.FileName,
                ExistingFilePath = document.FilePath,
                CreatedAt = document.CreatedAt
            };
            return View(model);
        }

        /// <summary>
        /// Updates an existing document.
        /// </summary>
        /// <param name="model">The document edit view model.</param>
        /// <returns>Redirects to Index on success or returns the view with errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DocumentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _mediator.Send(new UpdateDocumentCommand(model.Id, model));
                if (result.IsSuccess)
                {
                    TempData["Success"] = "Document updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            return View(model);
        }

        /// <summary>
        /// Deletes a document.
        /// </summary>
        /// <param name="id">The document ID.</param>
        /// <returns>Redirects to Index.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteDocumentCommand(id));
            if (result.IsSuccess)
            {
                TempData["Success"] = "Document deleted successfully!";
            }
            else
            {
                TempData["Error"] = result.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Downloads a document file.
        /// </summary>
        /// <param name="id">The document ID.</param>
        /// <returns>The file for download or NotFound if not found.</returns>
        public async Task<IActionResult> Download(int id)
        {
            var result = await _mediator.Send(new GetDocumentByIdQuery(id));
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }
            var document = result.Data;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", document.FilePath);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, document.ContentType, document.FileName);
        }
    }
}