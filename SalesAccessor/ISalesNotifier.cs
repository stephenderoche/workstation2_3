namespace Linedata.Client.Widget.SalesApiAccessor
{
    using System;
    using Linedata.Client.Workstation.LongviewAdapterClient.EventArgs;

    public interface ISalesNotifier
    {
        event EventHandler<AccountChangedEventArgs> SalesUpdated;

        void FireAccountChanged(long accountId);
    }
}
