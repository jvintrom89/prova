
namespace WindowsFormsApplication1
{
    partial class Form2
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
            this.PuntosY = new System.Windows.Forms.Label();
            this.BotonEmpezar = new System.Windows.Forms.Button();
            this.mapa = new System.Windows.Forms.PictureBox();
            this.Y = new System.Windows.Forms.Label();
            this.R = new System.Windows.Forms.Label();
            this.PuntosR = new System.Windows.Forms.Label();
            this.G = new System.Windows.Forms.Label();
            this.PuntosG = new System.Windows.Forms.Label();
            this.B = new System.Windows.Forms.Label();
            this.PuntosB = new System.Windows.Forms.Label();
            this.cronometro1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.mapa)).BeginInit();
            this.SuspendLayout();
            // 
            // PuntosY
            // 
            this.PuntosY.AutoSize = true;
            this.PuntosY.Location = new System.Drawing.Point(494, 27);
            this.PuntosY.Name = "PuntosY";
            this.PuntosY.Size = new System.Drawing.Size(14, 16);
            this.PuntosY.TabIndex = 0;
            this.PuntosY.Text = "0";
            // 
            // BotonEmpezar
            // 
            this.BotonEmpezar.Location = new System.Drawing.Point(24, 14);
            this.BotonEmpezar.Margin = new System.Windows.Forms.Padding(4);
            this.BotonEmpezar.Name = "BotonEmpezar";
            this.BotonEmpezar.Size = new System.Drawing.Size(128, 43);
            this.BotonEmpezar.TabIndex = 2;
            this.BotonEmpezar.Text = "EMPEZAR";
            this.BotonEmpezar.UseVisualStyleBackColor = true;
            this.BotonEmpezar.Click += new System.EventHandler(this.button1_Click);
            // 
            // mapa
            // 
            this.mapa.BackgroundImage = global::WindowsFormsApplication1.Properties.Resources.fondo;
            this.mapa.Location = new System.Drawing.Point(24, 63);
            this.mapa.Margin = new System.Windows.Forms.Padding(0);
            this.mapa.Name = "mapa";
            this.mapa.Size = new System.Drawing.Size(1178, 704);
            this.mapa.TabIndex = 3;
            this.mapa.TabStop = false;
            // 
            // Y
            // 
            this.Y.AutoSize = true;
            this.Y.Location = new System.Drawing.Point(388, 27);
            this.Y.Name = "Y";
            this.Y.Size = new System.Drawing.Size(59, 16);
            this.Y.TabIndex = 4;
            this.Y.Text = "Amarillo:";
            this.Y.Click += new System.EventHandler(this.label1_Click);
            // 
            // R
            // 
            this.R.AutoSize = true;
            this.R.Location = new System.Drawing.Point(552, 27);
            this.R.Name = "R";
            this.R.Size = new System.Drawing.Size(39, 16);
            this.R.TabIndex = 6;
            this.R.Text = "Rojo:";
            // 
            // PuntosR
            // 
            this.PuntosR.AutoSize = true;
            this.PuntosR.Location = new System.Drawing.Point(630, 27);
            this.PuntosR.Name = "PuntosR";
            this.PuntosR.Size = new System.Drawing.Size(14, 16);
            this.PuntosR.TabIndex = 5;
            this.PuntosR.Text = "0";
            // 
            // G
            // 
            this.G.AutoSize = true;
            this.G.Location = new System.Drawing.Point(689, 27);
            this.G.Name = "G";
            this.G.Size = new System.Drawing.Size(47, 16);
            this.G.TabIndex = 8;
            this.G.Text = "Verde:";
            // 
            // PuntosG
            // 
            this.PuntosG.AutoSize = true;
            this.PuntosG.Location = new System.Drawing.Point(770, 27);
            this.PuntosG.Name = "PuntosG";
            this.PuntosG.Size = new System.Drawing.Size(14, 16);
            this.PuntosG.TabIndex = 7;
            this.PuntosG.Text = "0";
            // 
            // B
            // 
            this.B.AutoSize = true;
            this.B.Location = new System.Drawing.Point(825, 27);
            this.B.Name = "B";
            this.B.Size = new System.Drawing.Size(35, 16);
            this.B.TabIndex = 10;
            this.B.Text = "Azul:";
            // 
            // PuntosB
            // 
            this.PuntosB.AutoSize = true;
            this.PuntosB.Location = new System.Drawing.Point(889, 27);
            this.PuntosB.Name = "PuntosB";
            this.PuntosB.Size = new System.Drawing.Size(14, 16);
            this.PuntosB.TabIndex = 9;
            this.PuntosB.Text = "0";
            // 
            // cronometro1
            // 
            this.cronometro1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cronometro1.Location = new System.Drawing.Point(1079, 14);
            this.cronometro1.Name = "cronometro1";
            this.cronometro1.Size = new System.Drawing.Size(123, 38);
            this.cronometro1.TabIndex = 12;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1292, 786);
            this.Controls.Add(this.cronometro1);
            this.Controls.Add(this.B);
            this.Controls.Add(this.PuntosB);
            this.Controls.Add(this.G);
            this.Controls.Add(this.PuntosG);
            this.Controls.Add(this.R);
            this.Controls.Add(this.PuntosR);
            this.Controls.Add(this.Y);
            this.Controls.Add(this.mapa);
            this.Controls.Add(this.BotonEmpezar);
            this.Controls.Add(this.PuntosY);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form2";
            this.Text = "Form2";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MoverJugador);
            ((System.ComponentModel.ISupportInitialize)(this.mapa)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label PuntosY;
        private System.Windows.Forms.Button BotonEmpezar;
        private System.Windows.Forms.PictureBox mapa;
        private System.Windows.Forms.Label Y;
        private System.Windows.Forms.Label R;
        private System.Windows.Forms.Label PuntosR;
        private System.Windows.Forms.Label G;
        private System.Windows.Forms.Label PuntosG;
        private System.Windows.Forms.Label B;
        private System.Windows.Forms.Label PuntosB;
        private System.Windows.Forms.Label cronometro1;
    }
}