using FileUploaderDocspider.Core.Domains.Models;
using FileUploaderDocspider.Core.Domains.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileUploaderDocspider.Core.Domains.Mappings
{
    public static class MappingExtensions
    {
        public static Document ToDomain(this DocumentCreateViewModel viewModel)
            => new Document
            {
                Title = viewModel.Title,
                Description = viewModel.Description,
                FileName = string.IsNullOrWhiteSpace(viewModel.FileName)
                    ? viewModel.File?.FileName
                    : viewModel.FileName,
                CreatedAt = DateTime.Now
            };

        public static DocumentViewModel ToViewModel(this Document document)
            => new DocumentViewModel
            {
                Id = document.Id,
                Title = document.Title,
                Description = document.Description,
                FileName = document.FileName,
                FilePath = document.FilePath,
                CreatedAt = document.CreatedAt,
                FileSize = document.FileSize,
                ContentType = document.ContentType
            };

        public static IEnumerable<DocumentViewModel> ToViewModel(this IEnumerable<Document> documents)
            => documents.Select(d => d.ToViewModel());

        public static Document UpdateFromViewModel(this Document document, DocumentEditViewModel viewModel)
        {
            document.Title = viewModel.Title;
            document.Description = viewModel.Description;
            document.FileName = string.IsNullOrWhiteSpace(viewModel.FileName)
                ? document.FileName
                : viewModel.FileName;
            return document;
        }
    }
}