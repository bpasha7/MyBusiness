namespace Contacts
{
    partial class ServiceForm
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
            this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
            this.pricesView = new System.Windows.Forms.ListView();
            this.Id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Short = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FullName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // pricesView
            // 
            this.pricesView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Id,
            this.Short,
            this.FullName});
            this.pricesView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pricesView.FullRowSelect = true;
            this.pricesView.GridLines = true;
            this.pricesView.LabelEdit = true;
            this.pricesView.Location = new System.Drawing.Point(0, 0);
            this.pricesView.Name = "pricesView";
            this.pricesView.Size = new System.Drawing.Size(492, 221);
            this.pricesView.TabIndex = 0;
            this.pricesView.UseCompatibleStateImageBehavior = false;
            this.pricesView.View = System.Windows.Forms.View.Details;
            // 
            // Id
            // 
            this.Id.Text = "Id";
            this.Id.Width = 0;
            // 
            // Short
            // 
            this.Short.Text = "Коротко";
            this.Short.Width = 92;
            // 
            // FullName
            // 
            this.FullName.Text = "Полное название";
            this.FullName.Width = 396;
            // 
            // ServiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 221);
            this.Controls.Add(this.pricesView);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(508, 260);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(508, 260);
            this.Name = "ServiceForm";
            this.Text = "ServiceForm";
            this.Load += new System.EventHandler(this.ServiceForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
        private System.Windows.Forms.ListView pricesView;
        private System.Windows.Forms.ColumnHeader Id;
        private System.Windows.Forms.ColumnHeader Short;
        private System.Windows.Forms.ColumnHeader FullName;
    }
}