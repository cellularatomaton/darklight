namespace DarkLight.Server
{
    partial class DarkLightServerForm
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
            this.btnCreateManagerService = new System.Windows.Forms.Button();
            this.btnLoadStrategies = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCreateManagerService
            // 
            this.btnCreateManagerService.Location = new System.Drawing.Point(8, 13);
            this.btnCreateManagerService.Name = "btnCreateManagerService";
            this.btnCreateManagerService.Size = new System.Drawing.Size(208, 23);
            this.btnCreateManagerService.TabIndex = 4;
            this.btnCreateManagerService.Text = "Create Manager Service";
            this.btnCreateManagerService.UseVisualStyleBackColor = true;
            this.btnCreateManagerService.Click += new System.EventHandler(this.btnCreateManagerService_Click);
            // 
            // btnLoadStrategies
            // 
            this.btnLoadStrategies.Location = new System.Drawing.Point(8, 42);
            this.btnLoadStrategies.Name = "btnLoadStrategies";
            this.btnLoadStrategies.Size = new System.Drawing.Size(208, 23);
            this.btnLoadStrategies.TabIndex = 6;
            this.btnLoadStrategies.Text = "Load Strategies";
            this.btnLoadStrategies.UseVisualStyleBackColor = true;
            this.btnLoadStrategies.Click += new System.EventHandler(this.btnLoadStrategies_Click);
            // 
            // DarkLightServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(224, 80);
            this.Controls.Add(this.btnLoadStrategies);
            this.Controls.Add(this.btnCreateManagerService);
            this.Name = "DarkLightServerForm";
            this.Text = "DarkLightServer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCreateManagerService;
        private System.Windows.Forms.Button btnLoadStrategies;
    }
}

