namespace GameOfLife
{
    partial class SpeedForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label lblTickRate;
            this.numTickRate = new System.Windows.Forms.NumericUpDown();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            lblTickRate = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numTickRate)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTickRate
            // 
            lblTickRate.AutoSize = true;
            lblTickRate.Location = new System.Drawing.Point(23, 25);
            lblTickRate.Name = "lblTickRate";
            lblTickRate.Size = new System.Drawing.Size(78, 15);
            lblTickRate.TabIndex = 0;
            lblTickRate.Text = "Tick rate (ms)";
            // 
            // numTickRate
            // 
            this.numTickRate.Location = new System.Drawing.Point(107, 23);
            this.numTickRate.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numTickRate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTickRate.Name = "numTickRate";
            this.numTickRate.Size = new System.Drawing.Size(120, 23);
            this.numTickRate.TabIndex = 1;
            this.numTickRate.ThousandsSeparator = true;
            this.numTickRate.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(166, 64);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(72, 64);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.Button_Click);
            // 
            // SpeedForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(253, 99);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.numTickRate);
            this.Controls.Add(lblTickRate);
            this.Name = "SpeedForm";
            this.Text = "Set simulation speed";
            ((System.ComponentModel.ISupportInitialize)(this.numTickRate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NumericUpDown numTickRate;
        private Button btnCancel;
        private Button btnOK;
    }
}