namespace SdrsDecoder.Plugin
{
    using SDRSharp.Common;
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Linq;
    using SdrsDecoder.Support;

    public partial class PocsagControl : UserControl
    {
        private ISharpControl control;

        private PocsagProcessor processor;

        public PocsagSettings Settings { get; }

        private BindingSource bindingSource;
        private BindingList<MessageBase> bindingList;

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
            this.bindingList = new BindingList<MessageBase>();

            this.bindingSource.DataSource = this.bindingList;

            this.control = control;

            this.processor =
                new PocsagProcessor(
                    this.control.AudioSampleRate,
                    (MessageBase message) =>
                    {
                        this.MessageReceived(message);
                    });

            this.processor.ChangeMode(this.Settings.SelectedMode);

            this.control.RegisterStreamHook(
                this.processor,
                SDRSharp.Radio.ProcessorType.DemodulatorOutput);

            this.processor.Enabled = true;

            this.dataGridView1.AutoGenerateColumns = false;

            this.dataGridView1.DataSource = this.bindingSource;

            this.checkBoxDeDuplicate.Checked = this.Settings.DeDuplicate;
            this.checkBoxHideBad.Checked = this.Settings.HideBadDecodes;
            this.checkBoxMultiline.Checked = this.Settings.MultilinePayload;

            foreach (var item in Manager.ConfigSets.Select(x => x.Name))
            {
                this.modeSelector.Items.Add(item);
            }

            this.modeSelector.SelectedValue = this.Settings.SelectedMode;

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

            // prevent typing in mode selector
            this.modeSelector.KeyPress += (object sender, KeyPressEventArgs e) =>
            {
                e.Handled = true;
            };

            this.modeSelector.SelectedValueChanged += (object sender, EventArgs e) =>
            {
                var value = this.modeSelector.Text;

                this.Settings.SelectedMode = value;
                this.processor.ChangeMode(value);
            };

            this.UpdateMultilineMode();
        }

        private void MessageReceived(MessageBase message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(
                    new Action<MessageBase>(
                        (message) =>
                        {
                            // skip duplicate messages
                            if (this.Settings.DeDuplicate &&
                                    message.Payload != string.Empty &&
                                    this.bindingList.Any(x => x.Hash == message.Hash))
                            {
                                return;
                            }

                            if (this.Settings.HideBadDecodes && message.HasErrors)
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
