using Lunalipse.Utilities;
using System;
using System.Collections.Generic;

namespace Lunalipse.Presentation.Generic
{
    public class ContentManager<TKey>
    {
        public ContentManager()
        {

        }

        /*
         * A LITTLE IDEA
         * use json to create a dynamic content, the setting plane made up by an ITEM LIST.
         * json:
         * GUI_CONTENT:
         *      SETTING_CLASS: STRING(
         *          SELECTABLE[
         *              IN ENUM(SettingCatalogues)
         *          ]
         *      ),
         *      SETTING_TEXT: STRING(),  //I18N TOKEN
         *      TYPE_OF_CONTROL: STRING(
         *          SELECTABLE[
         *              IN ENUM(ControlList)
         *          ]
         *      ),
         * #IF TYPE_OF_CONTROL == ControlList.BUTTON
         *      
         *      BTN_TEXT: STRING(),
         *       // FUNCTION NAME
         *       // WITH TWO PARAMETER, SIMILAR TO C# LISTENER
         *      BTN_LISTENER: STRING()
         *      
         * #ELSE IF TYPE_OF_CONTROL == ControlList.TEXTBOX
         *      
         *      TB_ACCPTABLE: STRING(
         *          SELECTABLE[
         *              INTEGER,
         *              FLOAT/DOUBLE,
         *              TEXT
         *          ]
         *      )
         *      
         * #ELSE IF TYPE_OF_CONTROL == ControlList.DORPDOWN
         * 
         *      DD_ITEM: ARRAY[
         *          TYPE[
         *              STRING
         *          ]
         *      ],
         *      // FUNCTION NAME
         *      // WITH ONE PARAMETER
         *      // ID OF EACH ITEM (FIRST ITEM IS 1,AND SECOND IS 2....)
         *      DD_LISTENER: STRING()
         *      
         * #ENDIF
         */

        //TODO ADD A METHOD THAT USE TO PARSE THE JSON TO FORM A LIST TO CONSTRUCT A UI. 
        //     USE RELECTION TO FIND THE LISTENER FUNCTION.
        //     EACH UI REFER TO A SPECIFIC PROCESS CLASS THAT CONTAIN THE BACKEND FUNCTION
    }
}
