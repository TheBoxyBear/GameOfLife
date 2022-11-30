
namespace GameOfLife
{
    partial class OldForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripMenuItem Menu_Colors;
            this.Menu_Color_Alive = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Color_Dead = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Colors_Reset = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu = new System.Windows.Forms.MenuStrip();
            this.Menu_StartStop = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_BoardSize = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Reset = new System.Windows.Forms.ToolStripMenuItem();
            this.TableBoard = new System.Windows.Forms.TableLayoutPanel();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            Menu_Colors = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // Menu_Colors
            // 
            Menu_Colors.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Color_Alive,
            this.Menu_Color_Dead,
            this.Menu_Colors_Reset});
            Menu_Colors.Name = "Menu_Colors";
            Menu_Colors.Size = new System.Drawing.Size(53, 20);
            Menu_Colors.Text = "Colors";
            // 
            // Menu_Color_Alive
            // 
            this.Menu_Color_Alive.Name = "Menu_Color_Alive";
            this.Menu_Color_Alive.Size = new System.Drawing.Size(157, 22);
            this.Menu_Color_Alive.Text = "Alive";
            this.Menu_Color_Alive.Click += new System.EventHandler(this.Menu_Color_Alive_Click);
            // 
            // Menu_Color_Dead
            // 
            this.Menu_Color_Dead.Name = "Menu_Color_Dead";
            this.Menu_Color_Dead.Size = new System.Drawing.Size(157, 22);
            this.Menu_Color_Dead.Text = "Dead";
            this.Menu_Color_Dead.Click += new System.EventHandler(this.Menu_Color_Dead_Click);
            // 
            // Menu_Colors_Reset
            // 
            this.Menu_Colors_Reset.Name = "Menu_Colors_Reset";
            this.Menu_Colors_Reset.Size = new System.Drawing.Size(157, 22);
            this.Menu_Colors_Reset.Text = "Reset to Default";
            this.Menu_Colors_Reset.Click += new System.EventHandler(this.Menu_Colors_Reset_Click);
            // 
            // Menu
            // 
            this.Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_StartStop,
            this.Menu_Reset,
            this.Menu_BoardSize,
            Menu_Colors});
            this.Menu.Location = new System.Drawing.Point(0, 0);
            this.Menu.Name = "Menu";
            this.Menu.Size = new System.Drawing.Size(800, 24);
            this.Menu.TabIndex = 0;
            this.Menu.Text = "menuStrip1";
            // 
            // Menu_StartStop
            // 
            this.Menu_StartStop.Name = "Menu_StartStop";
            this.Menu_StartStop.Size = new System.Drawing.Size(43, 20);
            this.Menu_StartStop.Text = "Start";
            this.Menu_StartStop.Click += new System.EventHandler(this.Menu_StartStop_Click);
            // 
            // Menu_BoardSize
            // 
            this.Menu_BoardSize.Name = "Menu_BoardSize";
            this.Menu_BoardSize.Size = new System.Drawing.Size(72, 20);
            this.Menu_BoardSize.Text = "Board size";
            this.Menu_BoardSize.Click += new System.EventHandler(this.Menu_BoardSize_Click);
            // 
            // Menu_Reset
            // 
            this.Menu_Reset.Name = "Menu_Reset";
            this.Menu_Reset.Size = new System.Drawing.Size(47, 20);
            this.Menu_Reset.Text = "Reset";
            this.Menu_Reset.Click += new System.EventHandler(this.Menu_Reset_Click);
            // 
            // TableBoard
            // 
            this.TableBoard.ColumnCount = 2;
            this.TableBoard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableBoard.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableBoard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableBoard.Location = new System.Drawing.Point(0, 24);
            this.TableBoard.Name = "TableBoard";
            this.TableBoard.RowCount = 2;
            this.TableBoard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableBoard.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableBoard.Size = new System.Drawing.Size(800, 617);
            this.TableBoard.TabIndex = 1;
            // 
            // Timer
            // 
            this.Timer.Interval = 500;
            this.Timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 641);
            this.Controls.Add(this.TableBoard);
            this.Controls.Add(this.Menu);
            this.Name = "MainForm";
            this.Text = "Game of Life";
            this.Menu.ResumeLayout(false);
            this.Menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip Menu;
        private System.Windows.Forms.TableLayoutPanel TableBoard;
        private System.Windows.Forms.ToolStripMenuItem Menu_StartStop;
        private System.Windows.Forms.ToolStripMenuItem Menu_BoardSize;
        private System.Windows.Forms.ToolStripMenuItem Menu_Color_Alive;
        private System.Windows.Forms.ToolStripMenuItem Menu_Color_Dead;
        private System.Windows.Forms.ToolStripMenuItem Menu_Colors_Reset;
        private System.Windows.Forms.Timer Timer;
        private System.Windows.Forms.ToolStripMenuItem Menu_Reset;
    }
}

