namespace SdrsDecoder.Plugin
{
    using SDRSharp.Common;
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Linq;
    using SdrsDecoder.Support;
    using System.IO;

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
            this.checkBoxLogging.Checked = this.Settings.Logging;

            foreach (var item in Manager.ConfigSets.Select(x => x.Name))
            {
                this.modeSelector.Items.Add(item);
            }

            this.modeSelector.SelectedIndex = this.modeSelector.FindStringExact(this.Settings.SelectedMode);

            this.textBoxFilter.Text = this.Settings.Filter;

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

            this.checkBoxLogging.Click +=
              (object sender, EventArgs e) =>
              {
                  this.Settings.Logging = this.checkBoxLogging.Checked;
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

            this.textBoxFilter.TextChanged += (object sender, EventArgs e) =>
            {
                this.Settings.Filter = this.textBoxFilter.Text;
            };

            this.UpdateMultilineMode();
        }

        private static object LogLock = new object();

        private string CsvifyText(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return "\"\"";
            }

            var result = source;

            result = result.Replace("\r\n", " ");
            result = result.Replace("\n", " ");
            result = result.Replace("\r", " ");

            result = result.Replace("\"", "\"\"");
            result = $"\"{result}\"";

            return result;
        }

        private void LogMessage(MessageBase message, string fileNameSuffix = "")
        {
            var directory = "sdrs-log";

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filename = DateTime.Now.ToString("yyyy-MM-dd") + fileNameSuffix + ".csv";

            var path = $"{directory}/{filename}";

            lock (LogLock)
            {
                if (!File.Exists(path))
                {
                    File.WriteAllText(path, "timestamp,protocol,address,errors,type,payload\n");
                }

                var line = "";

                line += $"{CsvifyText(message.TimestampText)},";
                line += $"{CsvifyText(message.Protocol)},";
                line += $"{CsvifyText(message.Address)},";
                line += $"{CsvifyText(message.ErrorText)},";
                line += $"{CsvifyText(message.TypeText)},";
                line += $"{CsvifyText(message.Payload)}";

                File.AppendAllLines(path, new string[] { line });
            }
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

                            var filter = this.Settings.Filter;
                            var filterOn = !string.IsNullOrWhiteSpace(filter);
                            var filterMatched = false;

                            if (filterOn)
                            {
                                var filterElements = filter.Split(",");

                                foreach (var filterElement in filterElements)
                                {
                                    if (!string.IsNullOrWhiteSpace(message.Address) && message.Address.Contains(filterElement, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        filterMatched = true;
                                        break;
                                    }

                                    if (!string.IsNullOrWhiteSpace(message.Payload) && message.Payload.Contains(filterElement, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        filterMatched = true;
                                        break;
                                    }
                                }
                            }

                            var messageValidForFilter = filterOn && filterMatched;

                            int firstDisplayed = this.dataGridView1.FirstDisplayedScrollingRowIndex;
                            int displayed = this.dataGridView1.DisplayedRowCount(true);
                            int lastVisible = (firstDisplayed + displayed) - 1;
                            int lastIndex = this.dataGridView1.RowCount - 1;

                            if (messageValidForFilter || !filterOn)
                            {
                                this.bindingList.Add(message);

                                while (this.bindingList.Count > 1000)
                                {
                                    this.bindingList.RemoveAt(0);
                                }

                                if (lastVisible == lastIndex)
                                {
                                    this.dataGridView1.FirstDisplayedScrollingRowIndex = firstDisplayed + 1;
                                }
                            }
                            
                            if (this.Settings.Logging)
                            {
                                // log everything to main log
                                this.LogMessage(message);

                                // log filtered stuff to filtered log
                                if (messageValidForFilter)
                                {
                                    this.LogMessage(message, "-filtered");
                                }
                            }
                        }),
                    new object[] { message });
            }
        }
    }
}
