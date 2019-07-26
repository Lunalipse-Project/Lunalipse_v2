using Lunalipse.Resource.Generic.Types;
using System.Collections.Generic;

namespace Lunalipse.Resource.Interface
{
    public interface ILrssReader
    {
        void LoadLrssCompressed(string path);
        bool RestoringMagic(byte[] DecKey = null);
        List<LrssIndex> GetIndex();
        LrssResource ReadResource(LrssIndex li);
    }
}