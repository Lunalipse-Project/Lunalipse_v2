using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Data.BehaviorScript
{
    public enum DefinedCmd : uint
    {
        CMD_NAN    =  0x0000,
        /// <summary>
        /// Play the song by name in current selected Catalogue, using specified volume
        /// <para>
        /// luna.play(String)
        /// </para>
        /// </summary>
        LUNA_PLAY =  0x0001,
        /// <summary>
        /// Play the song by index in current selected Catalogue
        /// <para>
        /// luna.playn(Int32)
        /// </para>
        /// </summary>
        LUNA_PLAYN =  0x0002,
        /// <summary>
        /// Play song by name in specified Catalogue
        /// <para>
        /// luna.playc(String,String)
        /// </para>
        /// </summary>
        LUNA_PLAYC =  0x0003,
        /// <summary>
        /// Adjust equalizer Setting
        /// <para>
        /// luna.eqzr(Int32[10])
        /// </para>
        /// </summary>
        LUNA_EQZR =  0x0004,
        /// <summary>
        /// Fetch next song in current selected Catalogue
        /// <para>
        /// luna.next(void)
        /// </para>
        /// </summary>
        LUNA_NEXT =  0x0005,
        /// <summary>
        /// Loop the script
        /// <para>
        /// luna.lloop(void)
        /// </para>
        /// </summary>
        LUNA_LLOOP =  0x0006,
        /// <summary>
        /// Set Catalogue
        /// <para>
        /// luna.set(String)
        /// </para>
        /// </summary>
        LUNA_SET =  0x0007
    }
}
