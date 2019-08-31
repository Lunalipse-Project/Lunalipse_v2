using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.AudioControlPanel;
using Lunalipse.Common.Interfaces.IPlayList;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.LpsAudio
{
    /// <summary>
    /// Controller for audio sequence
    /// </summary>
    public class SequenceControllerManager
    {
        static volatile SequenceControllerManager ManagerInstance;
        static readonly object InstanceLock = new object();

        // Controllers - 4 args
        // Action<MusicEntity> PrepareMusicFunc
        // ICatalogue catalogue
        // PlayMode playmode
        // bool isNext
        public Dictionary<string, SeqController> Controllers { get; private set; }

        public SeqController CurrentController { get; private set; }

        public string CurrentControlerID { get; private set; }

        public static SequenceControllerManager Instance
        {
            get
            {
                if (ManagerInstance == null)
                {
                    lock(InstanceLock)
                    {
                        ManagerInstance = ManagerInstance ?? new SequenceControllerManager();
                    }
                }
                return ManagerInstance;
            }
        }

        private SequenceControllerManager()
        {
            Controllers = new Dictionary<string, SeqController>();
        }

        public void AddController(string id, Action<Action<MusicEntity>, ICatalogue, PlayMode, bool> Controller, bool canInterference = false, bool supportScript = false)
        {
            Controllers.AddNonRepeat(id, new SeqController()
            {
                controllerDelegation = Controller,
                FullyTakeover = canInterference,
                SupportScipt = supportScript
            });
        }

        public void RemoveController(string id)
        {
            Controllers.Remove(id);
        }

        public void SetController(string id)
        {
            if(Controllers.ContainsKey(id))
            {
                CurrentControlerID = id;
                CurrentController = Controllers[id];
            }
        }
    }

    public class SeqController
    {
        public Action<Action<MusicEntity>, ICatalogue, PlayMode, bool> controllerDelegation;
        /// <summary>
        /// 指示用户是否可以干预控制器控制的顺序
        /// </summary>
        public bool FullyTakeover = false;
        public bool SupportScipt = false;
    }
}
