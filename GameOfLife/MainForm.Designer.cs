namespace GameOfLife
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripMenuItem Menu_Colors;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
            this.Menu_Color_Alive = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Color_Alive_Custom = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Color_Dead = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Color_Dead_Custom = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Color_Grid = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.Menu_Color_Grid_Custom = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Colors_Reset = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu = new System.Windows.Forms.MenuStrip();
            this.Menu_StartStop = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Reset = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_BoardSize = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_ShowGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.panBoard = new System.Windows.Forms.Panel();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.Status = new System.Windows.Forms.StatusStrip();
            this.Status_State = new System.Windows.Forms.ToolStripStatusLabel();
            Menu_Colors = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.Menu.SuspendLayout();
            this.Status.SuspendLayout();
            this.SuspendLayout();
            // 
            // Menu_Colors
            // 
            Menu_Colors.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Color_Alive,
            this.Menu_Color_Dead,
            this.Menu_Color_Grid,
            this.Menu_Colors_Reset});
            Menu_Colors.Name = "Menu_Colors";
            Menu_Colors.Size = new System.Drawing.Size(53, 20);
            Menu_Colors.Text = "Colors";
            // 
            // Menu_Color_Alive
            // 
            this.Menu_Color_Alive.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripSeparator1,
            this.Menu_Color_Alive_Custom});
            this.Menu_Color_Alive.Name = "Menu_Color_Alive";
            this.Menu_Color_Alive.Size = new System.Drawing.Size(157, 22);
            this.Menu_Color_Alive.Text = "Alive";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(122, 6);
            // 
            // Menu_Color_Alive_Custom
            // 
            this.Menu_Color_Alive_Custom.Name = "Menu_Color_Alive_Custom";
            this.Menu_Color_Alive_Custom.Size = new System.Drawing.Size(125, 22);
            this.Menu_Color_Alive_Custom.Text = "Custom...";
            this.Menu_Color_Alive_Custom.Click += new System.EventHandler(this.Menu_Color_Alive_Custom_Click);
            // 
            // Menu_Color_Dead
            // 
            this.Menu_Color_Dead.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripSeparator2,
            this.Menu_Color_Dead_Custom});
            this.Menu_Color_Dead.Name = "Menu_Color_Dead";
            this.Menu_Color_Dead.Size = new System.Drawing.Size(157, 22);
            this.Menu_Color_Dead.Text = "Dead";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(122, 6);
            // 
            // Menu_Color_Dead_Custom
            // 
            this.Menu_Color_Dead_Custom.Name = "Menu_Color_Dead_Custom";
            this.Menu_Color_Dead_Custom.Size = new System.Drawing.Size(125, 22);
            this.Menu_Color_Dead_Custom.Text = "Custom...";
            this.Menu_Color_Dead_Custom.Click += new System.EventHandler(this.Menu_Color_Dead_Custom_Click);
            // 
            // Menu_Color_Grid
            // 
            this.Menu_Color_Grid.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator3,
            this.Menu_Color_Grid_Custom});
            this.Menu_Color_Grid.Name = "Menu_Color_Grid";
            this.Menu_Color_Grid.Size = new System.Drawing.Size(157, 22);
            this.Menu_Color_Grid.Text = "Grid";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(122, 6);
            // 
            // Menu_Color_Grid_Custom
            // 
            this.Menu_Color_Grid_Custom.Name = "Menu_Color_Grid_Custom";
            this.Menu_Color_Grid_Custom.Size = new System.Drawing.Size(125, 22);
            this.Menu_Color_Grid_Custom.Text = "Custom...";
            this.Menu_Color_Grid_Custom.Click += new System.EventHandler(this.Menu_Color_Grid_Custom_Click);
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
            Menu_Colors,
            this.Menu_ShowGrid});
            this.Menu.Location = new System.Drawing.Point(0, 0);
            this.Menu.Name = "Menu";
            this.Menu.Size = new System.Drawing.Size(686, 24);
            this.Menu.TabIndex = 1;
            this.Menu.Text = "menuStrip1";
            // 
            // Menu_StartStop
            // 
            this.Menu_StartStop.Name = "Menu_StartStop";
            this.Menu_StartStop.Size = new System.Drawing.Size(43, 20);
            this.Menu_StartStop.Text = "Start";
            this.Menu_StartStop.Click += new System.EventHandler(this.Menu_StartStop_Click);
            // 
            // Menu_Reset
            // 
            this.Menu_Reset.Name = "Menu_Reset";
            this.Menu_Reset.Size = new System.Drawing.Size(47, 20);
            this.Menu_Reset.Text = "Reset";
            this.Menu_Reset.Click += new System.EventHandler(this.Menu_Reset_Click);
            // 
            // Menu_BoardSize
            // 
            this.Menu_BoardSize.Name = "Menu_BoardSize";
            this.Menu_BoardSize.Size = new System.Drawing.Size(72, 20);
            this.Menu_BoardSize.Text = "Board size";
            this.Menu_BoardSize.Click += new System.EventHandler(this.Menu_BoardSize_Click);
            // 
            // Menu_ShowGrid
            // 
            this.Menu_ShowGrid.Checked = true;
            this.Menu_ShowGrid.CheckOnClick = true;
            this.Menu_ShowGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Menu_ShowGrid.Name = "Menu_ShowGrid";
            this.Menu_ShowGrid.Size = new System.Drawing.Size(85, 20);
            this.Menu_ShowGrid.Text = "Show Grid ✓";
            this.Menu_ShowGrid.Click += new System.EventHandler(this.Menu_ShowGrid_Click);
            // 
            // panBoard
            // 
            this.panBoard.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panBoard.BackColor = System.Drawing.SystemColors.Control;
            this.panBoard.Location = new System.Drawing.Point(0, 24);
            this.panBoard.Margin = new System.Windows.Forms.Padding(0);
            this.panBoard.Name = "panBoard";
            this.panBoard.Size = new System.Drawing.Size(686, 415);
            this.panBoard.TabIndex = 2;
            this.panBoard.Paint += new System.Windows.Forms.PaintEventHandler(this.panBoard_Paint);
            this.panBoard.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panBoard_MouseDown);
            this.panBoard.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panBoard_MouseUp);
            // 
            // Timer
            // 
            this.Timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // Status
            // 
            this.Status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Status_State});
            this.Status.Location = new System.Drawing.Point(0, 439);
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(686, 22);
            this.Status.TabIndex = 3;
            this.Status.Text = "statusStrip1";
            // 
            // Status_State
            // 
            this.Status_State.Name = "Status_State";
            this.Status_State.Size = new System.Drawing.Size(76, 17);
            this.Status_State.Text = "Gen: 0 Pop: 0";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 461);
            this.Controls.Add(this.Status);
            this.Controls.Add(this.panBoard);
            this.Controls.Add(this.Menu);
            this.MainMenuStrip = this.Menu;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Game of Life";
            this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
            this.Menu.ResumeLayout(false);
            this.Menu.PerformLayout();
            this.Status.ResumeLayout(false);
            this.Status.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Panel panBoard;
        private ToolStripMenuItem Menu_StartStop;
        private ToolStripMenuItem Menu_Reset;
        private ToolStripMenuItem Menu_BoardSize;
        private ToolStripMenuItem Menu_Color_Alive;
        private ToolStripMenuItem Menu_Color_Dead;
        private ToolStripMenuItem Menu_Colors_Reset;
        private MenuStrip Menu;
        private ToolStripMenuItem Menu_Color_Alive_Custom;
        private ToolStripMenuItem Menu_Color_Dead_Custom;
        private System.Windows.Forms.Timer Timer;
        private ToolStripMenuItem Menu_ShowGrid;
        private ToolStripMenuItem Menu_Color_Grid;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem Menu_Color_Grid_Custom;
        private StatusStrip Status;
        private ToolStripStatusLabel Status_State;
    }
}