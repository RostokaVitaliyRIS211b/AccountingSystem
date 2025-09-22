using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

using AvaloniaPdfViewer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaComponents.MetaDataView.Helpers
{
    public static class ViewerHelper
    {
        #region Создание контролов для просмотра метаданных сигнала
        //==============================================================

        /// <summary>
        /// Создаёт TemplatedControl для просмотра PDF и загружает данные из файла (поддерживает интерфейс <see cref="IMetaDataView"/>)
        /// </summary>
        /// <param name="fileFullPath">Полный путь к pdf-файлу</param>
        /// <returns></returns>
        public static ViewerOfPdf CreatePdfViewer(string fileFullPath)
        {
            ViewerOfPdf v = new ();
            v.LoadDocument(fileFullPath);
            return v;
        }

        /// <summary>
        /// Создаёт TemplatedControl для просмотра PDF и загружает данные из массива байт (поддерживает интерфейс <see cref="IMetaDataView"/>)
        /// </summary>
        /// <param name="pdf">Массив байт с данными в формате PDF</param>
        /// <returns></returns>
        public static ViewerOfPdf CreatePdfViewer(byte[] pdf)
        {
            ViewerOfPdf v = new ();
            v.LoadDocument(pdf);
            return v;
        }

        /// <summary>
        /// Создаёт TemplatedControl для просмотра изображений и загружает данные из файла (поддерживает интерфейс <see cref="IMetaDataView"/>)
        /// </summary>
        /// <param name="fileFullPath">Полный путь к файлу изображения</param>
        /// <returns></returns>
        public static ImageViewer CreateImageViewer(string fileFullPath)
        {
            ImageViewer v = new ();
            v.LoadDocument(fileFullPath);
            return v;
        }

        /// <summary>
        /// Создаёт TemplatedControl для просмотра изображений и загружает данные из массива байт (поддерживает интерфейс <see cref="IMetaDataView"/>)
        /// </summary>
        /// <param name="image">Массив байт с данными в формате изображения</param>
        /// <returns></returns>
        public static ImageViewer CreateImageViewer(byte[] image)
        {
            ImageViewer v = new ();
            v.LoadDocument(image);
            return v;
        }

        /// <summary>
        /// Создает TabItem для просмотра изображения, ПРИМЕЧАНИЕ: кнопка закрытия вкладки в свойстве Tag содержит вкладку к которой она относится
        /// </summary>
        /// <param name="dataName">Имя изображения</param>
        /// <param name="fileFullPath">Полный путь к файлу изображения</param>
        /// <param name="tag">Данные для свойтсва Tag у создаваемого TabItem</param>
        /// <param name="CloseTabEventHandler">Обработчик нажатия на кнопку закрытия вкладки,Если null то кнопка закрытия не будет добавлена, кнопка в свойстве Tag содержит вкладку к которой она относится</param>
        /// <returns> TabItem </returns>
        public static TabItem CreateImageTabItem(string dataName, string fileFullPath, object? tag = null, EventHandler<RoutedEventArgs>? CloseTabEventHandler = null)
        {
            return CreateTabItem(dataName, CreateImageViewer(fileFullPath), tag, CloseTabEventHandler);
        }

        /// <summary>
        /// Создает TabItem для просмотра изображения, ПРИМЕЧАНИЕ: кнопка закрытия вкладки в свойстве Tag содержит вкладку к которой она относится
        /// </summary>
        /// <param name="dataName">Имя изображения</param>
        /// <param name="data">Массив байт с данными в формате изображения</param>
        /// <param name="tag">Данные для свойтсва Tag у создаваемого TabItem</param>
        /// <param name="CloseTabEventHandler">Обработчик нажатия на кнопку закрытия вкладки,Если null то кнопка закрытия не будет добавлена, кнопка в свойстве Tag содержит вкладку к которой она относится</param>
        /// <returns> TabItem </returns>
        public static TabItem CreateImageTabItem(string dataName, byte[] data, object? tag = null, EventHandler<RoutedEventArgs>? CloseTabEventHandler = null)
        {
            return CreateTabItem(dataName, CreateImageViewer(data), tag, CloseTabEventHandler);
        }


        /// <summary>
        /// Создает TabItem для просмотра pdf, ПРИМЕЧАНИЕ: кнопка закрытия вкладки в свойстве Tag содержит вкладку к которой она относится
        /// </summary>
        /// <param name="dataName">Имя pdf файла</param>
        /// <param name="fileFullPath">Полный путь к файлу pdf </param>
        /// <param name="tag">Данные для свойтсва Tag у создаваемого TabItem</param>
        /// <param name="CloseTabEventHandler">Обработчик нажатия на кнопку закрытия вкладки,Если null то кнопка закрытия не будет добавлена, кнопка в свойстве Tag содержит вкладку к которой она относится</param>
        /// <returns> TabItem </returns>
        public static TabItem CreatePdfTabItem(string dataName, string fileFullPath, object? tag = null, EventHandler<RoutedEventArgs>? CloseTabEventHandler = null)
        {
            return CreateTabItem(dataName, CreatePdfViewer(fileFullPath), tag, CloseTabEventHandler);
        }

        /// <summary>
        /// Создает TabItem для просмотра изображения, ПРИМЕЧАНИЕ: кнопка закрытия вкладки в свойстве Tag содержит вкладку к которой она относится
        /// </summary>
        /// <param name="dataName">Имя изображения</param>
        /// <param name="data">Массив байт с данными в формате PDF</param>
        /// <param name="tag">Данные для свойтсва Tag у создаваемого TabItem</param>
        /// <param name="CloseTabEventHandler">Обработчик нажатия на кнопку закрытия вкладки,Если null то кнопка закрытия не будет добавлена, кнопка в свойстве Tag содержит вкладку к которой она относится</param>
        /// <returns> TabItem </returns>
        public static TabItem CreatePdfTabItem(string dataName, byte[] data, object? tag = null, EventHandler<RoutedEventArgs>? CloseTabEventHandler = null)
        {
            return CreateTabItem(dataName, CreatePdfViewer(data), tag, CloseTabEventHandler);
        }

        private static TabItem CreateTabItem(string dataName, object content, object? tag = null, EventHandler<RoutedEventArgs>? eventHandler = null)
        {
            TabItem item = new TabItem();
            DockPanel p = new DockPanel() { HorizontalAlignment = HorizontalAlignment.Stretch };


            if (eventHandler is not null)
            {
                Button btn = new Button()
                {
                    Content = "✕",
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Padding = new Thickness(5),
                    Margin = new Thickness(5, 0, 0, 0),
                    FontSize = 12,
                    VerticalAlignment = VerticalAlignment.Center,
                    Tag = item
                };
                ToolTip.SetTip(btn, "Удалить вкладку");
                btn.Click += eventHandler;
                DockPanel.SetDock(btn, Dock.Right);
                p.Children.Add(btn);
            }
            p.Children.Add(new TextBlock() { Text = dataName, VerticalAlignment = VerticalAlignment.Center, FontSize = 12 });

            item.Header = p;
            item.Content = content;
            item.Tag = tag;

            return item;
        }

        //==============================================================
        #endregion

        /// <summary>
        /// Метод для получения названия созданного TabItem
        /// </summary>
        /// <param name="item">Созданный через данный ViewerHelper TabItem</param>
        /// <returns></returns>
        public static string? GetTabItemHeader(TabItem item)
        {
            string? s = item.Header?.ToString();
            DockPanel? p = item.Header as DockPanel;
            if (p?.Children.Count > 0)
            {
                foreach (var child in p.Children)
                {
                    TextBlock? t = child as TextBlock;
                    if (t != null)
                    {
                        s = t.Text;
                        break;
                    }
                }
            }
            return s;
        }

        /// <summary>
        /// Метод для получения данных в виде массива байт из TabItem созданного через данный статический класс
        /// </summary>
        /// <param name="item">TabItem созданный через данный статический класс</param>
        /// <returns></returns>
        public static byte[] GetDataFromTab(TabItem item)
        {
            if (item.Content is IMetaDataView view)
            {
                return view.GetDataAsByteArray();
            }
            throw new ArgumentException("Переданный TabItem не был создан через данный статический класс");
        }
    }
}
