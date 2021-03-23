namespace Pocsag.Plugin
{
    using SDRSharp.Common;
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    public partial class PocsagControl : UserControl
    {
        private ISharpControl control;

        private PocsagProcessor processor;

        private BindingSource bindingSource;
        private BindingList<Pocsag.Message> bindingList;

        public PocsagControl(ISharpControl control)
        {
            InitializeComponent();

            this.bindingSource = new BindingSource();
            this.bindingList = new BindingList<Pocsag.Message>();

            this.bindingSource.DataSource = this.bindingList;

            this.control = control;

            this.processor =
                new PocsagProcessor(
                    this.control.AudioSampleRate,
                    (Pocsag.Message message) =>
                    {
                        this.MessageReceived(message);
                    });

            this.control.RegisterStreamHook(
                this.processor,
                SDRSharp.Radio.ProcessorType.DemodulatorOutput);

            this.processor.Enabled = true;

            this.dataGridView1.AutoGenerateColumns = false;

            this.dataGridView1.DataSource = this.bindingSource;
        }

        private void MessageReceived(Pocsag.Message message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(
                    new Action<Pocsag.Message>(
                        (message) =>
                        {
                            int firstDisplayed = this.dataGridView1.FirstDisplayedScrollingRowIndex;
                            int displayed = this.dataGridView1.DisplayedRowCount(true);
                            int lastVisible = (firstDisplayed + displayed) - 1;
                            int lastIndex = this.dataGridView1.RowCount - 1;

                            this.bindingList.Add(message);

                            if (lastVisible == lastIndex)
                            {
                                this.dataGridView1.FirstDisplayedScrollingRowIndex = firstDisplayed + 1;
                            }
                        }),
                    new object[] { message });
            }
        }
    }
}
