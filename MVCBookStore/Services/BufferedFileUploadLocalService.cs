using MVCBookStore.Interfaces;

namespace MVCBookStore.Services
{
    public class BufferedFileUploadLocalService : IBufferedFileUploadService
    {
        public async Task<string> UploadFile(IFormFile file, IWebHostEnvironment webHostEnvironment, string folder)
        {
            string path = "";
            try
            {
                if (file.Length > 0)
                {
                    path = Path.GetFullPath(Path.Combine(webHostEnvironment.WebRootPath, folder));
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (var fileStream = new FileStream(Path.Combine(path, file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return file.FileName;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("File Copy Failed", ex);
            }
        }
    }
}
