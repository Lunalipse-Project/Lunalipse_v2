using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements
{
    /// <summary>
    /// Represent spell invented by Star Swirl the Breaded. (Delegation to predefine functions)
    /// </summary>
    public class LetterStarSwirlSpell : LetterValue
    {
        Delegate starSwirlSpell;
        public int ParameterNumber { get; private set; }
        public LetterStarSwirlSpell(Delegate starSwirlSpell) : base(ElementType.SPELL_DELEGATION)
        {
            this.starSwirlSpell = starSwirlSpell;
            this.ParameterNumber = starSwirlSpell.Method.GetParameters().Length;
        }

        /// <summary>
        /// check parameter number is meet the requirement.
        /// return:
        ///     less than 0, too few parameters!
        ///     equal to 0, just fit.
        ///     greater than 0, too many parameters!
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        public int parameterCheck(LetterArrayList @params)
        {
            int size = @params == null ? 0 : @params.GetSize();
            return ParameterNumber.CompareTo(size);
        }

        public LetterValue CastTheSpell(LetterArrayList @params)
        {
            object[] parameters = new object[ParameterNumber];
            ParameterInfo[] parameterInfo = starSwirlSpell.Method.GetParameters();
            for (int i = 0; i < ParameterNumber; i++)
            {
                Type paraType = parameterInfo[i].ParameterType;
                parameters[i] = @params.getValueByTypeAt(i, paraType);
            }
            object returnVal = null;
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    returnVal = starSwirlSpell.Method.Invoke(starSwirlSpell.Target, parameters);
                });
            }
            catch(Exception e)
            {
                throw e.InnerException;
            }
            return CreateLetterValue(returnVal, starSwirlSpell.Method.ReturnType);
        }
    }
}
