// Just testing remote push and need to add this change to have someting to remote push
using GalaSoft.MvvmLight;
using Microsoft.WindowsAzure.MobileServices;
#if !OnlineOnly
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
#endif
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using JumpStreetMobile.Shared.ViewModel;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Linq;
using JumpStreetMobile.Shared.Utils;
using System.Net.Http;
using Xamarin.Forms;
using Plugin.Connectivity;
using GalaSoft.MvvmLight.Messaging;
using JumpStreetMobile.Shared.Messages;

namespace JumpStreetMobile.Shared.Model
{
    /// <summary>
    /// Locator class styled after the standard MVVM Light pattern 
    /// </summary>
    /// <remarks>
    /// I did not use dependency injection because there isn't any UI designers in
    /// Xamarin that have design-time data capabilities
    /// </remarks>
#if !OnlineOnly
    class Locator : ViewModelBase, IMobileServiceSyncHandler
#endif
#if OnlineOnly
    class Locator : ViewModelBase
#endif
    {
        public string Version { get { return "Version 1.0"; } }

        #region static public Locator Instance
        static Locator _Locator = null;
        static public Locator Instance { get { if (_Locator == null) _Locator = new Locator(); return _Locator; } }
        #endregion

        #region Mobile Service Related Members

        // ToDo: Add the NuGet package WindowsAzure.MobileServices.SQLiteStore to every mobile client project
        // For Xamarin.iOS, also edit AppDelegate.cs and uncomment the call to SQLitePCL.CurrentPlatform.Init()
        // For more information, see: http://go.microsoft.com/fwlink/?LinkId=620342 

        /// <summary>
        /// URL of the mobile service backend for this application
        /// </summary>
        /// <remarks>
        /// Note: If you are using, or plan on using, Mobile App Authentication then you will need to be using
        /// https scheme for URL to backend service otherwise you will get a runtime exception stating the lack
        /// of https in the URL.
        /// </remarks>
        public Uri ApplicationUri = new Uri(@"https://JumpStreetMobile.azurewebsites.net");

        #region private MobileServiceClient MobileService
        public MobileServiceClient _MobileService = null;
        public MobileServiceClient MobileService
        {
            get
            {
                if (_MobileService == null)
                {
                    _MobileService = new MobileServiceClient(ApplicationUri);

#if !OnlineOnly
                    var store = new MobileServiceSQLiteStore("localstore.db");

                    // Create the tables
                    store.DefineTable<TodoItem>();

                    //Initializes the SyncContext using the IMobileServiceSyncHandler.
                    _MobileService.SyncContext.InitializeAsync(store, this);
#endif
                }

                return _MobileService;
            }
        }
        #endregion

        #region String Constants
        const string LOCAL_VERSION = "Use local version";
        const string SERVER_VERSION = "Use server version";
        const string ACTIVE_ITEMS = "ActiveItems";
        #endregion

#if !OnlineOnly

        #region IMobileServiceSyncHandler Methods
        public virtual Task OnPushCompleteAsync(MobileServicePushCompletionResult result)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Sync conflict resolution logic
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        /// <remarks>
        /// Gets called by SDK when it detects a synchronization conflict during sync operations.
        /// If there are multiple conflicts during a sync operation then this method will be called
        /// once for each conflict.
        /// </remarks>
        public virtual async Task<JObject> ExecuteTableOperationAsync(IMobileServiceTableOperation operation)
        {
            MobileServicePreconditionFailedException error;

            do
            {
                error = null;

                try
                {
                    return await operation.ExecuteAsync();
                }
                catch (MobileServicePreconditionFailedException ex)
                {
                    error = ex;
                }

                if (error != null)
                {
                    var serverValue = error.Value;

                    ResolverResponse resolution = await ConflictResolver(serverValue, operation.Item);

                    if (resolution == ResolverResponse.LocalVersion)
                    {
                        // Overwrite the server version and try the operation again by continuing the loop
                        operation.Item[MobileServiceSystemColumns.Version] = serverValue[MobileServiceSystemColumns.Version];
                        continue;
                    }
                    else if (resolution == ResolverResponse.ServerVersion)
                    {
                        return (JObject)serverValue;
                    }
                    else
                    {
                        operation.AbortPush();
                    }
                }
            } while (error != null);

            return null;
        }
        #endregion

#endif
        /// <summary>
        /// Conflict resolver to use when when synchronization conflicts occur
        /// </summary>
        public ConflictResolver ConflictResolver { get; set; }

        #region public string OnlineStatus
        /// <summary>
        /// The <see cref="OnlineStatus" /> property's name.
        /// </summary>
        public const string OnlineStatusPropertyName = "OnlineStatus";

        private string _OnlineStatus;

        /// <summary>
        /// Sets and gets the OnlineStatus property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string OnlineStatus
        {
            get
            {
                return _OnlineStatus == null ? "Local" : _OnlineStatus;
            }
            private set
            {
                Set(OnlineStatusPropertyName, ref _OnlineStatus, value);
            }
        }
        #endregion

        #region public string LoginStatus
        /// <summary>
        /// The <see cref="LoginStatus" /> property's name.
        /// </summary>
        public const string LoginStatusPropertyName = "LoginStatus";

        private string _LoginStatus;

        /// <summary>
        /// Sets and gets the LoginStatus property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string LoginStatus
        {
            get
            {
                return _LoginStatus == null ? "Login" : _LoginStatus;
            }
            set
            {
                Set(LoginStatusPropertyName, ref _LoginStatus, value);
            }
        }
        #endregion

        /// <summary>
        /// Sets and gets the IsSyncEnabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsSyncEnabled { get { return ApplicationCapabilities.ModeOfOperation == ModeOfOperation.OnlineAndOffline; } }

        /// <summary>
        /// Syncs local data with server and vice versa
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This method also sets the IsOnline
        /// </remarks>
        async public Task SyncChanges()
        {
#if !OnlineOnly
            try
            {
                // Make sure syncing is enabled and we are connected before attempting to sync
                // The IMobileServiceSyncHandler methods in this class handle conflict resolution, if there is any
                await MobileService.SyncContext.PushAsync();

                // Sync all backing ViewModels here
                await SyncTodoItemsViewModel();
            }
            catch (MobileServicePushFailedException exception)
            {
                System.Diagnostics.Debug.WriteLine("Database Sync Failed: {0}", exception.PushResult.Status);

                foreach (var error in exception.PushResult.Errors)
                {
                    System.Diagnostics.Debug.WriteLine("\t{0} failed on {1} because '{2}': {3}", error.OperationKind, error.TableName, error.Status, error.RawResult);
                }

                Messenger.Default.Send<ShowMessageDialog>(new ShowMessageDialog() { Title = "Be Aware", Message = "Could not cocnnect to the app service so the application will run in offline mode. Error: " + exception.Message + " - " + exception.PushResult.Status.ToString() });
            }
            catch (Exception e)
            {
                // If users chooses to keep server version it can cause the ListView collection to be modified which will
                // throw an exception with the HResult below which means the enumeration operation may not execute which
                // we will ignore since its the best we can do. In practice, ignoring this error has worked fine.
                if (e.HResult != -2146233079)
                {
                    // Todo: need to report this error to user to let them know that sync failed
                    System.Diagnostics.Debug.WriteLine("Could not reach mobile service.  Continuing in offline mode.  Error: {0}", args: e.Message);

                    Messenger.Default.Send<ShowMessageDialog>(new ShowMessageDialog() { Title = "Sychronization Error", Message = "Could not cocnnect to the app service so the application will run in offline mode. Error: " + e.Message });
                }
            }
#endif
        }

        async public Task PushChanges(bool syncViewModels = false)
        {
#if !OnlineOnly
            try
            {
                // The IMobileServiceSyncHandler methods in this class handle conflict resolution, if there is any
                await MobileService.SyncContext.PushAsync();

                if (syncViewModels)
                {
                    // Sync all backing ViewModels here
                    await SyncTodoItemsViewModel();
                }
            }
            catch (MobileServicePushFailedException exception)
            {
                System.Diagnostics.Debug.WriteLine("Database Sync Failed: {0}", exception.PushResult.Status);
                foreach (var error in exception.PushResult.Errors)
                {
                    System.Diagnostics.Debug.WriteLine("\t{0} failed on {1} because '{2}': {3}", error.OperationKind, error.TableName, error.Status, error.RawResult);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Could not reach mobile service.  Continuing in offline mode.  Error: {0}", e.Message);
            }
#endif
        }

        #endregion // Mobile Service Related Members

        #region Authentication Members

        public IAuthenticate Authenticator { get; set; }

        #region public bool IsLoginRequested

        /// <summary>
        /// The <see cref="IsLoginRequested" /> property's name.
        /// </summary>
        public const string IsLoginRequestedPropertyName = "IsLoginRequested";

        private bool _IsLoginRequested = false;

        /// <summary>
        /// Sets and gets the IsLoginRequested property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsLoginRequested
        {
            get
            {
                return _IsLoginRequested;
            }
            set
            {
                Set(IsLoginRequestedPropertyName, ref _IsLoginRequested, value);

                // Force the binding of the dependent property update
                ShowLogin = value;
            }
        }

        #endregion // public bool IsLoginRequested

        #region public bool IsLoginNeeded
        /// <summary>
        /// The <see cref="IsLoginNeeded" /> property's name.
        /// </summary>
        public const string IsLoginNeededPropertyName = "IsLoginNeeded";

        private bool _IsLoginNeeded = ApplicationCapabilities.IsAuthenticationRequired &&
                                      ApplicationCapabilities.ModeOfOperation == ModeOfOperation.OnlineOnly;

        /// <summary>
        /// Indicates wether or not the user needs to login
        /// </summary>
        /// <remarks>
        /// The value of this property is dependent on, and set by, the IsAuthenticated property
        /// and if its true it means authentication is required but the user has not successful
        /// authenticated yet and thus they need to login.
        /// 
        /// If this property is set to false it means the user has been successfully authenticated
        /// and nolonger needs to login
        /// </remarks>
        public bool IsLoginNeeded
        {
            get
            {
                return _IsLoginNeeded;
            }
            private set
            {
                Set(IsLoginNeededPropertyName, ref _IsLoginNeeded, value);
            }
        }
        #endregion

        #region public bool IsAuthenticated
        /// <summary>
        /// The <see cref="IsAuthenticated" /> property's name.
        /// </summary>
        public const string IsAuthenticatedPropertyName = "IsAuthenticated";

        private bool _IsAuthenticated = false;

        /// <summary>
        /// Sets and gets the IsAuthenticated property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                return _IsAuthenticated;
            }
            set
            {
                Set(IsAuthenticatedPropertyName, ref _IsAuthenticated, value);
            }
        }

