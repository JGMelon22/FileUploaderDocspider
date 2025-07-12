using System;

namespace FileUploaderDocspider.Core.Domains.ViewModels
{
    public class DocumentViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
    }
}