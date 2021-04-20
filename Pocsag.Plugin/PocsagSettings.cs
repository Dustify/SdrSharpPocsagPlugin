namespace Pocsag.Plugin
{
    using SDRSharp.Radio;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class PocsagSettings
    {
        public bool DeDuplicate
        {
            get
            {
                return Utils.GetBooleanSetting("plugin.pocsag.DeDuplicate", true);
            }
            set
            {
                Utils.SaveSetting("plugin.pocsag.DeDuplicate", value);
            }
        }

        public bool HideBadDecodes
        {
            get
            {
                return Utils.GetBooleanSetting("plugin.pocsag.HideBadDecodes", true);
            }
            set
            {
                Utils.SaveSetting("plugin.pocsag.HideBadDecodes", value);
            }
        }

        public bool MultilinePayload
        {
            get
            {
                return Utils.GetBooleanSetting("plugin.pocsag.MultilinePayload", false);
            }
            set
            {
                Utils.SaveSetting("plugin.pocsag.MultilinePayload", value);
            }
        }

        public int Pocsag512FilterDepth
        {
            get
            {
                return Utils.GetIntSetting("plugin.pocsag.Pocsag512FilterDepth", 92);
            }
            set
            {
                Utils.SaveSetting("plugin.pocsag.Pocsag512FilterDepth", value);
            }
        }

        public int Pocsag1200FilterDepth
        {
            get
            {
                return Utils.GetIntSetting("plugin.pocsag.Pocsag1200FilterDepth", 46);
            }
            set
            {
                Utils.SaveSetting("plugin.pocsag.Pocsag1200FilterDepth", value);
            }
        }

        public int Pocsag2400FilterDepth
        {
            get
            {
                return Utils.GetIntSetting("plugin.pocsag.Pocsag2400FilterDepth", 23);
            }
            set
            {
                Utils.SaveSetting("plugin.pocsag.Pocsag2400FilterDepth", value);
            }
        }
    }
}
