using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;

using AvaloniaComponents.MetaDataView.Helpers;

using System.IO;

namespace AvaloniaComponents.MetaDataView;

public class ImageViewer : TemplatedControl,IMetaDataView
{
    public static readonly StyledProperty<Bitmap> BitMapSourceProperty =
       AvaloniaProperty.Register<ImageViewer, Bitmap>(nameof(BitMapSource));


    public Bitmap BitMapSource
    {
        get => this.GetValue(BitMapSourceProperty);
        set => SetValue(BitMapSourceProperty, value);
    }



    public byte[] GetDataAsByteArray()
    {
        return MetaDataViewHelper.GetByteArrayFrom(BitMapSource);
    }

    public void LoadDocument(string fileFullPath)
    {
        BitMapSource = new Bitmap(fileFullPath);
    }

    public void LoadDocument(byte[] data)
    {
        BitMapSource = new Bitmap(new MemoryStream(data));
    }
}