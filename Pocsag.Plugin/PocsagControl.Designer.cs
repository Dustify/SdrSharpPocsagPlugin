
namespace Pocsag.Plugin
{
    partial class PocsagControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Timestamp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FrameIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChannelAccessProtocolCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Function = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Baud = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Payload = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Timestamp,
            this.FrameIndex,
            this.ChannelAccessProtocolCode,
            this.Function,
            this.Baud,
            this.Payload});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.Size = new System.Drawing.Size(697, 411);
            this.dataGridView1.TabIndex = 0;
            // 
            // Timestamp
            // 
            this.Timestamp.DataPropertyName = "Timestamp";
            this.Timestamp.HeaderText = "Timestamp";
            this.Timestamp.Name = "Timestamp";
            this.Timestamp.ReadOnly = true;
            this.Timestamp.Width = 91;
            // 
            // FrameIndex
            // 
            this.FrameIndex.DataPropertyName = "FrameIndex";
            this.FrameIndex.HeaderText = "Frame Index";
            this.FrameIndex.Name = "FrameIndex";
            this.FrameIndex.ReadOnly = true;
            this.FrameIndex.Width = 97;
            // 
            // ChannelAccessProtocolCode
            // 
            this.ChannelAccessProtocolCode.DataPropertyName = "ChannelAccessProtocolCode";
            this.ChannelAccessProtocolCode.HeaderText = "CAP Code";
            this.ChannelAccessProtocolCode.Name = "ChannelAccessProtocolCode";
            this.ChannelAccessProtocolCode.ReadOnly = true;
            this.ChannelAccessProtocolCode.Width = 86;
            // 
            // Function
            // 
            this.Function.DataPropertyName = "Function";
            this.Function.HeaderText = "Function";
            this.Function.Name = "Function";
            this.Function.ReadOnly = true;
            this.Function.Width = 79;
            // 
            // Baud
            // 
            this.Baud.DataPropertyName = "Baud";
            this.Baud.HeaderText = "BPS";
            this.Baud.Name = "Baud";
            this.Baud.ReadOnly = true;
            this.Baud.Width = 52;
            // 
            // Payload
            // 
            this.Payload.DataPropertyName = "Payload";
            this.Payload.HeaderText = "Payload";
            this.Payload.Name = "Payload";
            this.Payload.ReadOnly = true;
            this.Payload.Width = 74;
            // 
            // PocsagControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView1);
            this.Name = "PocsagControl";
            this.Size = new System.Drawing.Size(697, 411);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Timestamp;
        private System.Windows.Forms.DataGridViewTextBoxColumn FrameIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChannelAccessProtocolCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn Function;
        private System.Windows.Forms.DataGridViewTextBoxColumn Baud;
        private System.Windows.Forms.DataGridViewTextBoxColumn Payload;
    }
}
