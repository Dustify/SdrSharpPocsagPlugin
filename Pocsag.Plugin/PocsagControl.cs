namespace Pocsag.Plugin
{
    using SDRSharp.Common;
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Linq;

    public partial class PocsagControl : UserControl
    {
        private ISharpControl control;

        private PocsagProcessor processor;

        private BindingSource bindingSource;
        private BindingList<Pocsag.PocsagMessage> bindingList;

        private bool deDuplicate = true;

        private bool hideBad = true;

        public PocsagControl(ISharpControl control)
        {
            InitializeComponent();

            this.bindingSource = new BindingSource();
            this.bindingList = new BindingList<Pocsag.PocsagMessage>();

            this.bindingSource.DataSource = this.bindingList;

            this.control = control;

            this.processor =
                new PocsagProcessor(
                    this.control.AudioSampleRate,
                    (Pocsag.PocsagMessage message) =>
                    {
                        this.MessageReceived(message);
                    });

            this.control.RegisterStreamHook(
                this.processor,
                SDRSharp.Radio.ProcessorType.DemodulatorOutput);

            this.processor.Enabled = true;

            this.dataGridView1.AutoGenerateColumns = false;

            this.dataGridView1.DataSource = this.bindingSource;

            this.checkBoxDeDuplicate.Checked = this.deDuplicate;
            this.checkBoxHideBad.Checked = this.hideBad;

            this.checkBoxDeDuplicate.Click +=
                (object sender, EventArgs e) =>
                {
                    this.deDuplicate = this.checkBoxDeDuplicate.Checked;
                };

            this.checkBoxHideBad.Click +=
                (object sender, EventArgs e) =>
                {
                    this.hideBad = this.checkBoxHideBad.Checked;
                };

            this.buttonClear.Click +=
                (object sender, EventArgs e) =>
                {
                    this.bindingList.Clear();
                };
        }

        private void MessageReceived(Pocsag.PocsagMessage message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(
                    new Action<Pocsag.PocsagMessage>(
                        (message) =>
                        {
                            // skip duplicate messages
                            if (this.deDuplicate &&
                                    message.Payload != string.Empty &&
                                    this.bindingList.Any(x => x.Hash == message.Hash))
                            {
                                return;
                            }

                            if (this.hideBad && !message.IsValid)
                            {
                                return;
                            }

                            int firstDisplayed = this.dataGridView1.FirstDisplayedScrollingRowIndex;
                            int displayed = this.dataGridView1.DisplayedRowCount(true);
                            int lastVisible = (firstDisplayed + displayed) - 1;
                            int lastIndex = this.dataGridView1.RowCount - 1;

                            this.bindingList.Add(message);

                            while (this.bindingList.Count > 1000)
                            {
                                this.bindingList.RemoveAt(0);
                            }

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
