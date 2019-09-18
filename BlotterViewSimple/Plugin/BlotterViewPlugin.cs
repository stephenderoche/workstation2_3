namespace BlotterView.Client.Plugin
{
    using Linedata.Client.Widget.BaseWidget.Plugin;
    using Linedata.Framework.WidgetFrame.PluginBase;
    //using System.ComponentModel.Composition;
    using Linedata.Shared.Widget.Common;

    [Plugin(
        "BlotterView",
        "Trading",
        "44709678-8559-4C48-86A8-0D717BBCC244",
        "pack://application:,,,/BlotterView.Client;component/Images/transaction-list.png")]




    public class BlotterViewPlugin : BasePlugin<BlotterViewWidget, BlotterViewParams,WidgetMessages.AccountChanged>
    //public class BlotterViewPlugin : BasePlugin<BlotterViewWidget, BlotterViewParams, WidgetMessages.AccountChanged>
    {
        string a = "p";
    }
}
