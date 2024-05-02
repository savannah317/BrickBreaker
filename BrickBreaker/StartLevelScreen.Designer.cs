namespace BrickBreaker
{
    partial class StartLevelScreen
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
            this.resultLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // resultLabel
            // 
            this.resultLabel.BackColor = System.Drawing.Color.Transparent;
            this.resultLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.resultLabel.Location = new System.Drawing.Point(4, -1);
            this.resultLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.resultLabel.Name = "resultLabel";
            this.resultLabel.Size = new System.Drawing.Size(361, 181);
            this.resultLabel.TabIndex = 13;
            this.resultLabel.Text = "Space To Begin";
            this.resultLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // StartLevelScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.BackgroundImage = global::BrickBreaker.Properties.Resources.minecraftBkgd;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.resultLabel);
            this.Name = "StartLevelScreen";
            this.Size = new System.Drawing.Size(369, 187);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.StartLevelScreen_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label resultLabel;
    }
}
