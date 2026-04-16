namespace NoobProject.Helper {
    public class ImageHelper {
        private readonly IWebHostEnvironment _env;

        public ImageHelper(IWebHostEnvironment env) {
            _env = env;
        }

       
        // save it to memory and return web-url to call it
        // dont forget to resolve the url ( add request host ) 
        public async Task<string> SaveImageAsync(IFormFile image) {
            // note : web urls use ===> /          PC urls use ===> \

            // _env.WebRootPath = "C:\MyProject\wwwroot"

            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            // "C:\MyProject\wwwroot\uploads"

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            // C:\MyProject\wwwroot\uploads\a1b2c3d4-e5f6-g7h8-i9j0-k1l2m3n4o5p6.jpg

            using (var stream = new FileStream(filePath, FileMode.Create)) {
                await image.CopyToAsync(stream);
            }
            var imageUrl = $"/uploads/{uniqueFileName}";
            return imageUrl;
        }
        public Task DeleteImages(List<string> imagesPaths) {
            foreach (var imagesPath in imagesPaths)
                DeleteImage(imagesPath);
            return Task.CompletedTask;
        }
        // delete from memory , then u delete it from DB
        public Task DeleteImage(string imagePath) {
            // image path ===> /uploads/imageName

            var relativePath = imagePath[1..];
            // uploads/imageName

            relativePath = relativePath.Replace("/", "\\");
            // uploads\imageName

            var fullPath = Path.Combine(_env.WebRootPath, relativePath);
            // C:\MyProject\wwwroot\uploads\imageName

            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);

            return Task.CompletedTask;
        }

        // the relative path from DB and resolve it to web url by adding request host and scheme
        // relative path ===> /uploads/imageName
        // request scheme ===> http or https  , from Request.Scheme ( Request is a property in the controller that gives you access to the current HTTP request )
        // request host ===> example.com , from Request.Host
        public List<string>? ResolveImageUrls(List<string>? relativePaths, string requestScheme, string requestHost) {
            var images = relativePaths?.Select(p => $"{requestScheme}://{requestHost}{p}").ToList();
            return images;
        }
        public string? ResolveImageUrl(string? relativePath, string requestScheme, string requestHost) {
            if(relativePath == null) return null;   
            var img = $"{requestScheme}://{requestHost}{relativePath}";
            return img;
        }
    }
}
