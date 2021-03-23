namespace Pocsag.Plugin
{
    using SDRSharp.Common;
    using System.Windows.Forms;

    public class PocsagPlugin : ISharpPlugin
    {
        private PocsagControl gui;
        private ISharpControl control;

        public UserControl Gui
        {
            get
            {
                if (this.gui == null)
                {
                    this.gui = new PocsagControl(this.control);
                }

                return this.gui;
            }
        }

        public string DisplayName => "POCSAG Decoder";

        public void Close()
        {

        }

        public void Initialize(ISharpControl control)
        {
            this.control = control;
        }
    }
}
