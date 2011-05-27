namespace TestGUI
{
    partial class TestGUI
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
            this.btnStartStrategy = new System.Windows.Forms.Button();
            this.btnStopStrategy = new System.Windows.Forms.Button();
            this.btnGetStrategies = new System.Windows.Forms.Button();
            this.lblNumStrats = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnStartStrategy
            // 
            this.btnStartStrategy.Location = new System.Drawing.Point(12, 115);
            this.btnStartStrategy.Name = "btnStartStrategy";
            this.btnStartStrategy.Size = new System.Drawing.Size(129, 23);
            this.btnStartStrategy.TabIndex = 0;
            this.btnStartStrategy.Text = "Start Strategy";
            this.btnStartStrategy.UseVisualStyleBackColor = true;
            this.btnStartStrategy.Click += new System.EventHandler(this.btnStartStrategy_Click);
            // 
            // btnStopStrategy
            // 
            this.btnStopStrategy.Location = new System.Drawing.Point(12, 144);
            this.btnStopStrategy.Name = "btnStopStrategy";
            this.btnStopStrategy.Size = new System.Drawing.Size(129, 23);
            this.btnStopStrategy.TabIndex = 1;
            this.btnStopStrategy.Text = "Stop Strategy";
            this.btnStopStrategy.UseVisualStyleBackColor = true;
            this.btnStopStrategy.Click += new System.EventHandler(this.btnStopStrategy_Click);
            // 
            // btnGetStrategies
            // 
            this.btnGetStrategies.Location = new System.Drawing.Point(13, 13);
            this.btnGetStrategies.Name = "btnGetStrategies";
            this.btnGetStrategies.Size = new System.Drawing.Size(128, 23);
            this.btnGetStrategies.TabIndex = 2;
            this.btnGetStrategies.Text = "Get Strategies";
            this.btnGetStrategies.UseVisualStyleBackColor = true;
            this.btnGetStrategies.Click += new System.EventHandler(this.btnGetStrategies_Click);
            // 
            // lblNumStrats
            // 
            this.lblNumStrats.AutoSize = true;
            this.lblNumStrats.Location = new System.Drawing.Point(22, 66);
            this.lblNumStrats.Name = "lblNumStrats";
            this.lblNumStrats.Size = new System.Drawing.Size(100, 13);
            this.lblNumStrats.TabIndex = 3;
            this.lblNumStrats.Text = "Number Strategies: ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(157, 179);
            this.Controls.Add(this.lblNumStrats);
            this.Controls.Add(this.btnGetStrategies);
            this.Controls.Add(this.btnStopStrategy);
            this.Controls.Add(this.btnStartStrategy);
            this.Name = "TestGUI";
            this.Text = "Test GUI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStartStrategy;
        private System.Windows.Forms.Button btnStopStrategy;
        private System.Windows.Forms.Button btnGetStrategies;
        private System.Windows.Forms.Label lblNumStrats;
    }
}

