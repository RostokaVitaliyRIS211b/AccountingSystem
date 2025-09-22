using Avalonia.Controls;
using Avalonia.Interactivity;

using AvaloniaComponents.MetaDataView.Helpers;

using System.IO;

namespace AvaloniaComponents.MetaDataView.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";

    private byte[] Img1 { get; }
    private byte[] Img2 { get; }
    private byte[] Pdf1 { get; }
    private byte[] Pdf2 { get; }

    public MainViewModel()
    {
        Img1 = File.ReadAllBytes("G:\\niggers.png");
        Img2 = File.ReadAllBytes("G:\\nigger2.png");
        Pdf1 = File.ReadAllBytes("G:\\123.pdf");
        Pdf2 = File.ReadAllBytes("G:\\123456.pdf");
    }

    public void SetTabs(TabControl tabControl)
    {
        tabControl.Items.Add(ViewerHelper.CreateImageTabItem("nword.png",Img1,null, Closed));
        tabControl.Items.Add(ViewerHelper.CreateImageTabItem("nword2.png", Img2, null, Closed));
        tabControl.Items.Add(ViewerHelper.CreatePdfTabItem("123.pdf", Pdf1, null, Closed));
        tabControl.Items.Add(ViewerHelper.CreatePdfTabItem("123456.pdf", Pdf2, null, Closed));
    }

    public void Closed(object? sender,RoutedEventArgs e)
    {

    }
}
