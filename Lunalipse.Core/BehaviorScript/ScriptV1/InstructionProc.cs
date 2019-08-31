using Lunalipse.Common.Data;
using Lunalipse.Common.Data.BehaviorScript;
using Lunalipse.Core.PlayList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV1
{
    static class InstructionProc
    {
        public static MusicEntity PROC_LUNA_PLAY(int type, object[] args, CataloguePool cp, ref Catalogue chosen, ref int ptr)
        {
            if(type == (int)DefinedCmd.LUNA_PLAY)
            {
                if (chosen == null)
                    chosen = cp.GetCatalogue(0);
                if (args.Length > 1)
                    LpsAudio.AudioDelegations.ChangeVolume((float)args[1]);
                return chosen.getMusic(ScriptUtil.RemoveExtension(args[0] as string));
            }
            return null;
        }
        public static MusicEntity PROC_LUNA_PLAYN(int type, object[] args, CataloguePool cp, ref Catalogue chosen, ref int ptr)
        {
            if (type == (int)DefinedCmd.LUNA_PLAYN)
            {
                if (chosen == null)
                    chosen = cp.GetCatalogue(0);
                if (args.Length > 1)
                    LpsAudio.AudioDelegations.ChangeVolume((float)args[1]);
                int num = (int)args[0];
                if (num < 0)
                    return chosen.getMusic(new Random().Next(0, chosen.GetCount()));
                else
                    return chosen.getMusic(num);

            }
            return null;
        }
        public static MusicEntity PROC_LUNA_PLAYC(int type, object[] args, CataloguePool cp, ref Catalogue chosen, ref int ptr)
        {
            if (type == (int)DefinedCmd.LUNA_PLAYC)
            {
                if (args.Length > 2)
                    LpsAudio.AudioDelegations.ChangeVolume((float)args[2]);
                return cp.GetCatalogueFirst(args[0] as string).getMusic(ScriptUtil.RemoveExtension(args[1] as string));
            }
            return null;
        }
        public static MusicEntity PROC_LUNA_NEXT(int type, object[] args, CataloguePool cp, ref Catalogue chosen, ref int ptr)
        {
            if (type == (int)DefinedCmd.LUNA_NEXT)
            {
                if (chosen == null)
                    chosen = cp.GetCatalogue(0);
                if (args.Length > 2)
                    LpsAudio.AudioDelegations.ChangeVolume((float)args[2]);
                return chosen.getNext();
            }
            return null;
        }
        public static MusicEntity PROC_LUNA_LLOOP(int type, object[] args, CataloguePool cp, ref Catalogue chosen, ref int ptr)
        {
            if (type == (int)DefinedCmd.LUNA_LLOOP)
            {
                ptr = 0;
                return null;
            }
            return null;
        }
        public static MusicEntity PROC_LUNA_EQZR(int type, object[] args, CataloguePool cp, ref Catalogue chosen, ref int ptr)
        {
            if (type == (int)DefinedCmd.LUNA_EQZR)
            {
                double[] dat = new double[10];
                for (int i = 0; i < 10; i++)
                {
                    double d = (double)args[i];
                    if (d > 12)
                    {
                        d = 12;
                    }
                    else if (d < -12)
                    {
                        d = -12;
                    }
                    dat[i] = d;
                }
                LpsAudio.AudioDelegations.ChangeEqualizerSetting?.Invoke(dat);
                return null;
            }
            return null;
        }
        public static MusicEntity PROC_LUNA_SET(int type, object[] args, CataloguePool cp, ref Catalogue chosen, ref int ptr)
        {
            if (type == (int)DefinedCmd.LUNA_SET)
            {
                chosen = cp.GetCatalogueFirst(args[0] as string);
                return null;
            }
            return null;
        }

        /* ################## Start of Suffix Proc ################## */

        public static bool PROC_SUFX_RAND(int type, object[] args, ref int Count)
        {
            if (type != (int)DefinedSuffix.SUFX_RAND) return false;
            Random r = new Random();
            if (args.Length < 2)
            {
                if((int)args[0]<=1)
                {
                    Count = 1;
                }
                else
                {
                    Count = r.Next(1, (int)args[0]);
                }
            }
            else
            {
                if ((int)args[1] <= (int)args[0])
                {
                    Count = 1;
                }
                else
                {
                    Count = r.Next((int)args[0], (int)args[1]);
                }
            }
            return true;
        }
        public static bool PROC_SUFX_COUNT(int type, object[] args, ref int Count)
        {
            if (type != (int)DefinedSuffix.SUFX_COUNT) return false;
            Count = (int)args[0];
            return true;
        }
    }
}
