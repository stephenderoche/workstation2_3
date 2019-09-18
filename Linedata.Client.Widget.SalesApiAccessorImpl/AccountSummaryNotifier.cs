namespace Linedata.Client.Widget.AccountSummaryApiAccessorImpl
{
    using System;
    using System.ComponentModel.Composition;
    using System.ServiceModel;
    using System.Windows.Threading;
    using Linedata.Client.Widget.AccountSummaryApiAccessor;
    using Linedata.Client.Widget.Common;
    using Linedata.Client.Workstation.LongviewAdapterClient.EventArgs;

    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IAccountSummaryNotifier))]
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class AccountSummaryNotifier : IAccountSummaryNotifier
    {
        private readonly Dispatcher mainDispatcher;

        public AccountSummaryNotifier()
        {
            this.mainDispatcher = Dispatcher.CurrentDispatcher;
        }

        public event EventHandler<AccountChangedEventArgs> AccountSummaryUpdated;

        public void FireAccountChanged(long accountId)
        {
            if (this.AccountSummaryUpdated == null)
            {
                return;
            }

            this.mainDispatcher.InvokeAsRequired(() => this.AccountSummaryUpdated(this, new AccountChangedEventArgs(accountId)));
        }
    }
}
