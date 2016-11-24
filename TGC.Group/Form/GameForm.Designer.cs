namespace TGC.Group.Form
{
    partial class GameForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameForm));
            this.panel3D = new System.Windows.Forms.Panel();
            this.labelTiempo = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.lblGanaPierde = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelElijeTIempo = new System.Windows.Forms.Label();
            this.labelDificultad = new System.Windows.Forms.Label();
            this.numericUpDownDificultad = new System.Windows.Forms.NumericUpDown();
            this.panel3D.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDificultad)).BeginInit();
            this.SuspendLayout();
            // 
            // panel3D
            // 
            this.panel3D.Controls.Add(this.numericUpDownDificultad);
            this.panel3D.Controls.Add(this.labelDificultad);
            this.panel3D.Controls.Add(this.labelTiempo);
            this.panel3D.Controls.Add(this.button1);
            this.panel3D.Controls.Add(this.numericUpDown1);
            this.panel3D.Controls.Add(this.lblGanaPierde);
            this.panel3D.Controls.Add(this.pictureBox1);
            this.panel3D.Controls.Add(this.labelElijeTIempo);
            this.panel3D.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3D.Location = new System.Drawing.Point(0, 0);
            this.panel3D.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel3D.Name = "panel3D";
            this.panel3D.Size = new System.Drawing.Size(1569, 1071);
            this.panel3D.TabIndex = 0;
            // 
            // labelTiempo
            // 
            this.labelTiempo.AutoSize = true;
            this.labelTiempo.Location = new System.Drawing.Point(40, 966);
            this.labelTiempo.Name = "labelTiempo";
            this.labelTiempo.Size = new System.Drawing.Size(134, 20);
            this.labelTiempo.TabIndex = 8;
            this.labelTiempo.Text = "Tiempo De Juego";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Modern No. 20", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(12, 1021);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(175, 47);
            this.button1.TabIndex = 0;
            this.button1.Text = "Jugar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(66, 989);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(82, 26);
            this.numericUpDown1.TabIndex = 6;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // lblGanaPierde
            // 
            this.lblGanaPierde.AutoSize = true;
            this.lblGanaPierde.BackColor = System.Drawing.Color.Transparent;
            this.lblGanaPierde.Font = new System.Drawing.Font("Algerian", 36F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGanaPierde.Location = new System.Drawing.Point(150, 448);
            this.lblGanaPierde.Name = "lblGanaPierde";
            this.lblGanaPierde.Size = new System.Drawing.Size(467, 80);
            this.lblGanaPierde.TabIndex = 5;
            this.lblGanaPierde.Text = "TEXTOGANA";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(566, 422);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // labelElijeTIempo
            // 
            this.labelElijeTIempo.AutoSize = true;
            this.labelElijeTIempo.Location = new System.Drawing.Point(8, 966);
            this.labelElijeTIempo.Name = "labelElijeTIempo";
            this.labelElijeTIempo.Size = new System.Drawing.Size(0, 20);
            this.labelElijeTIempo.TabIndex = 7;
            // 
            // labelDificultad
            // 
            this.labelDificultad.AutoSize = true;
            this.labelDificultad.Location = new System.Drawing.Point(23, 804);
            this.labelDificultad.Name = "labelDificultad";
            this.labelDificultad.Size = new System.Drawing.Size(75, 20);
            this.labelDificultad.TabIndex = 9;
            this.labelDificultad.Text = "Dificultad";
            // 
            // numericUpDownDificultad
            // 
            this.numericUpDownDificultad.Increment = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.numericUpDownDificultad.Location = new System.Drawing.Point(12, 827);
            this.numericUpDownDificultad.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownDificultad.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownDificultad.Name = "numericUpDownDificultad";
            this.numericUpDownDificultad.Size = new System.Drawing.Size(120, 26);
            this.numericUpDownDificultad.TabIndex = 10;
            this.numericUpDownDificultad.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1569, 1071);
            this.Controls.Add(this.panel3D);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "GameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameForm_FormClosing);
            this.Load += new System.EventHandler(this.GameForm_Load);
            this.panel3D.ResumeLayout(false);
            this.panel3D.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDificultad)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3D;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblGanaPierde;
        private System.Windows.Forms.Label labelElijeTIempo;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelTiempo;
        private System.Windows.Forms.NumericUpDown numericUpDownDificultad;
        private System.Windows.Forms.Label labelDificultad;
    }
}