        #endregion //public bool IsAuthenticated

        #region public bool ShowLogin
        /// <summary>
        /// The <see cref="ShowLogin" /> property's name.
        /// </summary>
        public const string ShowLoginPropertyName = "ShowLogin";

        private bool _ShowLogin = false;

        /// <summary>
        /// Sets and gets the ShowLogin property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool ShowLogin
        {
            get
            {
                return IsLoginNeeded || IsLoginRequested;
            }
            private set
            {
                // The value of this property is totally independent of the backing
                // property.  No matter what value is assigned to this property it
                // its only effect will be to update this property's binding.  This
                // will cause the computed value in the getter to flow to the UI
                Set(ShowLoginPropertyName, ref _ShowLogin, !_ShowLogin);
            }
        }
        #endregion //public bool ShowLogin

        public bool IsAuthenticationRequired { get { return ApplicationCapabilities.IsAuthenticationRequired; } }

        #region Login/Logout methods
        async public Task Login(string provider)
        {
            // If authentication is enabled and we're not authenticated yet
            if (Authenticator != null && !IsAuthenticated)
                IsAuthenticated = await Authenticator.Authenticate(provider);

            if (IsAuthenticated)
            {
                // Login is no longer needed now that we know we are logged in
                Locator.Instance.IsLoginNeeded = false;

                try
                {
                    JToken userName = await MobileService.InvokeApiAsync("UserProfile/UserName", HttpMethod.Get, new Dictionary<string, string>() { { "provider", ApplicationCapabilities.ConvertString2IdentityProvider(provider).ToString() } });

                    LoginStatus = userName.ToObject(typeof(string)) as string;
                }
                catch (Exception)
                {
                    LoginStatus = "<User Name Unavailable>";
                }
            }
            else
                LoginStatus = "Login";
        }

