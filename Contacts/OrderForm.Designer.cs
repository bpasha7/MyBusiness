namespace Contacts
{
    partial class OrderForm
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
            this.gcOrdersInfo = new DevExpress.XtraGrid.GridControl();
            this.gvOrdersInfo = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colOrderInfoDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOrderInfoPayment = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOrderInfoClient = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOrderInfoName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemCheckedComboBoxEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
            this.colId = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gcOrdersInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvOrdersInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckedComboBoxEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // gcOrdersInfo
            // 
            this.gcOrdersInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcOrdersInfo.EmbeddedNavigator.TextStringFormat = "Запись {0} из {1}";
            this.gcOrdersInfo.Location = new System.Drawing.Point(0, 0);
            this.gcOrdersInfo.MainView = this.gvOrdersInfo;
            this.gcOrdersInfo.Name = "gcOrdersInfo";
            this.gcOrdersInfo.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1,
            this.repositoryItemCheckedComboBoxEdit1});
            this.gcOrdersInfo.Size = new System.Drawing.Size(749, 367);
            this.gcOrdersInfo.TabIndex = 5;
            this.gcOrdersInfo.UseEmbeddedNavigator = true;
            this.gcOrdersInfo.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvOrdersInfo});
            // 
            // gvOrdersInfo
            // 
            this.gvOrdersInfo.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colOrderInfoDate,
            this.colOrderInfoPayment,
            this.colOrderInfoClient,
            this.colOrderInfoName,
            this.colStatus,
            this.colId});
            this.gvOrdersInfo.GridControl = this.gcOrdersInfo;
            this.gvOrdersInfo.GroupPanelText = "Поступления";
            this.gvOrdersInfo.GroupSummary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum, null, this.colOrderInfoPayment, "c0", new decimal(new int[] {
                            0,
                            0,
                            0,
                            0}))});
            this.gvOrdersInfo.Name = "gvOrdersInfo";
            this.gvOrdersInfo.OptionsPrint.EnableAppearanceEvenRow = true;
            this.gvOrdersInfo.OptionsPrint.PrintDetails = true;
            this.gvOrdersInfo.OptionsPrint.PrintGroupFooter = false;
            this.gvOrdersInfo.OptionsPrint.PrintPreview = true;
            this.gvOrdersInfo.OptionsPrint.UsePrintStyles = false;
            this.gvOrdersInfo.OptionsSelection.CheckBoxSelectorColumnWidth = 20;
            this.gvOrdersInfo.OptionsSelection.MultiSelect = true;
            this.gvOrdersInfo.OptionsView.ShowFooter = true;
            this.gvOrdersInfo.RowUpdated += new DevExpress.XtraGrid.Views.Base.RowObjectEventHandler(this.gvOrdersInfo_RowUpdated);
            // 
            // colOrderInfoDate
            // 
            this.colOrderInfoDate.AppearanceCell.Options.UseTextOptions = true;
            this.colOrderInfoDate.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colOrderInfoDate.Caption = "Дата";
            this.colOrderInfoDate.DisplayFormat.FormatString = "g";
            this.colOrderInfoDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colOrderInfoDate.FieldName = "Date";
            this.colOrderInfoDate.Name = "colOrderInfoDate";
            this.colOrderInfoDate.OptionsColumn.ReadOnly = true;
            this.colOrderInfoDate.Visible = true;
            this.colOrderInfoDate.VisibleIndex = 0;
            // 
            // colOrderInfoPayment
            // 
            this.colOrderInfoPayment.AppearanceCell.Options.UseTextOptions = true;
            this.colOrderInfoPayment.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colOrderInfoPayment.Caption = "Оплата";
            this.colOrderInfoPayment.DisplayFormat.FormatString = "c0";
            this.colOrderInfoPayment.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colOrderInfoPayment.FieldName = "Payment";
            this.colOrderInfoPayment.Name = "colOrderInfoPayment";
            this.colOrderInfoPayment.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "Payment", "Всего: {0:c0}")});
            this.colOrderInfoPayment.Visible = true;
            this.colOrderInfoPayment.VisibleIndex = 2;
            // 
            // colOrderInfoClient
            // 
            this.colOrderInfoClient.Caption = "Клиент";
            this.colOrderInfoClient.FieldName = "ClientName";
            this.colOrderInfoClient.Name = "colOrderInfoClient";
            // 
            // colOrderInfoName
            // 
            this.colOrderInfoName.AppearanceCell.Options.UseTextOptions = true;
            this.colOrderInfoName.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colOrderInfoName.Caption = "Услуга";
            this.colOrderInfoName.FieldName = "PriceName";
            this.colOrderInfoName.Name = "colOrderInfoName";
            this.colOrderInfoName.OptionsColumn.ReadOnly = true;
            this.colOrderInfoName.Visible = true;
            this.colOrderInfoName.VisibleIndex = 1;
            // 
            // colStatus
            // 
            this.colStatus.Caption = "Пропуск";
            this.colStatus.ColumnEdit = this.repositoryItemCheckEdit1;
            this.colStatus.FieldName = "Left";
            this.colStatus.Name = "colStatus";
            this.colStatus.Visible = true;
            this.colStatus.VisibleIndex = 3;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            this.repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.repositoryItemCheckEdit1.Tag = true;
            // 
            // repositoryItemCheckedComboBoxEdit1
            // 
            this.repositoryItemCheckedComboBoxEdit1.AutoHeight = false;
            this.repositoryItemCheckedComboBoxEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemCheckedComboBoxEdit1.Name = "repositoryItemCheckedComboBoxEdit1";
            // 
            // colId
            // 
            this.colId.Caption = "gridColumn1";
            this.colId.FieldName = "Id";
            this.colId.Name = "colId";
            // 
            // OrderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 367);
            this.Controls.Add(this.gcOrdersInfo);
            this.Name = "OrderForm";
            this.ShowIcon = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OrderForm_FormClosing);
            this.Load += new System.EventHandler(this.OrderForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gcOrdersInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvOrdersInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckedComboBoxEdit1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gcOrdersInfo;
        private DevExpress.XtraGrid.Views.Grid.GridView gvOrdersInfo;
        private DevExpress.XtraGrid.Columns.GridColumn colOrderInfoDate;
        private DevExpress.XtraGrid.Columns.GridColumn colOrderInfoPayment;
        private DevExpress.XtraGrid.Columns.GridColumn colOrderInfoClient;
        private DevExpress.XtraGrid.Columns.GridColumn colOrderInfoName;
        private DevExpress.XtraGrid.Columns.GridColumn colStatus;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit repositoryItemCheckedComboBoxEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn colId;
    }
}