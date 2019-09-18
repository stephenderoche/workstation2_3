namespace Linedata.Client.Widget.SalesApiAccessorImpl
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;
    using Linedata.Client.Widget.SalesApiAccessor;
    using Linedata.Client.Workstation.Api.PortfolioManagement;
    using Linedata.Client.Workstation.SharedReferences;
    using Linedata.Shared.Workstation.Api.DataContracts;
    using Linedata.Shared.Workstation.Api.PortfolioManagement.DataContracts;
    using log4net;

    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(ISalesApiAccessor))]
    public class SalesApiAccessor : ISalesApiAccessor
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SalesApiAccessor));
        private readonly IClientAccountApi clientAccountApi;
        private long? lastSubscribedAccountId;
        private bool disposed = false;

        [ImportingConstructor]
        public SalesApiAccessor(ISalesNotifier accountSummaryNotifier)
        {
            this.AccountSummaryNotifier = accountSummaryNotifier;
            this.clientAccountApi = ApiAccessor.Get<IClientAccountApi>();
        }

        ~SalesApiAccessor()
        {
            this.Dispose(false);
        }

        public ISalesNotifier AccountSummaryNotifier { get; private set; }

        public void OnMessageDataUpdated(MiddleTierMessageData messageData)
        {
            AccountMessageData accountMessageData = messageData as AccountMessageData;

            // sanity check
            if (accountMessageData == null)
            {
                return;
            }

            if (this.AccountSummaryNotifier != null)
            {
                this.AccountSummaryNotifier.FireAccountChanged(accountMessageData.AccountId);
            }
        }

        public async void GetAllAccounts(Action<IList<BasicAccountInfo>> callback)
        {
            try
            {
                callback(await Task.Run(() => this.clientAccountApi.GetAllAccounts()));
            }
            catch (Exception e)
            {
                Logger.DisplayError("Failed to get accounts", e);
            }
        }

        public async void GetDetailAccountInfo(long accountId, Action<DetailAccountInfo> callback)
        {
            try
            {
                callback(await Task.Run(() => this.clientAccountApi.GetDetailAccountInfo(accountId)));
            }
            catch (Exception e)
            {
                Logger.DisplayError("Failed to get infos for account: " + accountId, e);
            }
        }

        public void SubscribeToUpdates(long accountId)
        {
            try
            {
                this.lastSubscribedAccountId = accountId;
                ApiAccessor.SubscribeToUpdates(new AccountMessageData(accountId), this.OnMessageDataUpdated);
            }
            catch (Exception e)
            {
                Logger.DisplayError("Account " + accountId + " failed to subscribe to updates", e);
            }
        }

        public void UnsubscribeToUpdates(long accountId)
        {
            try
            {
                this.lastSubscribedAccountId = null;
                ApiAccessor.UnsubscribeToUpdates(new AccountMessageData(accountId), this.OnMessageDataUpdated);
            }
            catch (Exception e)
            {
                Logger.DisplayError("Account " + accountId + " failed to unsubscribe from updates", e);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.lastSubscribedAccountId.HasValue)
                    {
                        try
                        {
                            Task.Run(() => this.UnsubscribeToUpdates(this.lastSubscribedAccountId.Value));
                        }
                        catch (Exception e)
                        {
                            Logger.DisplayError("Account " + this.lastSubscribedAccountId.Value + " failed to unsubscribe from updates", e);
                        }
                    }

                    if (this.clientAccountApi != null)
                    {
                        ApiAccessor.Dispose(this.clientAccountApi);
                    }
                }

                this.disposed = true;
            }
        }
    }
}
