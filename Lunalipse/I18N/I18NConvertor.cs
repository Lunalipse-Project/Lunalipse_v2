﻿using Lunalipse.Common.Data;
using Lunalipse.Common.Interfaces.II18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.I18N
{
    public class I18NConvertor : II18NConvertor
    {

        II18NPages Pages = null;

        public I18NConvertor(II18NPages pgI)
        {
            if (pgI != null) Pages = pgI;
        }

        public string ConvertTo(SupportedPages page, string key)
        {
            if (Pages == null) return key;
            return Pages.GetPage(page).getContext(key);
        }
    }
}
