namespace Linedata.Client.Widget.SalesApiAccessorImpl
{
    using System;
    using System.ComponentModel.Composition;
    using System.ServiceModel;
    using System.Windows.Threading;
    using Linedata.Client.Widget.SalesApiAccessor;
    using Linedata.Client.Widget.Common;
    using Linedata.Client.Workstation.LongviewAdapterClient.EventArgs;

    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(ISalesNotifier))]
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class SalesNotifier : ISalesNotifier
    {
        private readonly Dispatcher mainDispatcher;

        public SalesNotifier()
        {
            this.mainDispatcher = Dispatcher.CurrentDispatcher;
        }

        public event EventHandler<AccountChangedEventArgs> SalesUpdated;

        public void FireAccountChanged(long accountId)
        {
            if (this.SalesUpdated == null)
            {
                return;
            }

            this.mainDispatcher.InvokeAsRequired(() => this.SalesUpdated(this, new AccountChangedEventArgs(accountId)));
        }
    }
}