        async public Task Logout()
        {
            try
            {
                // If authentication is enabled and we're logged in then logout
                if (Authenticator != null && IsAuthenticated)
                {
                    await MobileService.LogoutAsync();

                    IsAuthenticated = false;
                    LoginStatus = "Login";

                    // Login is only manditory if mode is online-only and authentication is required
                    IsLoginNeeded = ApplicationCapabilities.IsAuthenticationRequired &&
                                    ApplicationCapabilities.ModeOfOperation == ModeOfOperation.OnlineOnly;
                }
            }
            catch (Exception e)
            {
                // ToDo: handle this situation properly by using MVVM Light message
            }
        }

        #endregion Login/Logout

        #endregion // Authentication Members

        #region Push Notification Members
        public IPushNotifications Notifier { get; set; }

        /// <summary>
        /// Indicates whether or not push notifications are required for this application
        /// </summary>
        public bool IsPushNotificationRequired { get { return ApplicationCapabilities.IsPushNotificationRequired; } }

        #region public bool IsPushNotificationsRegistered
        /// <summary>
        /// The <see cref="IsPushNotificationsRegistered" /> property's name.
        /// </summary>
        public const string IsPushNotificationsRegisteredPropertyName = "IsPushNotificationsRegistered";

        private bool _IsPushNotificationsRegistered = false;

