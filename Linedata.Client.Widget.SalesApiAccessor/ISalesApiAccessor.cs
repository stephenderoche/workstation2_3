namespace Linedata.Client.Widget.SalesApiAccessor
{
    using System;
    using System.Collections.Generic;
    using Linedata.Shared.Workstation.Api.PortfolioManagement.DataContracts;

    public interface ISalesApiAccessor : IDisposable
    {
        ISalesNotifier AccountSummaryNotifier { get; }

        void GetAllAccounts(Action<IList<BasicAccountInfo>> callback);

        void GetDetailAccountInfo(long accountId, Action<DetailAccountInfo> callback);

        void SubscribeToUpdates(long accountId);

        void UnsubscribeToUpdates(long accountId);
    }
}
