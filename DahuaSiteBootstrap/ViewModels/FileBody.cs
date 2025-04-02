using System.IO.Compression;

namespace DahuaSiteBootstrap.ViewModels
{
    public class FileBody
    {

        public string? DisplayName { get; set; }

        public string? Category { get; set; }

        public string? Description { get; set; }

        public IFormFile Data { get; set; }

        
    }
}
