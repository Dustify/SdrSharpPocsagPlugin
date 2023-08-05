namespace SdrsDecoder.Plugin
{
    using  SdrsDecoder.Support;
    using SDRSharp.Common;
    using System;
    using System.Windows.Forms;

    public class PocsagPlugin : ISharpPlugin
    {
        private PocsagControl gui;
        private ISharpControl control;

        public UserControl Gui
        {
            get
            {
                try
                {
                    if (this.gui == null)
                    {
                        this.gui = new PocsagControl(this.control);
                    }

                    return this.gui;
                }
                catch (Exception exception)
                {
                    Log.LogException(exception);
                }

                return null;
            }
        }

        public string DisplayName => "Decoder";

        public void Close()
        {
        }

        public void Initialize(ISharpControl control)
        {
            try
            {
                this.control = control;
            }
            catch (Exception exception)
            {
                Log.LogException(exception);
            }
        }
    }
}