        /// <summary>
        /// Sets and gets the IsPushNotificationsRegistered property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsPushNotificationsRegistered
        {
            get
            {
                return _IsPushNotificationsRegistered;
            }
            set
            {
                Set(IsPushNotificationsRegisteredPropertyName, ref _IsPushNotificationsRegistered, value);
            }
        }
        #endregion

        #endregion

        #region Connectivity Members

        async public Task<bool> IsOnline()
        {
            bool result = false;

            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(this.MobileService.MobileAppUri);

                result = response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.Message);
                result = false;
            }

            // Set the dependent status property
            OnlineStatus = result ? "Online" : "Local";

            return result;
        }

        #endregion

        #region public bool IsBusy
        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        public const string IsBusyPropertyName = "IsBusy";

        private bool _IsBusy = false;

        /// <summary>
        /// Sets and gets the IsBusy property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return _IsBusy;
            }
            set
            {
                Set(IsBusyPropertyName, ref _IsBusy, value);
            }
        }
        #endregion

        #region TodoItem related members

        // To enable offline sync so that data is stored locally on the
        // device, use this declaration of TodoItemTable and
        // 1) Set ApplicationCapabilities.ModeOfOperation to OnlineAndOffline or OfflineOnly
        // 2) Uncomment the table definition below 
        // 3) Uncomment calls to SyncTodoItemsViewModel() and the method itself
        // 4) Comment the other sync-based defintion of TodoItemTable
        #region public IMobileServiceSyncTable<TodoItem> TodoItemTable
#if !OnlineOnly
        private IMobileServiceSyncTable<TodoItem> _TodoItemTable;
        public IMobileServiceSyncTable<TodoItem> TodoItemTable
        {
            get
            {
                if (_TodoItemTable == null)
                    _TodoItemTable = Instance.MobileService.GetSyncTable<TodoItem>();

                return _TodoItemTable;
            }
        }
#endif
        #endregion

        // To disable offline sync so that no data is stored locally and
        // all data operations happen live, use this definition
        // 1) Set ApplicationCapabilities.ModeOfOperation to ModeOfOperation.OnlineOnly
        // 2) Uncomment the table definition below 
        // 3) Comment out calls to SyncTodoItemsViewModel() and the method itself
        // 4) Comment the other non-sync defintion of TodoItemTable
        #region public IMobileServiceTable<TodoItem> TodoItemTable
#if OnlineOnly
        private IMobileServiceTable<TodoItem> _TodoItemTable;
        public IMobileServiceTable<TodoItem> TodoItemTable
        {
            get
            {
                if (_TodoItemTable == null)
                    _TodoItemTable = Instance.MobileService.GetTable<TodoItem>();

                return _TodoItemTable;
            }
        }
#endif
        #endregion

        #region public ObservableCollection<TodoItemViewModel> TodoItems
        ObservableCollection<TodoItemViewModel> _TodoItems = null;
        public ObservableCollection<TodoItemViewModel> TodoItems
        {
            get
            {
                return _TodoItems;
            }
        }
        #endregion

