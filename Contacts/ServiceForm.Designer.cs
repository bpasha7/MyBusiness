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
            this.gcServices = new DevExpress.XtraGrid.GridControl();
            this.gvServices = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colShort = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colName = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcServices)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvServices)).BeginInit();
            this.SuspendLayout();
            // 
            // gcServices
            // 
            this.gcServices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcServices.Location = new System.Drawing.Point(0, 0);
            this.gcServices.MainView = this.gvServices;
            this.gcServices.Name = "gcServices";
            this.gcServices.Size = new System.Drawing.Size(492, 221);
            this.gcServices.TabIndex = 1;
            this.gcServices.UseEmbeddedNavigator = true;
            this.gcServices.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvServices});
            // 
            // gvServices
            // 
            this.gvServices.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colId,
            this.colShort,
            this.colName});
            this.gvServices.GridControl = this.gcServices;
            this.gvServices.Name = "gvServices";
            this.gvServices.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
            this.gvServices.OptionsNavigation.AutoFocusNewRow = true;
            this.gvServices.OptionsView.ShowGroupPanel = false;
            this.gvServices.RowDeleting += new DevExpress.Data.RowDeletingEventHandler(this.gvServices_RowDeleting);
            this.gvServices.RowUpdated += new DevExpress.XtraGrid.Views.Base.RowObjectEventHandler(this.gvServices_RowUpdated);
            // 
            // colId
            // 
            this.colId.Caption = "Id";
            this.colId.FieldName = "Id";
            this.colId.Name = "colId";
            this.colId.Visible = true;
            this.colId.VisibleIndex = 2;
            // 
            // colShort
            // 
            this.colShort.Caption = "Коротко";
            this.colShort.FieldName = "Short";
            this.colShort.MaxWidth = 75;
            this.colShort.Name = "colShort";
            this.colShort.Visible = true;
            this.colShort.VisibleIndex = 0;
            // 
            // colName
            // 
            this.colName.Caption = "Полное";
            this.colName.FieldName = "Name";
            this.colName.Name = "colName";
            this.colName.Visible = true;
            this.colName.VisibleIndex = 1;
            // 
            // ServiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 221);
            this.Controls.Add(this.gcServices);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(508, 260);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(508, 260);
            this.Name = "ServiceForm";
            this.Text = "ServiceForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServiceForm_FormClosing);
            this.Load += new System.EventHandler(this.ServiceForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcServices)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvServices)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
        private DevExpress.XtraGrid.GridControl gcServices;
        private DevExpress.XtraGrid.Views.Grid.GridView gvServices;
        private DevExpress.XtraGrid.Columns.GridColumn colId;
        private DevExpress.XtraGrid.Columns.GridColumn colShort;
        private DevExpress.XtraGrid.Columns.GridColumn colName;
    }
}