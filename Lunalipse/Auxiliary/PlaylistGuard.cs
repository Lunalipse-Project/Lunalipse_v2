using Lunalipse.Common.Bus.Event;
using Lunalipse.Common.Data;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Core.Cache;
using Lunalipse.Core.PlayList;
using Lunalipse.Presentation.BasicUI;
using Lunalipse.Utilities;
using System;
using System.IO;
using System.Windows;

namespace Lunalipse.Auxiliary
{
    public class PlaylistGuard : ITranslatable
    {
        CacheHub cacheHub;
        CataloguePool cataloguePool;
        MusicListPool musicListPool;
        EventBus EVENT_BUS;

        string missingTitle, deleteConfirmTitle;
        string missingContent, deleteConfirmContent;

        string savedFolder;

        const string CATALOGUE_FILE_EXTENSION = "cata";

        public PlaylistGuard()
        {
            cacheHub = CacheHub.INSTANCE();
            cataloguePool = CataloguePool.INSATNCE;
            musicListPool = MusicListPool.INSATNCE();
            savedFolder = Environment.CurrentDirectory + @"\UserData\";
            if (!Directory.Exists(savedFolder)) Directory.CreateDirectory(savedFolder);
            EVENT_BUS = EventBus.Instance;
            EVENT_BUS.AddUnicastReciever("PlaylistGuard", PlayListGuard_UnicastReciever);
        }

        protected void PlayListGuard_UnicastReciever(EventBusTypes MsgType,object[] data)
        {
            if(MsgType == EventBusTypes.ON_ACTION_DELETE)
            {
                string CatalogueUid = data[0] as string;
                Catalogue tobeDelete = cataloguePool.GetCatalogue(CatalogueUid);
                if (tobeDelete == null) return;
                CommonDialog WarningToDelete = new CommonDialog(deleteConfirmTitle, deleteConfirmContent.FormateEx(tobeDelete.Name), MessageBoxButton.YesNo);
                if(!WarningToDelete.ShowDialog().Value) return;

                string SavedCatalogue = savedFolder + CatalogueUid + ".cata";
                if (!File.Exists(SavedCatalogue)) return;
                File.Delete(savedFolder + CatalogueUid + ".cata");
                EVENT_BUS.Boardcast(EventBusTypes.ON_ACTION_COMPLETE, "C_UPD_USR");
            }
        }
        
        public void Restore()
        {
            bool IsEntityMissing = false;
            foreach(string path in Directory.GetFiles(savedFolder))
            {
                Catalogue restoredCatalogue = Read(path);
                Catalogue NewCatalogue = new Catalogue(restoredCatalogue.Name, restoredCatalogue.UUID);
                NewCatalogue.isUserDefined = restoredCatalogue.isUserDefined;
                foreach (MusicEntity me in restoredCatalogue.MusicList)
                {
                    MusicEntity Entity = musicListPool.Musics.Find(x => Path.GetFileName(x.Path) == Path.GetFileName(me.Path));
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
            deleteConfirmContent = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_CATALOGUE_CATADELETE_MSG");
            missingTitle = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_PLAYLISTGUARD_MISSING_TITLE");
            deleteConfirmTitle = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_CATALOGUE_CATADELETE_TITLE");
        }

        private void Save(Catalogue catalogue)
        {
            byte[] savedData = UniversalObjectSerializor<Catalogue>.Serialize(catalogue);
            Compression.Compress(savedData, "{0}/{1}.{2}".FormateEx(savedFolder, catalogue.Uid(), CATALOGUE_FILE_EXTENSION));
        }

        private Catalogue Read(string CataloguePath)
        {
            byte[] savedData = Compression.Decompress(CataloguePath);
            return UniversalObjectSerializor<Catalogue>.Deserialize(savedData);
        }
    }
}
