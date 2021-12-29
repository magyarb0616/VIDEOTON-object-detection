
namespace MLNET_ObjectDetection_WinForms
{
    partial class Form1
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
			this.btnSelectImage = new System.Windows.Forms.Button();
			this.fileDialog = new System.Windows.Forms.OpenFileDialog();
			this.picPrediction = new System.Windows.Forms.PictureBox();
			this.btnNewPrediction = new System.Windows.Forms.Button();
			this.CameraList_comboBox = new System.Windows.Forms.ComboBox();
			this.Satart_button = new System.Windows.Forms.Button();
			this.videoSourcePlayer1 = new AForge.Controls.VideoSourcePlayer();
			this.Snapshot_button = new System.Windows.Forms.Button();
			this.lbl_prediction = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.picPrediction)).BeginInit();
			this.SuspendLayout();
			// 
			// btnSelectImage
			// 
			this.btnSelectImage.Location = new System.Drawing.Point(15, 936);
			this.btnSelectImage.Margin = new System.Windows.Forms.Padding(6);
			this.btnSelectImage.Name = "btnSelectImage";
			this.btnSelectImage.Size = new System.Drawing.Size(192, 50);
			this.btnSelectImage.TabIndex = 0;
			this.btnSelectImage.Text = "Select Image";
			this.btnSelectImage.UseVisualStyleBackColor = true;
			// 
			// fileDialog
			// 
			this.fileDialog.FileName = "file";
			this.fileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.fileDialog_FileOk);
			// 
			// picPrediction
			// 
			this.picPrediction.Location = new System.Drawing.Point(744, 114);
			this.picPrediction.Margin = new System.Windows.Forms.Padding(6);
			this.picPrediction.Name = "picPrediction";
			this.picPrediction.Size = new System.Drawing.Size(720, 540);
			this.picPrediction.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picPrediction.TabIndex = 1;
			this.picPrediction.TabStop = false;
			// 
			// btnNewPrediction
			// 
			this.btnNewPrediction.Location = new System.Drawing.Point(219, 938);
			this.btnNewPrediction.Margin = new System.Windows.Forms.Padding(6);
			this.btnNewPrediction.Name = "btnNewPrediction";
			this.btnNewPrediction.Size = new System.Drawing.Size(192, 46);
			this.btnNewPrediction.TabIndex = 2;
			this.btnNewPrediction.Text = "New Prediction";
			this.btnNewPrediction.UseVisualStyleBackColor = true;
			this.btnNewPrediction.Click += new System.EventHandler(this.btnNewPrediction_Click);
			// 
			// CameraList_comboBox
			// 
			this.CameraList_comboBox.FormattingEnabled = true;
			this.CameraList_comboBox.Location = new System.Drawing.Point(15, 50);
			this.CameraList_comboBox.Name = "CameraList_comboBox";
			this.CameraList_comboBox.Size = new System.Drawing.Size(249, 33);
			this.CameraList_comboBox.TabIndex = 3;
			// 
			// Satart_button
			// 
			this.Satart_button.Location = new System.Drawing.Point(270, 50);
			this.Satart_button.Name = "Satart_button";
			this.Satart_button.Size = new System.Drawing.Size(145, 36);
			this.Satart_button.TabIndex = 4;
			this.Satart_button.Text = "Start";
			this.Satart_button.UseVisualStyleBackColor = true;
			this.Satart_button.Click += new System.EventHandler(this.Satart_button_Click_1);
			// 
			// videoSourcePlayer1
			// 
			this.videoSourcePlayer1.Location = new System.Drawing.Point(15, 114);
			this.videoSourcePlayer1.Name = "videoSourcePlayer1";
			this.videoSourcePlayer1.Size = new System.Drawing.Size(720, 540);
			this.videoSourcePlayer1.TabIndex = 5;
			this.videoSourcePlayer1.Text = "videoSourcePlayer1";
			this.videoSourcePlayer1.VideoSource = null;
			this.videoSourcePlayer1.NewFrame += new AForge.Controls.VideoSourcePlayer.NewFrameHandler(this.videoSourcePlayer1_NewFrame);
			// 
			// Snapshot_button
			// 
			this.Snapshot_button.Location = new System.Drawing.Point(421, 50);
			this.Snapshot_button.Name = "Snapshot_button";
			this.Snapshot_button.Size = new System.Drawing.Size(186, 36);
			this.Snapshot_button.TabIndex = 7;
			this.Snapshot_button.Text = "Snapshot";
			this.Snapshot_button.UseVisualStyleBackColor = true;
			this.Snapshot_button.Click += new System.EventHandler(this.Snapshot_button_Click_1);
			// 
			// lbl_prediction
			// 
			this.lbl_prediction.AutoSize = true;
			this.lbl_prediction.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.lbl_prediction.Location = new System.Drawing.Point(1175, 35);
			this.lbl_prediction.Name = "lbl_prediction";
			this.lbl_prediction.Size = new System.Drawing.Size(137, 73);
			this.lbl_prediction.TabIndex = 8;
			this.lbl_prediction.Text = "___";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label2.Location = new System.Drawing.Point(833, 35);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(336, 73);
			this.label2.TabIndex = 9;
			this.label2.Text = "Prediction:";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(1497, 668);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lbl_prediction);
			this.Controls.Add(this.Snapshot_button);
			this.Controls.Add(this.videoSourcePlayer1);
			this.Controls.Add(this.Satart_button);
			this.Controls.Add(this.CameraList_comboBox);
			this.Controls.Add(this.btnNewPrediction);
			this.Controls.Add(this.picPrediction);
			this.Controls.Add(this.btnSelectImage);
			this.Margin = new System.Windows.Forms.Padding(6);
			this.Name = "Form1";
			this.Text = "Form1";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.picPrediction)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectImage;
        private System.Windows.Forms.OpenFileDialog fileDialog;
        private System.Windows.Forms.PictureBox picPrediction;
        private System.Windows.Forms.Button btnNewPrediction;
		private System.Windows.Forms.Button Satart_button;
		private System.Windows.Forms.ComboBox CameraList_comboBox;
		private System.Windows.Forms.Button Snapshot_button;
		private AForge.Controls.VideoSourcePlayer videoSourcePlayer1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lbl_prediction;
	}
}

