using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HOAPro.Services
{
    public class ImageFileService : HOAPro.Services.IImageFileService
    {
        private string _baseFile = null;
        public ImageFileService(string baseFile)
        {
            this._baseFile = baseFile;
        }

        public string CreateLocallyStoredFile(string imageDirectory)
        {
            if (File.Exists(this._baseFile))
            {
                string newFileName = Guid.NewGuid().ToString() + ".jpg";
                File.Copy(this._baseFile, newFileName);
                return newFileName;
            }
            else
                return null;
        }

        public static IImageFileService Create(string baseFile)
        {
            return new ImageFileService(baseFile);
        }
    }
}
