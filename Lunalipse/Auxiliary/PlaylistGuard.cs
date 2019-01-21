using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.Cache;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Core.Cache;
using Lunalipse.Core.Metadata;
using Lunalipse.Core.PlayList;
using Lunalipse.Presentation.BasicUI;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lunalipse.Auxiliary
{
    public class PlaylistGuard : ITranslatable
    {
        CacheHub cacheHub;
        CataloguePool cataloguePool;
        MusicListPool musicListPool;

        string missingTitle;
        string missingContent;

        string savedFolder;

        const string CATALOGUE_FILE_EXTENSION = "cata";

        public PlaylistGuard()
        {
            cacheHub = CacheHub.INSTANCE();
            cataloguePool = CataloguePool.INSATNCE;
            musicListPool = MusicListPool.INSATNCE();
            savedFolder = Environment.CurrentDirectory + @"\UserData\";
            if (!Directory.Exists(savedFolder)) Directory.CreateDirectory(savedFolder);
        }

        
        public void Restore()
        {
            bool IsEntityMissing = false;
            foreach(string path in Directory.GetFiles(savedFolder))
            {
                Catalogue restoredCatalogue = Read(path);
                Catalogue NewCatalogue = new Catalogue(restoredCatalogue.Name);
                NewCatalogue.isUserDefined = restoredCatalogue.isUserDefined;
                foreach (MusicEntity me in restoredCatalogue.MusicList)
                {
                    if (!File.Exists(me.Path))
                    {
                        IsEntityMissing = true;
                        continue;
                    }
                    MusicEntity Entity = musicListPool.Musics.Find(x => x.Path == me.Path);
                    if (Entity != null)
                    {
                        NewCatalogue.MusicList.Add(Entity);
                    }
                    else IsEntityMissing = true;
                }
                cataloguePool.AddCatalogue(NewCatalogue);
            }

            if(IsEntityMissing)
            {
                CommonDialog EntityMissed = new CommonDialog(missingTitle, missingContent, MessageBoxButton.OK);
                EntityMissed.ShowDialog();
            }
        }

        public void SavePlaylist()
        {
            foreach (Catalogue catalogue in cataloguePool.GetUserDefined().FindAll(x => x.IsModified))
            {
                Save(catalogue);
            }
        }

        public void Translate(II18NConvertor i8c)
        {
            missingContent = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_PLAYLISTGUARD_MISSING_CONTENT");
            missingTitle = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_PLAYLISTGUARD_MISSING_TITLE");
        }

        private void Save(Catalogue catalogue)
        {
            byte[] savedData = UniversalObjectSerializor<Catalogue>.Serialize(catalogue);
            Compression.Compress(savedData, "{0}/{1}.{2}".FormateEx(savedFolder, catalogue.Name, CATALOGUE_FILE_EXTENSION));
        }

        private Catalogue Read(string CataloguePath)
        {
            byte[] savedData = Compression.Decompress(CataloguePath);
            return UniversalObjectSerializor<Catalogue>.Deserialize(savedData);
        }
    }
}
