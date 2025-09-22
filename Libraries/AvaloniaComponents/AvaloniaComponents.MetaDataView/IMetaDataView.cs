using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaComponents.MetaDataView
{
    public interface IMetaDataView
    {
        void LoadDocument(string fileFullPath);

        void LoadDocument(byte[] data);

        byte[] GetDataAsByteArray();
    }
}
