namespace WarshipsFormClient
{
	partial class Form1
	{
		/// <summary>
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
			this.StartGameBtn = new System.Windows.Forms.Button();
			this.TurnShipBtn = new System.Windows.Forms.Button();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.radioButton3 = new System.Windows.Forms.RadioButton();
			this.radioButton4 = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.ShipsLabel1 = new System.Windows.Forms.Label();
			this.ShipsLabel2 = new System.Windows.Forms.Label();
			this.ShipsLabel3 = new System.Windows.Forms.Label();
			this.ShipsLabel4 = new System.Windows.Forms.Label();
			this.ChooseShip = new System.Windows.Forms.GroupBox();
			this.ShipDirLabel = new System.Windows.Forms.Label();
			this.ShipDirection = new System.Windows.Forms.Label();
			this.ServerMessages = new System.Windows.Forms.Label();
			this.ChooseShip.SuspendLayout();
			this.SuspendLayout();
			// 
			// StartGameBtn
			// 
			this.StartGameBtn.Location = new System.Drawing.Point(1064, 510);
			this.StartGameBtn.Name = "StartGameBtn";
			this.StartGameBtn.Size = new System.Drawing.Size(106, 31);
			this.StartGameBtn.TabIndex = 0;
			this.StartGameBtn.Text = "Вперёд";
			this.StartGameBtn.UseVisualStyleBackColor = true;
			this.StartGameBtn.Click += new System.EventHandler(this.StartGameBtn_Click);
			// 
			// TurnShipBtn
			// 
			this.TurnShipBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.TurnShipBtn.Location = new System.Drawing.Point(951, 78);
			this.TurnShipBtn.Name = "TurnShipBtn";
			this.TurnShipBtn.Size = new System.Drawing.Size(96, 86);
			this.TurnShipBtn.TabIndex = 1;
			this.TurnShipBtn.Text = "⟳";
			this.TurnShipBtn.UseVisualStyleBackColor = true;
			this.TurnShipBtn.Click += new System.EventHandler(this.TurnShipBtn_Click);
			// 
			// radioButton1
			// 
			this.radioButton1.AutoSize = true;
			this.radioButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.radioButton1.Location = new System.Drawing.Point(28, 62);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(69, 50);
			this.radioButton1.TabIndex = 2;
			this.radioButton1.TabStop = true;
			this.radioButton1.Text = "■";
			this.radioButton1.UseVisualStyleBackColor = true;
			this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
			// 
			// radioButton2
			// 
			this.radioButton2.AutoSize = true;
			this.radioButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.radioButton2.Location = new System.Drawing.Point(28, 129);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(97, 50);
			this.radioButton2.TabIndex = 3;
			this.radioButton2.TabStop = true;
			this.radioButton2.Text = "■■";
			this.radioButton2.UseVisualStyleBackColor = true;
			this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
			// 
			// radioButton3
			// 
			this.radioButton3.AutoSize = true;
			this.radioButton3.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.radioButton3.Location = new System.Drawing.Point(28, 198);
			this.radioButton3.Name = "radioButton3";
			this.radioButton3.Size = new System.Drawing.Size(125, 50);
			this.radioButton3.TabIndex = 4;
			this.radioButton3.TabStop = true;
			this.radioButton3.Text = "■■■";
			this.radioButton3.UseVisualStyleBackColor = true;
			this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
			// 
			// radioButton4
			// 
			this.radioButton4.AutoSize = true;
			this.radioButton4.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.radioButton4.Location = new System.Drawing.Point(28, 264);
			this.radioButton4.Name = "radioButton4";
			this.radioButton4.Size = new System.Drawing.Size(153, 50);
			this.radioButton4.TabIndex = 5;
			this.radioButton4.TabStop = true;
			this.radioButton4.Text = "■■■■";
			this.radioButton4.UseVisualStyleBackColor = true;
			this.radioButton4.CheckedChanged += new System.EventHandler(this.radioButton4_CheckedChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(196, 43);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(151, 17);
			this.label1.TabIndex = 6;
			this.label1.Text = "Осталось разместить";
			// 
			// ShipsLabel1
			// 
			this.ShipsLabel1.AutoSize = true;
			this.ShipsLabel1.Location = new System.Drawing.Point(196, 77);
			this.ShipsLabel1.Name = "ShipsLabel1";
			this.ShipsLabel1.Size = new System.Drawing.Size(16, 17);
			this.ShipsLabel1.TabIndex = 7;
			this.ShipsLabel1.Text = "4";
			// 
			// ShipsLabel2
			// 
			this.ShipsLabel2.AutoSize = true;
			this.ShipsLabel2.Location = new System.Drawing.Point(196, 144);
			this.ShipsLabel2.Name = "ShipsLabel2";
			this.ShipsLabel2.Size = new System.Drawing.Size(16, 17);
			this.ShipsLabel2.TabIndex = 8;
			this.ShipsLabel2.Text = "3";
			// 
			// ShipsLabel3
			// 
			this.ShipsLabel3.AutoSize = true;
			this.ShipsLabel3.Location = new System.Drawing.Point(196, 211);
			this.ShipsLabel3.Name = "ShipsLabel3";
			this.ShipsLabel3.Size = new System.Drawing.Size(16, 17);
			this.ShipsLabel3.TabIndex = 9;
			this.ShipsLabel3.Text = "2";
			// 
			// ShipsLabel4
			// 
			this.ShipsLabel4.AutoSize = true;
			this.ShipsLabel4.Location = new System.Drawing.Point(196, 276);
			this.ShipsLabel4.Name = "ShipsLabel4";
			this.ShipsLabel4.Size = new System.Drawing.Size(16, 17);
			this.ShipsLabel4.TabIndex = 10;
			this.ShipsLabel4.Text = "1";
			// 
			// ChooseShip
			// 
			this.ChooseShip.Controls.Add(this.ShipsLabel4);
			this.ChooseShip.Controls.Add(this.radioButton2);
			this.ChooseShip.Controls.Add(this.ShipsLabel3);
			this.ChooseShip.Controls.Add(this.radioButton1);
			this.ChooseShip.Controls.Add(this.ShipsLabel2);
			this.ChooseShip.Controls.Add(this.radioButton3);
			this.ChooseShip.Controls.Add(this.ShipsLabel1);
			this.ChooseShip.Controls.Add(this.radioButton4);
			this.ChooseShip.Controls.Add(this.label1);
			this.ChooseShip.Location = new System.Drawing.Point(713, 186);
			this.ChooseShip.Name = "ChooseShip";
			this.ChooseShip.Size = new System.Drawing.Size(416, 318);
			this.ChooseShip.TabIndex = 11;
			this.ChooseShip.TabStop = false;
			this.ChooseShip.Text = "Выберите корабль";
			// 
			// ShipDirLabel
			// 
			this.ShipDirLabel.AutoSize = true;
			this.ShipDirLabel.Location = new System.Drawing.Point(710, 9);
			this.ShipDirLabel.Name = "ShipDirLabel";
			this.ShipDirLabel.Size = new System.Drawing.Size(156, 17);
			this.ShipDirLabel.TabIndex = 12;
			this.ShipDirLabel.Text = "Направление корабля";
			// 
			// ShipDirection
			// 
			this.ShipDirection.AutoSize = true;
			this.ShipDirection.Font = new System.Drawing.Font("Microsoft Sans Serif", 25.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.ShipDirection.Location = new System.Drawing.Point(755, 78);
			this.ShipDirection.Name = "ShipDirection";
			this.ShipDirection.Size = new System.Drawing.Size(65, 51);
			this.ShipDirection.TabIndex = 13;
			this.ShipDirection.Text = "→";
			// 
			// ServerMessages
			// 
			this.ServerMessages.AutoSize = true;
			this.ServerMessages.Location = new System.Drawing.Point(601, 9);
			this.ServerMessages.Name = "ServerMessages";
			this.ServerMessages.Size = new System.Drawing.Size(103, 17);
			this.ServerMessages.TabIndex = 14;
			this.ServerMessages.Text = "Basic message";
			this.ServerMessages.Visible = false;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1182, 553);
			this.Controls.Add(this.ServerMessages);
			this.Controls.Add(this.ShipDirection);
			this.Controls.Add(this.ShipDirLabel);
			this.Controls.Add(this.TurnShipBtn);
			this.Controls.Add(this.StartGameBtn);
			this.Controls.Add(this.ChooseShip);
			this.DoubleBuffered = true;
			this.Name = "Form1";
			this.Text = "Морской бой";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
			this.ChooseShip.ResumeLayout(false);
			this.ChooseShip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button StartGameBtn;
		private System.Windows.Forms.Button TurnShipBtn;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.RadioButton radioButton3;
		private System.Windows.Forms.RadioButton radioButton4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label ShipsLabel1;
		private System.Windows.Forms.Label ShipsLabel2;
		private System.Windows.Forms.Label ShipsLabel3;
		private System.Windows.Forms.Label ShipsLabel4;
		private System.Windows.Forms.GroupBox ChooseShip;
		private System.Windows.Forms.Label ShipDirLabel;
		private System.Windows.Forms.Label ShipDirection;
		private System.Windows.Forms.Label ServerMessages;
	}
}

