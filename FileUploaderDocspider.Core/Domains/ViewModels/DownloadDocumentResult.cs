namespace FileUploaderDocspider.Core.Domains.ViewModels
{
    public class DownloadDocumentResult
    {
        public byte[] FileBytes { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
