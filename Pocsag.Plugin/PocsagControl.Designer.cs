
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
            this.checkBoxDeDuplicate = new System.Windows.Forms.CheckBox();
            this.checkBoxHideBad = new System.Windows.Forms.CheckBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.Timestamp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FrameIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Function = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Bps = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HasBchErrorText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HasParityErrorText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Payload = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Timestamp,
            this.FrameIndex,
            this.Address,
            this.Function,
            this.Bps,
            this.HasBchErrorText,
            this.HasParityErrorText,
            this.Type,
            this.Payload});
            this.dataGridView1.Location = new System.Drawing.Point(0, 30);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.Size = new System.Drawing.Size(697, 381);
            this.dataGridView1.TabIndex = 0;
            // 
            // checkBoxDeDuplicate
            // 
            this.checkBoxDeDuplicate.AutoSize = true;
            this.checkBoxDeDuplicate.Location = new System.Drawing.Point(5, 5);
            this.checkBoxDeDuplicate.Name = "checkBoxDeDuplicate";
            this.checkBoxDeDuplicate.Size = new System.Drawing.Size(94, 19);
            this.checkBoxDeDuplicate.TabIndex = 1;
            this.checkBoxDeDuplicate.Text = "De-duplicate";
            this.checkBoxDeDuplicate.UseVisualStyleBackColor = true;
            // 
            // checkBoxHideBad
            // 
            this.checkBoxHideBad.AutoSize = true;
            this.checkBoxHideBad.Location = new System.Drawing.Point(106, 5);
            this.checkBoxHideBad.Name = "checkBoxHideBad";
            this.checkBoxHideBad.Size = new System.Drawing.Size(121, 19);
            this.checkBoxHideBad.TabIndex = 2;
            this.checkBoxHideBad.Text = "Hide bad decodes";
            this.checkBoxHideBad.UseVisualStyleBackColor = true;
            // 
            // buttonClear
            // 
            this.buttonClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClear.Location = new System.Drawing.Point(616, 5);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 3;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            // 
            // Timestamp
            // 
            this.Timestamp.DataPropertyName = "TimestampText";
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
            // Address
            // 
            this.Address.DataPropertyName = "Address";
            this.Address.HeaderText = "Address";
            this.Address.Name = "Address";
            this.Address.ReadOnly = true;
            this.Address.Width = 74;
            // 
            // Function
            // 
            this.Function.DataPropertyName = "Function";
            this.Function.HeaderText = "Function";
            this.Function.Name = "Function";
            this.Function.ReadOnly = true;
            this.Function.Width = 79;
            // 
            // Bps
            // 
            this.Bps.DataPropertyName = "Bps";
            this.Bps.HeaderText = "BPS";
            this.Bps.Name = "Bps";
            this.Bps.ReadOnly = true;
            this.Bps.Width = 52;
            // 
            // HasBchErrorText
            // 
            this.HasBchErrorText.DataPropertyName = "HasBchErrorText";
            this.HasBchErrorText.HeaderText = "BCH Error(s)";
            this.HasBchErrorText.Name = "HasBchErrorText";
            this.HasBchErrorText.ReadOnly = true;
            this.HasBchErrorText.Width = 97;
            // 
            // HasParityErrorText
            // 
            this.HasParityErrorText.DataPropertyName = "HasParityErrorText";
            this.HasParityErrorText.HeaderText = "Parity Error(s)";
            this.HasParityErrorText.Name = "HasParityErrorText";
            this.HasParityErrorText.ReadOnly = true;
            this.HasParityErrorText.Width = 103;
            // 
            // Type
            // 
            this.Type.DataPropertyName = "TypeText";
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            this.Type.Width = 56;
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
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.checkBoxHideBad);
            this.Controls.Add(this.checkBoxDeDuplicate);
            this.Controls.Add(this.dataGridView1);
            this.Name = "PocsagControl";
            this.Size = new System.Drawing.Size(697, 411);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.CheckBox checkBoxDeDuplicate;
        private System.Windows.Forms.CheckBox checkBoxHideBad;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.DataGridViewTextBoxColumn Timestamp;
        private System.Windows.Forms.DataGridViewTextBoxColumn FrameIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn Address;
        private System.Windows.Forms.DataGridViewTextBoxColumn Function;
        private System.Windows.Forms.DataGridViewTextBoxColumn Bps;
        private System.Windows.Forms.DataGridViewTextBoxColumn HasBchErrorText;
        private System.Windows.Forms.DataGridViewTextBoxColumn HasParityErrorText;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Payload;
    }
}
