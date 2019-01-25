using Lunalipse.Common.Data;
using System;

namespace Lunalipse.Presentation.Generic
{
    public delegate void OnItemSelected<T>(T selected, object tag = null);
    public class Delegation
    {
        public static Action<object> RemovingItem;
        public static Action<MusicEntity> CatalogueUpdated;

        public static Action<object> AddToNewCatalogue;
        public static Action<MusicEntity> EditMetadata;
    }
}
