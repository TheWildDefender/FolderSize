
namespace FolderSizes
{
	partial class FolderSizes
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
			this.dirListing = new System.Windows.Forms.ListView();
			this.nameColumn = new System.Windows.Forms.ColumnHeader();
			this.sizeColumn = new System.Windows.Forms.ColumnHeader();
			this.openMenuButton = new System.Windows.Forms.ToolStripButton();
			this.upMenuButton = new System.Windows.Forms.ToolStripButton();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.refreshButton = new System.Windows.Forms.ToolStripButton();
			this.toolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// dirListing
			// 
			this.dirListing.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dirListing.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn,
            this.sizeColumn});
			this.dirListing.FullRowSelect = true;
			this.dirListing.GridLines = true;
			this.dirListing.HideSelection = false;
			this.dirListing.Location = new System.Drawing.Point(12, 28);
			this.dirListing.Name = "dirListing";
			this.dirListing.Size = new System.Drawing.Size(316, 521);
			this.dirListing.TabIndex = 1;
			this.dirListing.UseCompatibleStateImageBehavior = false;
			this.dirListing.View = System.Windows.Forms.View.Details;
			this.dirListing.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.dirListing_ColumnClick);
			this.dirListing.ItemActivate += new System.EventHandler(this.dirListing_ItemActivate);
			// 
			// nameColumn
			// 
			this.nameColumn.Text = "Name";
			this.nameColumn.Width = 150;
			// 
			// sizeColumn
			// 
			this.sizeColumn.Text = "Size";
			this.sizeColumn.Width = 100;
			// 
			// openMenuButton
			// 
			this.openMenuButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.openMenuButton.Image = global::FolderSizes.Properties.Resources.folder;
			this.openMenuButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.openMenuButton.Name = "openMenuButton";
			this.openMenuButton.Size = new System.Drawing.Size(23, 22);
			this.openMenuButton.Text = "Open";
			this.openMenuButton.Click += new System.EventHandler(this.openMenuButton_Click);
			// 
			// upMenuButton
			// 
			this.upMenuButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.upMenuButton.Image = global::FolderSizes.Properties.Resources.up;
			this.upMenuButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.upMenuButton.Name = "upMenuButton";
			this.upMenuButton.Size = new System.Drawing.Size(23, 22);
			this.upMenuButton.Text = "Parent folder";
			this.upMenuButton.Click += new System.EventHandler(this.upMenuButton_Click);
			// 
			// toolStrip
			// 
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMenuButton,
            this.upMenuButton,
            this.refreshButton});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(340, 25);
			this.toolStrip.TabIndex = 0;
			this.toolStrip.Text = "toolStrip";
			// 
			// refreshButton
			// 
			this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.refreshButton.Image = global::FolderSizes.Properties.Resources.refresh;
			this.refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.refreshButton.Name = "refreshButton";
			this.refreshButton.Size = new System.Drawing.Size(23, 22);
			this.refreshButton.Text = "Refresh";
			this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
			// 
			// FolderSizes
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(340, 561);
			this.Controls.Add(this.dirListing);
			this.Controls.Add(this.toolStrip);
			this.Name = "FolderSizes";
			this.Text = "FolderSize";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FolderSizes_FormClosing);
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.ListView dirListing;
		private System.Windows.Forms.ColumnHeader nameColumn;
		private System.Windows.Forms.ColumnHeader sizeColumn;
		private System.Windows.Forms.ToolStripButton openMenuButton;
		private System.Windows.Forms.ToolStripButton upMenuButton;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripButton refreshButton;
	}
}