#if !OnlineOnly
        // ToDo: Turn this into a generic method on the order of:
        // Task SyncViewModel<T, U>(string queryId, IMobileServiceSyncTable<T> table, IMobileServiceTableQuery<U> query)
        async public Task SyncTodoItemsViewModel()
        {
            // Update the local database from the server.  Note: For sync tables, the SDK saves
            // an updatedAt timestamp with each queryId ('ACTIVE_ITEMS' in this case) and uses
            // that timestamp to pull only new records from the server (i.e. records who's 
            // UpdateAt >= updatedAt assocated with queryId).  So this PullAsync() only pulls
            // newly added or updated records.  Note that we aren't using a filtering query
            // (we passed a null instead) otherwise we would miss items that were updated by
            // another user if that update caused it to match the filter.  In other words, we
            // don't want to filter out a possible update just because it now meets the filtering
            // criteria otherwise we won't be able to sync that change with the ViewModel.
            // ToDo: Get this version of the PullAsync to work since it is more efficient
            //await TodoItemTable.PullAsync(ACTIVE_ITEMS + Locator.Instance.MobileService.CurrentUser.UserId, null);
            await TodoItemTable.PullAsync(ACTIVE_ITEMS, null);

            List<TodoItem> tblTodoItems = await TodoItemTable.ToListAsync();

            // This foreach loops handles updating or adding ViewModel items by iterating
            // over todo items fetched from the server (held in tblTodoItems) and add and
            // updating corresponding todo items is the backing ViewModel (held in TodoItems)
            foreach (TodoItem tblTodoItem in tblTodoItems)
            {
                bool itemIsInViewModel = false;

                // Can't use Linq on an ObservableCollection so we have to manually search for it
                foreach (TodoItemViewModel vmTodoItem in TodoItems)
                {
                    // If table item is in ViewModel then check to see if it needs to be updated
                    if (tblTodoItem.Id == vmTodoItem.TodoItem.Id)
                    {
                        itemIsInViewModel = true;

                        // If ViewModel item needs to be updated
                        if (vmTodoItem.TodoItem.Version != tblTodoItem.Version)
                        {
                            System.Diagnostics.Debug.WriteLine("Updating item: {0}, \nOld Name: {1}, Old Done: {2}, Old Version: {3}\nNew Name: {4}, New Done: {5}, New Version: {6}", vmTodoItem.TodoItem.Id, vmTodoItem.Name, vmTodoItem.Done, vmTodoItem.TodoItem.Version, tblTodoItem.Name, tblTodoItem.Done, tblTodoItem.Version);

                            // Because TodoItems are data bound to the UI we have to update it on the UI thread
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                // Assign these through the ViewModel properties so that INPC event fires
                                vmTodoItem.Name = tblTodoItem.Name;
                                vmTodoItem.Done = tblTodoItem.Done;

                                // Assign these through the Model properties since they don't have 
                                // corresponding ViewModel propeties to assign through
                                vmTodoItem.TodoItem.Version = tblTodoItem.Version;
                            });

                            // Because the update is occuring async we need to block until the update
                            // completes otherwise UI binding updates might not show up properly 
                            bool updateComplete = false;
                            while (!updateComplete)
                            {
                                System.Diagnostics.Debug.WriteLine("Waiting for updates to complete");

                                updateComplete = vmTodoItem.Name == tblTodoItem.Name &&
                                                 vmTodoItem.Done == tblTodoItem.Done &&
                                                 vmTodoItem.TodoItem.Version == tblTodoItem.Version;

                                // Free up execution while we wait
                                await Task.Delay(50);
                            }
                        }
                    }
                }

                // If table item is not in ViewModel but it should be then add it
                if (!itemIsInViewModel && !tblTodoItem.Done)
                {
                    System.Diagnostics.Debug.WriteLine("Adding item: {0}, Name: {1}", tblTodoItem.Id, tblTodoItem.Name);

                    int beforeCount = TodoItems.Count;
                    bool addComplete = false;

                    // Because TodoItems is data bound to the UI we have to add it on the UI thread
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        TodoItems.Add(new TodoItemViewModel() { TodoItem = tblTodoItem });
                    });

                    // Because add is occuring async we need to block until it completes
                    // otherwise some adds might not be added to the backing ViewModel and
                    // thus the UI binding won't update correctly either
                    while (addComplete)
                    {
                        System.Diagnostics.Debug.WriteLine("Waiting for adds to complete");

                        addComplete = TodoItems.Count > beforeCount;

                        // Free up execution while we wait
                        await Task.Delay(50);
                    }
                }
            }

            List<TodoItemViewModel> vmItemToDelete = new List<TodoItemViewModel>();

            // This foreach loop delets todo items in backing ViewModel (held in
            // TodoItems) that no longer exist in the server todo items (held in
            // tblTodoItems). Since you can't remove items from a collection while
            // you are iterating over it, we need to create a separate list of 
            // items that need to be removed
            foreach (TodoItemViewModel vmTodoItem in TodoItems)
            {
                // If ViewModel item does not exist in synched table then it was deleted
                // so we need to delete it from the ViewModel as well
                if (!tblTodoItems.Exists(tblItem => tblItem.Id == vmTodoItem.TodoItem.Id))
                    vmItemToDelete.Add(vmTodoItem);

                // Todo items that were updated in such a way that they no longer meet
                // the filtering criteria are removed here.  If you don't have filtering
                // criteria in your app then comment or remove this if statement
                if (vmTodoItem.Done)
                    vmItemToDelete.Add(vmTodoItem);
            }

            int newCount = TodoItems.Count - vmItemToDelete.Count;

            // Remove delete items from the ViewModel
            foreach (TodoItemViewModel vmTodoItem in vmItemToDelete)
            {
                System.Diagnostics.Debug.WriteLine("Deleting item: {0}, Name: {1}", vmTodoItem.TodoItem.Id, vmTodoItem.Name);

                // Because TodoItems is data bound to the UI we have to remove it on the UI thread
                Device.BeginInvokeOnMainThread(() =>
                {
                    TodoItems.Remove(vmTodoItem);
                });
            }

            // ToDo: I wrote this loop to make sure UI updated and presumably I tested
            // this to make sure it was needed AND it worked.  What I have found is that
            // there are times where this loops forever while waiting for count to catch
            // up.  So for now I have it commented out.
            //// Because deletes are occuring async we need to block until they 
            //// complete otherwise UI binding might not update properly
            //while (TodoItems.Count > newCount)
            //{
            //    System.Diagnostics.Debug.WriteLine("Waiting for deletes to complete");

            //    // Free up execution while we wait
            //    await Task.Delay(50);
            //}

            // Clean up local table by removing items marked as Done otherwise those records will
            // just stay in the local database causing it to continually grow with items that arn't
            // needed any more but hang around hidden because of the pull filter being used.
            // See https://azure.microsoft.com/en-us/documentation/articles/app-service-mobile-offline-data-sync/
            // for a fuller explanation.
            await TodoItemTable.PurgeAsync(TodoItemTable.Where(todoItem => todoItem.Done));
        }
