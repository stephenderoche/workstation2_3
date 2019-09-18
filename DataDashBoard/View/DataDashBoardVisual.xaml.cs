﻿namespace DataDashBoard.Client.View
{
    using System;
    using System.Data;
    using System.Windows;
    using System.Windows.Controls;
    using SalesSharedContracts;

    using Linedata.Framework.Foundation;
    using Linedata.Shared.Api.ServiceModel;
    using DevExpress.Xpf.Grid;
  
    using DevExpress.Xpf.Bars;
 
    using DevExpress.Xpf.Editors;
  
  
    using DataDashBoard.Client.ViewModel;
    using System.IO;
    using System.Threading;
  
     using DataDashBoard.Client;
  


    public partial class DataDashBoardVisual : UserControl
    {
       
  
       // private string MSGBOX_TITLE_ERROR = "Generic Viewer";
      
         string subPath = "c:\\dashboard";
         string file = "DataDashBoard.xaml";
         private const string MSGBOX_TITLE_ERROR = "DataDashBoard";
        
         public DataDashBoardModel _view;


         # region Parameters

        string _xml;
        public string XML
        {
            set { _xml = value; }
            get { return _xml; }
        }
         private DataSet m_AllAccounts;
         public DataSet M_AllAccounts
         {
             set { m_AllAccounts = value; }
             get { return m_AllAccounts; }
         }

         private DataSet _allSecurities;
         public DataSet AllSecurities
         {
             set { _allSecurities = value; }
             get { return _allSecurities; }
         }

       
       


         private Int32 _selectedAccountId = -1;
         public Int32 SelectedAccountId
         {
             set { _selectedAccountId = value; }
             get { return _selectedAccountId; }
         }

         private Int32 _orderid = -1;
         public Int32 Orderid
         {
             set { _orderid = value; }
             get { return _orderid; }
         }

        

         private string _selectAccountName = string.Empty;
         public string SelectAccountName
         {
             set { _selectAccountName = value; }
             get { return _selectAccountName; }
         }
         # endregion Parameters

         public DataDashBoardVisual(DataDashBoardModel ViewerModel)
         {


             InitializeComponent();
             this.DataContext = ViewerModel;
             this._view = ViewerModel;
             
            
             GetSecurities();
         
             this.comboBoxEdit1.Text = _view.AccountName;



            
          
             DevExpress.Xpf.Grid.GridControl.AllowInfiniteGridSize = true;

             AssignXML();

         }

       



        # region Account

        public bool Validate(string ourTextBox)
        {
            bool retval = false;

            if (!String.IsNullOrEmpty(ourTextBox))
            {
                try
                {
                    ThreadPool.QueueUserWorkItem(
                        delegate(object eventArg)
                        {
                            int defaultAccountId = -1;
                            string defaultShortName = "";
                            try
                            {
                                _view.DBService.ValidateAccountForUser(ourTextBox, out defaultAccountId, out defaultShortName);
                                if (defaultAccountId != -1)
                                {

                                    _view.AccountId = defaultAccountId;
                                    _view.AccountName = defaultShortName;
                                    _view.Parameters.AccountName = _view.AccountName;


                                    _view.Get_ChartData(Convert.ToInt32(_view.AccountId));
                                    retval = true;
                                }

                                else
                                {
                                    _view.AccountId = -1;
                                    _view.AccountName = "";

                                }
                            }
                            catch (Exception ex)
                            {
                                _view.AccountId = -1;
                                _view.AccountName = "";
                                throw ex;
                            }

                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {

                            }), System.Windows.Threading.DispatcherPriority.Normal);

                        });
                }

                catch (Exception ex)
                {
                    _view.AccountId = -1;
                    _view.AccountName = "";
                    throw ex;
                }

              

                return retval;
            }
            else
            {
                _view.AccountId = -1;
                _view.AccountName = "";
            }

            return retval;
        }

        private void comboBoxEdit1_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            string textEnteredPlusNew = "short_name Like '" + comboBoxEdit1.Text + "%'";
            this.comboBoxEdit1.Items.Clear();
            int count = -1;
            ThreadPool.QueueUserWorkItem(
                delegate(object eventArg)
                {
                    var someObject = m_AllAccounts;
                    if (someObject == null)
                    {
                        _view.GetAcc();
                        return;
                    }
                    foreach (DataRow row in m_AllAccounts.Tables[0].Select(textEnteredPlusNew))
                    {

                        object item = row["short_name"];
                        object account_id = row["account_id"];

                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            comboBoxEdit1.Items.Add(new AccountItem(Convert.ToString(item), Convert.ToInt64(account_id)));



                        }), System.Windows.Threading.DispatcherPriority.Normal);

                        count = count + 1;
                        if (count == 100)
                        {
                            break;
                        }
                    }

                });

            Validate(_view.Parameters.AccountName);


        }

        public void get_account_name(string account_id)
        {
            string textEnteredPlusNew = "account_id = " + account_id;

            ThreadPool.QueueUserWorkItem(
                delegate(object eventArg)
                {
                    var someObject = m_AllAccounts;
                    if (someObject == null)
                    {
                        _view.GetAcc();
                        return;
                    }
                    foreach (DataRow row in m_AllAccounts.Tables[0].Select(textEnteredPlusNew))
                    {
                        object item = row["short_name"];
                        _view.AccountName = Convert.ToString(item);
                        _view.Parameters.AccountName = _view.AccountName;

                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {

                        }), System.Windows.Threading.DispatcherPriority.Normal);
                    }

                });
        }

        private void comboBoxEdit1_LostFocus(object sender, RoutedEventArgs e)
        {
            ComboBoxEdit comboBoxEdit = (ComboBoxEdit)sender;


            Validate(comboBoxEdit.Text);
        }


        #endregion Account

    

        # region HelperProcedures

     
        private void Get_From_info()
        {

            _view.AccountName = _view.Parameters.AccountName;
           

          


        }

       

        public void GetSecurities()
        {
            ThreadPool.QueueUserWorkItem(
                delegate(object eventArg)
                {
                   
                    AllSecurities = _view.DBService.GetSecurityInfo("");

                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {

                    }), System.Windows.Threading.DispatcherPriority.Normal);

                });
        }

 
        //}


     
        private void AssignXML()
        {





            XML = _view.Parameters.XML;


            if (File.Exists((subPath + "\\" + XML)))
            {


                _dataGrid.RestoreLayoutFromXml(subPath + "\\" + XML);


            }

          
        }

        public void SaveXML()
        {


            bool exists = System.IO.Directory.Exists((subPath));

            if (!exists)
            {
                System.IO.Directory.CreateDirectory((subPath));

                _dataGrid.SaveLayoutToXml(subPath + "\\" + XML);

            }
            else
            {
                _dataGrid.SaveLayoutToXml(subPath + "\\" + XML);

            }
        }

        public void UpdateAccoutText()
        {


            _view.Parameters.AccountName = _view.AccountName;


        }

       

       
        # endregion HelperProcedures



        # region Methods

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            _view.UpdateBBData(Convert.ToInt32(_view.AccountId));
            _view.Get_ChartData(Convert.ToInt32(_view.AccountId));
          
        }

        private void Button_Save_XML(object sender, RoutedEventArgs e)
        {
            _view.Get_ChartData(Convert.ToInt32(_view.AccountId));
        }
      
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {


            bool exists = System.IO.Directory.Exists((subPath));
            if (!exists)
            {
                System.IO.Directory.CreateDirectory((subPath));
            }
            Get_From_info();
         
           

        }

        private void viewAccountTax_CustomRowAppearance(object sender, CustomRowAppearanceEventArgs e)
        {
            e.Result = e.ConditionalValue;
            e.Handled = true;
        }

        private void viewAccountTax_CustomCellAppearance(object sender, CustomCellAppearanceEventArgs e)
        {
            e.Result = e.ConditionalValue;
            e.Handled = true;
        }

        void OnShowGridMenu(object sender, GridMenuEventArgs e)
        {
            if (e.MenuType == GridMenuType.Column)
            {
                GridColumnHeader columnHeader = e.TargetElement as GridColumnHeader;
                GridColumn column = columnHeader.DataContext as GridColumn;
                bool showColumnHeaderEditor = ColumnBehavior.GetIsRenameEditorActivated(column);
                BarButtonItem item = new BarButtonItem();
                if (showColumnHeaderEditor)
                    item.Content = "Hide ColumnHeader Editor";
                else
                    item.Content = "Show ColumnHeader Editor";
                item.ItemClick += OnItemClick;
                item.Tag = column;
                e.Customizations.Add(item);
            }
        }

        static void OnItemClick(object sender, ItemClickEventArgs e)
        {
            GridColumn column = e.Item.Tag as GridColumn;
            ColumnBehavior.SetIsRenameEditorActivated(column, !ColumnBehavior.GetIsRenameEditorActivated(column));
        }

        private void OnRenameEditorLostFocus(object sender, RoutedEventArgs e)
        {
            TextEdit edit = sender as TextEdit;
            edit.Visibility = System.Windows.Visibility.Hidden;
        }
        #endregion Methods

      

    

    
      

       

      

        



    

    }
}
