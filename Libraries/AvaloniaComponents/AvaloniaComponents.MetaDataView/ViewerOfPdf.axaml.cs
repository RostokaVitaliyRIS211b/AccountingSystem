using Avalonia;
using Avalonia.Controls.Primitives;

using AvaloniaPdfViewer;

using System.IO;



namespace AvaloniaComponents.MetaDataView;

public class ViewerOfPdf : TemplatedControl,IMetaDataView
{
    public static readonly StyledProperty<byte[]> ByteArraySourceProperty =
        AvaloniaProperty.Register<PdfViewer, byte[]>(nameof(ByteArraySource));

    public byte[] ByteArraySource
    {
        get => this.GetValue(ByteArraySourceProperty);
        set => SetValue(ByteArraySourceProperty, value);
    }


    public byte[] GetDataAsByteArray()
    {
        return ByteArraySource;
    }

    public void LoadDocument(string fileFullPath)
    {
        ByteArraySource = File.ReadAllBytes(fileFullPath);
    }

    public void LoadDocument(byte[] data)
    {
        ByteArraySource = data;
    }
}