#endif

        /// <summary>
        /// Loads the TodoItems property asynchronously because properties don't support async
        /// </summary>
        /// <returns>Returns all Todo items not marked as Done</returns>
        /// <remarks>
        /// We use a method-based fetcher to fill the collection with data the mobile service 
        /// table because c# properties can't be async.  This method is typically called in the
        /// Appearing (Xamarin.Forms) or Loaded (Windows Xaml) methods of the View that is going
        /// to show the collection.
        /// 
        /// Notice, for performace reasons, the collection is only fetched once for the lifespan
        /// of the application which means that you must keep the collection up to date as items
        /// are added, removed or updated in the backing mobile service table.  In other words,
        /// when you add, remove, or update the backing mobile service table you must do the 
        /// same thing the collection fetched by this method.
        /// </remarks>
        async public Task GetTodoItems()
        {
            if (_TodoItems == null)
            {
                _TodoItems = new ObservableCollection<TodoItemViewModel>();

                foreach (TodoItem todoItem in await TodoItemTable.Where(todoItem => !todoItem.Done).ToListAsync())
                {
                    _TodoItems.Add(new TodoItemViewModel() { TodoItem = todoItem });
                }
            }
        }    

#region public TodoListViewModel TodoListViewModel
        /// <summary>
        /// The <see cref="TodoListViewModel" /> property's name.
        /// </summary>
        public const string TodoListViewModelPropertyName = "TodoListViewModel";

        private TodoListViewModel _TodoListViewModel = null;

        /// <summary>
        /// Sets and gets the TodoListViewModel property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public TodoListViewModel TodoListViewModel
        {
            get
            {
                if (_TodoListViewModel == null)
                    _TodoListViewModel = new TodoListViewModel();

                return _TodoListViewModel;
            }
            set
            {
                Set(TodoListViewModelPropertyName, ref _TodoListViewModel, value);
            }
        }
#endregion

#endregion // TodoItem related members
    }
}
