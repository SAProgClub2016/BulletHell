namespace BulletHell.Editor
{
    partial class LevelEditorPanel
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
            this.ViewerTimelineSplit = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.ViewerTimelineSplit)).BeginInit();
            this.ViewerTimelineSplit.SuspendLayout();
            this.SuspendLayout();
            // 
            // ViewerTimelineSplit
            // 
            this.ViewerTimelineSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ViewerTimelineSplit.Location = new System.Drawing.Point(0, 0);
            this.ViewerTimelineSplit.Name = "ViewerTimelineSplit";
            this.ViewerTimelineSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.ViewerTimelineSplit.Size = new System.Drawing.Size(1280, 720);
            this.ViewerTimelineSplit.SplitterDistance = 691;
            this.ViewerTimelineSplit.TabIndex = 0;
            // 
            // LevelEditorPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ViewerTimelineSplit);
            this.Name = "LevelEditorPanel";
            this.Size = new System.Drawing.Size(1280, 720);
            ((System.ComponentModel.ISupportInitialize)(this.ViewerTimelineSplit)).EndInit();
            this.ViewerTimelineSplit.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer ViewerTimelineSplit;


    }
}
