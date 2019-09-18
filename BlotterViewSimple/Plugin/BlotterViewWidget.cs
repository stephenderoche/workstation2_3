using Linedata.Shared.Widget.Common;
namespace BlotterView.Client.Plugin
{
 
   
    using BlotterView.Client.ViewModel;
    using BlotterView.Client.View;
    using BlotterView.Client.Model;
    using BlotterView.Client.Plugin;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
  
    using Linedata.Framework.WidgetFrame.PluginBase;
    using Linedata.Shared.Widget.Common;
    using ReactiveUI;
    using Splat;

   

    [Export]

    public class BlotterViewWidget : WidgetSubscriber, IWidget, INotifyPropertyChanged
    {
        private readonly IReactivePublisher publisher;
        private BlotterViewVisual GenericWindow;
        //private BlotterViewParams pluginParameters;
        private IWidgetParameters parm;
        private const string AccountIdColumnName = "account_id";
        private WidgetGroups group;

        [ImportingConstructor]
        public BlotterViewWidget(IReactivePublisher publisher
           , BlotterViewViewerModel genericGridViewerMode)
            : base(publisher)
        {

            this.ViewModel = genericGridViewerMode;
            this.ViewModel.ParentWidget = this;


           // this.Parameters = new BlotterViewParams();
            //this.pluginParameters = new BlotterViewParams();
            //this.Parameters.SetParams(this.pluginParameters.GetParams());
            //this.ViewModel.Parameters = (BlotterViewParams)this.Parameters;


            Locator.CurrentMutable.Register(() => this.UiElement, typeof(IViewFor<BlotterViewViewerModel>));

           // ((BlotterViewParams)this.Parameters).UiElement = this.UiElement as IAccountSummaryView;

            //GenericWindow = new BlotterViewVisual(this.ViewModel);
            //this.UiElement = GenericWindow;
            //this.publisher = publisher;
            
        }


        //public FrameworkElement UiElement { get; private set; }
        ~BlotterViewWidget()
        {
          
            this.Dispose(false);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool CanShowSettings
        {
            get { return true; }
        }

        public CommunicationMode CommunicationMode
        {
            get { return CommunicationMode.Subscriber; }
        }

        public Size DesiredSize
        {
            get { return new Size(300, 200); }
        }



        public WidgetGroups Group
        {
            get
            {
                return this.group;
            }

            set
            {
                this.group = value;
                this.NotifyPropertyChanged("Group");
                
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void InitWidget()
        {
            this.RegisterHandlerFor<WidgetMessages.AccountChanged>(this.Handle);
        }


        public BlotterViewViewerModel ViewModel { get; private set; }

       // public IWidgetParameters Parameters { get; private set; }


        public IWidgetParameters Parameters
        {
            get
            {
                //if (this.parm == null)
                //{
                //    this.parm = new BlotterViewParams();
                //}

                return this.parm;
            }
        }

        public void ShowSettings()
        {
          
            BlotterViewSettingsViewModel settingsViewModel = new BlotterViewSettingsViewModel(this.ViewModel, this.GenericWindow);
            BlotterViewSettingVisual window = new BlotterViewSettingVisual(settingsViewModel, this.GenericWindow);

            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }

        public FrameworkElement UiElement
        {
            get;
            private set;
        }
        public void  Dispose()
        {
            BlotterViewVisual wb = this.UiElement as BlotterViewVisual;

           // wb.SaveXML();
        
            GC.SuppressFinalize(this);
        }

     

        public void Handle(WidgetMessages.AccountChanged message)
        {
            this.ViewModel.PublishedBlock = message.AccountId;



            //this.ViewModel.GetChecks();
        }


      

        public class AccountChanged : GroupMessage
        {
            public long AccountId { get; private set; }

            public AccountChanged(
                Guid tabId,
                WidgetGroups group,
                long accountId)
                : base(group, tabId)
            {
                AccountId = accountId;
            }
        }
      
    }
}
