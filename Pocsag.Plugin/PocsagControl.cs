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

        public PocsagSettings Settings { get; }

        private BindingSource bindingSource;
        private BindingList<PocsagMessage> bindingList;

        protected DataGridViewColumn PayloadColumn => this.dataGridView1.Columns["Payload"];

        private void UpdateMultilineMode()
        {
            this.Settings.MultilinePayload = this.checkBoxMultiline.Checked;

            this.PayloadColumn.DefaultCellStyle.WrapMode =
               this.Settings.MultilinePayload ?
                    DataGridViewTriState.True :
                    DataGridViewTriState.False;

            this.PayloadColumn.AutoSizeMode =
                this.Settings.MultilinePayload ?
                    DataGridViewAutoSizeColumnMode.Fill :
                    DataGridViewAutoSizeColumnMode.NotSet;
        }

        public PocsagControl(ISharpControl control)
        {
            InitializeComponent();

            this.Settings = new PocsagSettings();

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

            this.checkBoxDeDuplicate.Checked = this.Settings.DeDuplicate;
            this.checkBoxHideBad.Checked = this.Settings.HideBadDecodes;
            this.checkBoxMultiline.Checked = this.Settings.MultilinePayload;

            this.checkBoxDeDuplicate.Click +=
                (object sender, EventArgs e) =>
                {

                    this.Settings.DeDuplicate = this.checkBoxDeDuplicate.Checked;
                };

            this.checkBoxHideBad.Click +=
                (object sender, EventArgs e) =>
                {
                    this.Settings.HideBadDecodes = this.checkBoxHideBad.Checked;
                };

            this.checkBoxMultiline.Click +=
                (object sender, EventArgs e) =>
                {
                    this.UpdateMultilineMode();
                };

            this.buttonClear.Click +=
                (object sender, EventArgs e) =>
                {
                    this.bindingList.Clear();
                };

            this.UpdateMultilineMode();

            this.pocsagFd512.Value = this.Settings.Pocsag512FilterDepth;
            this.pocsagFd1200.Value = this.Settings.Pocsag1200FilterDepth;
            this.pocsagFd2400.Value = this.Settings.Pocsag2400FilterDepth;

            this.processor.Manager.Pocsag512FilterDepth = (int)this.pocsagFd512.Value;
            this.processor.Manager.Pocsag1200FilterDepth = (int)this.pocsagFd1200.Value;
            this.processor.Manager.Pocsag2400FilterDepth = (int)this.pocsagFd2400.Value;

            this.pocsagFd512.ValueChanged +=
                (object sender, EventArgs e) =>
                {
                    var value = (int)this.pocsagFd512.Value;

                    this.Settings.Pocsag512FilterDepth = value;
                    this.processor.Manager.Pocsag512FilterDepth = value;
                };

            this.pocsagFd1200.ValueChanged +=
               (object sender, EventArgs e) =>
               {
                   var value = (int)this.pocsagFd1200.Value;

                   this.Settings.Pocsag1200FilterDepth = value;
                   this.processor.Manager.Pocsag1200FilterDepth = value;
               };

            this.pocsagFd2400.ValueChanged +=
               (object sender, EventArgs e) =>
               {
                   var value = (int)this.pocsagFd2400.Value;

                   this.Settings.Pocsag2400FilterDepth = value;
                   this.processor.Manager.Pocsag2400FilterDepth = value;
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
                            if (this.Settings.DeDuplicate &&
                                    message.Payload != string.Empty &&
                                    this.bindingList.Any(x => x.Hash == message.Hash))
                            {
                                return;
                            }

                            if (this.Settings.HideBadDecodes && !message.IsValid)
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
