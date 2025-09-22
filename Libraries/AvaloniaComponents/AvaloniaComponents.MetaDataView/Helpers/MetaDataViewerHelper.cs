using Avalonia.Media.Imaging;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaComponents.MetaDataView.Helpers
{
    public static class MetaDataViewHelper
    {
        public static byte[] GetByteArrayFrom(Bitmap bmp)
        {
            var stream = new MemoryStream();
            bmp.Save(stream);
            return stream.ToArray();
        }

        public static byte[]? GetByteArrayFrom(AvaloniaPdfViewer.PdfViewer viewer)
        {
            return viewer.Source;
        }
    }
}
