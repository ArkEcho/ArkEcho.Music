namespace ArkEcho.Desktop
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.blazorWebView = new Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView();
            this.pictureSplash = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureSplash)).BeginInit();
            this.SuspendLayout();
            // 
            // blazorWebView
            // 
            this.blazorWebView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blazorWebView.Location = new System.Drawing.Point(0, 0);
            this.blazorWebView.Name = "blazorWebView";
            this.blazorWebView.Size = new System.Drawing.Size(2489, 1653);
            this.blazorWebView.TabIndex = 0;
            this.blazorWebView.Text = "blazorWebView";
            // 
            // pictureSplash
            // 
            this.pictureSplash.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pictureSplash.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureSplash.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureSplash.ErrorImage = ((System.Drawing.Image)(resources.GetObject("pictureSplash.ErrorImage")));
            this.pictureSplash.Image = ((System.Drawing.Image)(resources.GetObject("pictureSplash.Image")));
            this.pictureSplash.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureSplash.InitialImage")));
            this.pictureSplash.Location = new System.Drawing.Point(0, 0);
            this.pictureSplash.Name = "pictureSplash";
            this.pictureSplash.Size = new System.Drawing.Size(2489, 1653);
            this.pictureSplash.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureSplash.TabIndex = 1;
            this.pictureSplash.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2489, 1653);
            this.Controls.Add(this.pictureSplash);
            this.Controls.Add(this.blazorWebView);
            this.Name = "MainForm";
            this.Text = "ArkEcho";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureSplash)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView blazorWebView;
        private PictureBox pictureSplash;
    }
}