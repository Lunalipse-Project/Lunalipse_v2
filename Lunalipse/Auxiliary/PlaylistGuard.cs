using Lunalipse.Common.Bus.Event;
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.Cache;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Core.Cache;
using Lunalipse.Core.PlayList;
using Lunalipse.Presentation.BasicUI;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Lunalipse.Auxiliary
{
    public class PlaylistGuard : ITranslatable
    {
        CacheHub cacheSystem;
        CataloguePool cataloguePool;
        MusicListPool musicListPool;
        EventBus EVENT_BUS;

        string missingTitle, deleteConfirmTitle;
        string missingContent, deleteConfirmContent;

        public PlaylistGuard()
        {
            cacheSystem = CacheHub.Instance();
            cataloguePool = CataloguePool.Instance;
            musicListPool = MusicListPool.Instance();
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
                cacheSystem.DeleteCache(CacheType.PlayList, tobeDelete.UUID);
                cataloguePool.RemoveCatalogue(tobeDelete);
                EVENT_BUS.Boardcast(EventBusTypes.ON_ACTION_COMPLETE, "C_UPD_USR");
            }
        }
        
        public void Restore()
        {
            bool IsEntityMissing = false;
            foreach(CatalogueMetadata metadata in cacheSystem.RestoreObjects<CatalogueMetadata>(CacheType.PlayList))
            {
                Catalogue NewCatalogue = new Catalogue(metadata.Name, metadata.Uuid);
                NewCatalogue.isUserDefined = true;
                foreach (Tuple<string,string> tups in metadata.Musics)
                {
                    MusicEntity Entity = musicListPool.Musics.Find(x => x.MusicID == tups.Item1 || x.Name == tups.Item2);
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
            foreach (Catalogue catalogue in cataloguePool.GetUserDefined())
            {
                cacheSystem.CacheObject(catalogue, CacheType.PlayList);
            }
        }

        public void SaveMusicCache()
        {
            foreach (Catalogue catalogue in cataloguePool.GetLocationClassified())
            {
                cacheSystem.CacheObject(catalogue.MusicList, CacheType.MusicList, catalogue.Name);
            }
        }

        public void Translate(II18NConvertor i8c)
        {
            missingContent = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_PLAYLISTGUARD_MISSING_CONTENT");
            deleteConfirmContent = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_CATALOGUE_CATADELETE_MSG");
            missingTitle = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_PLAYLISTGUARD_MISSING_TITLE");
            deleteConfirmTitle = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_CATALOGUE_CATADELETE_TITLE");
        }
    }
}
