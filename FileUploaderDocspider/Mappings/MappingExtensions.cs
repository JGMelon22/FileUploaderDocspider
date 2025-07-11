using FileUploaderDocspider.Dtos.Requests;
using FileUploaderDocspider.Dtos.Responses;
using FileUploaderDocspider.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileUploaderDocspider.Mappings
{
    public static class MappingExtensions
    {
        public static Document ToDomain(this DocumentRequest request)
            => new Document
            {
                Title = request.Title,
                Description = request.Description,
                FileName = request.FileName,
                FilePath = request.FilePath,
                FileSize = request.FileSize,
                ContentType = request.ContentType,
                CreatedAt = DateTime.Now
            };

        public static DocumentResponse ToResponse(this Document document)
            => new DocumentResponse
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

        public static IEnumerable<DocumentResponse> ToResponse(this IEnumerable<Document> documents)
        {
            return documents.Select(document =>
                new DocumentResponse
                {
                    Id = document.Id,
                    Title = document.Title,
                    Description = document.Description,
                    FileName = document.FileName,
                    FilePath = document.FilePath,
                    CreatedAt = document.CreatedAt,
                    FileSize = document.FileSize,
                    ContentType = document.ContentType
                });
        }

        public static Document UpdateFromRequest(this Document document, DocumentRequest request)
        {
            document.Title = request.Title;
            document.Description = request.Description;
            document.FileName = request.FileName;
            document.FilePath = request.FilePath;
            document.FileSize = request.FileSize;
            document.ContentType = request.ContentType;

            return document;
        }
    }
}