
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
            components = new System.ComponentModel.Container();
            dataGridView1 = new System.Windows.Forms.DataGridView();
            Timestamp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            FrameIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Function = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Bps = new System.Windows.Forms.DataGridViewTextBoxColumn();
            HasBchErrorText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            HasParityErrorText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ErrorsCorrected = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Payload = new System.Windows.Forms.DataGridViewTextBoxColumn();
            checkBoxDeDuplicate = new System.Windows.Forms.CheckBox();
            checkBoxHideBad = new System.Windows.Forms.CheckBox();
            buttonClear = new System.Windows.Forms.Button();
            checkBoxMultiline = new System.Windows.Forms.CheckBox();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Timestamp, FrameIndex, Address, Function, Bps, HasBchErrorText, HasParityErrorText, ErrorsCorrected, Type, Payload });
            dataGridView1.Location = new System.Drawing.Point(0, 30);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new System.Drawing.Size(1418, 381);
            dataGridView1.TabIndex = 0;
            // 
            // Timestamp
            // 
            Timestamp.DataPropertyName = "TimestampText";
            Timestamp.HeaderText = "Timestamp";
            Timestamp.Name = "Timestamp";
            Timestamp.ReadOnly = true;
            Timestamp.Width = 91;
            // 
            // FrameIndex
            // 
            FrameIndex.DataPropertyName = "FrameIndex";
            FrameIndex.HeaderText = "Frame Index";
            FrameIndex.Name = "FrameIndex";
            FrameIndex.ReadOnly = true;
            FrameIndex.Width = 89;
            // 
            // Address
            // 
            Address.DataPropertyName = "Address";
            Address.HeaderText = "Address";
            Address.Name = "Address";
            Address.ReadOnly = true;
            Address.Width = 74;
            // 
            // Function
            // 
            Function.DataPropertyName = "Function";
            Function.HeaderText = "Function";
            Function.Name = "Function";
            Function.ReadOnly = true;
            Function.Width = 79;
            // 
            // Bps
            // 
            Bps.DataPropertyName = "Bps";
            Bps.HeaderText = "BPS";
            Bps.Name = "Bps";
            Bps.ReadOnly = true;
            Bps.Width = 52;
            // 
            // HasBchErrorText
            // 
            HasBchErrorText.DataPropertyName = "HasBchErrorText";
            HasBchErrorText.HeaderText = "BCH Error(s)";
            HasBchErrorText.Name = "HasBchErrorText";
            HasBchErrorText.ReadOnly = true;
            HasBchErrorText.Width = 89;
            // 
            // HasParityErrorText
            // 
            HasParityErrorText.DataPropertyName = "HasParityErrorText";
            HasParityErrorText.HeaderText = "Parity Error(s)";
            HasParityErrorText.Name = "HasParityErrorText";
            HasParityErrorText.ReadOnly = true;
            HasParityErrorText.Width = 95;
            // 
            // ErrorsCorrected
            // 
            ErrorsCorrected.DataPropertyName = "ErrorsCorrected";
            ErrorsCorrected.HeaderText = "Errors Corrected";
            ErrorsCorrected.Name = "ErrorsCorrected";
            ErrorsCorrected.ReadOnly = true;
            ErrorsCorrected.Width = 107;
            // 
            // Type
            // 
            Type.DataPropertyName = "TypeText";
            Type.HeaderText = "Type";
            Type.Name = "Type";
            Type.ReadOnly = true;
            Type.Width = 56;
            // 
            // Payload
            // 
            Payload.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Payload.DataPropertyName = "Payload";
            Payload.HeaderText = "Payload";
            Payload.Name = "Payload";
            Payload.ReadOnly = true;
            // 
            // checkBoxDeDuplicate
            // 
            checkBoxDeDuplicate.AutoSize = true;
            checkBoxDeDuplicate.Location = new System.Drawing.Point(5, 5);
            checkBoxDeDuplicate.Name = "checkBoxDeDuplicate";
            checkBoxDeDuplicate.Size = new System.Drawing.Size(94, 19);
            checkBoxDeDuplicate.TabIndex = 1;
            checkBoxDeDuplicate.Text = "De-duplicate";
            checkBoxDeDuplicate.UseVisualStyleBackColor = true;
            // 
            // checkBoxHideBad
            // 
            checkBoxHideBad.AutoSize = true;
            checkBoxHideBad.Location = new System.Drawing.Point(106, 5);
            checkBoxHideBad.Name = "checkBoxHideBad";
            checkBoxHideBad.Size = new System.Drawing.Size(121, 19);
            checkBoxHideBad.TabIndex = 2;
            checkBoxHideBad.Text = "Hide bad decodes";
            checkBoxHideBad.UseVisualStyleBackColor = true;
            // 
            // buttonClear
            // 
            buttonClear.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonClear.Location = new System.Drawing.Point(1337, 5);
            buttonClear.Name = "buttonClear";
            buttonClear.Size = new System.Drawing.Size(75, 23);
            buttonClear.TabIndex = 3;
            buttonClear.Text = "Clear";
            toolTip1.SetToolTip(buttonClear, "Clear all entries from the table");
            buttonClear.UseVisualStyleBackColor = true;
            // 
            // checkBoxMultiline
            // 
            checkBoxMultiline.AutoSize = true;
            checkBoxMultiline.Location = new System.Drawing.Point(234, 5);
            checkBoxMultiline.Name = "checkBoxMultiline";
            checkBoxMultiline.Size = new System.Drawing.Size(99, 19);
            checkBoxMultiline.TabIndex = 4;
            checkBoxMultiline.Text = "Wrap payload";
            checkBoxMultiline.UseVisualStyleBackColor = true;
            // 
            // toolTip1
            // 
            toolTip1.AutomaticDelay = 0;
            // 
            // PocsagControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(checkBoxMultiline);
            Controls.Add(buttonClear);
            Controls.Add(checkBoxHideBad);
            Controls.Add(checkBoxDeDuplicate);
            Controls.Add(dataGridView1);
            Name = "PocsagControl";
            Size = new System.Drawing.Size(1418, 411);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.CheckBox checkBoxDeDuplicate;
        private System.Windows.Forms.CheckBox checkBoxHideBad;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.CheckBox checkBoxMultiline;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Timestamp;
        private System.Windows.Forms.DataGridViewTextBoxColumn FrameIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn Address;
        private System.Windows.Forms.DataGridViewTextBoxColumn Function;
        private System.Windows.Forms.DataGridViewTextBoxColumn Bps;
        private System.Windows.Forms.DataGridViewTextBoxColumn HasBchErrorText;
        private System.Windows.Forms.DataGridViewTextBoxColumn HasParityErrorText;
        private System.Windows.Forms.DataGridViewTextBoxColumn ErrorsCorrected;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Payload;
    }
}
