﻿namespace SdrsDecoder.Plugin
{
    using SDRSharp.Radio;


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

        public string SelectedMode
        {
            get
            {
                return Utils.GetStringSetting("plugin.pocsag.SelectedMode", "POCSAG (512, 1200, 2400)");
            }
            set
            {
                Utils.SaveSetting("plugin.pocsag.SelectedMode", value);
            }
        }

        public bool Logging
        {
            get
            {
                return Utils.GetBooleanSetting("plugin.pocsag.Logging", false);
            }
            set
            {
                Utils.SaveSetting("plugin.pocsag.Logging", value);
            }
        }

        public string Filter
        {
            get
            {
                return Utils.GetStringSetting("plugin.pocsag.Filter", "");
            }
            set
            {
                Utils.SaveSetting("plugin.pocsag.Filter", value);
            }
        }
    }
}
