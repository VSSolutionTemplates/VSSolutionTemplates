using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JumpStreetMobile.Messages;
using JumpStreetMobile.Model;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JumpStreetMobile.ViewModel
{
    public class TodoListViewModel : ViewModelBase
    {
        #region public TodoItemViewModel TodoItemViewModel
        /// <summary>
        /// The <see cref="TodoItemViewModel" /> property's name.
        /// </summary>
        public const string TodoItemViewModelPropertyName = "TodoItemViewModel";

        private TodoItemViewModel _TodoItemViewModel = null;

        /// <summary>
        /// Sets and gets the TodoItemViewModel property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public TodoItemViewModel TodoItemViewModel
        {
            get
            {
                return _TodoItemViewModel;
            }
            set
            {
                Set(TodoItemViewModelPropertyName, ref _TodoItemViewModel, value);
            }
        }
        #endregion

        #region public string Name
        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string _Name;

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Name
        {
            get
            {
                return _TodoItemViewModel.Name;
            }
            set
            {
                // Sync backing property in Model
                _TodoItemViewModel.Name = value;

                // Raise the INPC event so binding gets new value
                Set(NamePropertyName, ref _Name, value);
            }
        }
        #endregion

        #region public bool Done
        /// <summary>
        /// The <see cref="Done" /> property's name.
        /// </summary>
        public const string DonePropertyName = "Done";

        private bool _Done;

        /// <summary>
        /// Sets and gets the Done property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool Done
        {
            get
            {
                return _TodoItemViewModel.Done;
            }
            set
            {
                _TodoItemViewModel.Done = value;

                Set(DonePropertyName, ref _Done, value);
            }
        }
        #endregion

        #region public bool IsAddActive
        /// <summary>
        /// The <see cref="IsAddActive" /> property's name.
        /// </summary>
        public const string IsAddActivePropertyName = "IsAddActive";

        private bool _IsAddActive = false;

        /// <summary>
        /// Sets and gets the IsAddActive property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool IsAddActive
        {
            get
            {
                return _IsAddActive;
            }
            set
            {
                Set(IsAddActivePropertyName, ref _IsAddActive, value);

                // Set dependent variables
                AddButtonOpacity = _IsAddActive ? 0.65 : 1.0;
            }
        }
        #endregion

        #region public double AddButtonOpacity
        /// <summary>
        /// The <see cref="AddButtonOpacity" /> property's name.
        /// </summary>
        public const string AddButtonOpacityPropertyName = "AddButtonOpacity";

        private double _AddButtonOpacity = 1.0;

        /// <summary>
        /// Sets and gets the AddButtonOpacity property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double AddButtonOpacity
        {
            get
            {
                return _AddButtonOpacity;
            }
            set
            {
                Set(AddButtonOpacityPropertyName, ref _AddButtonOpacity, value);
            }
        }
        #endregion

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand<TodoItemViewModel> CompleteCommand { get; private set; }
        public RelayCommand SyncCommand { get; private set; }

        public TodoListViewModel()
        {
            TodoItemViewModel = new TodoItemViewModel();

            AddCommand = new RelayCommand(Add, CanAdd);
            CompleteCommand = new RelayCommand<TodoItemViewModel>(Complete, CanComplete);
            SyncCommand = new RelayCommand(Sync, CanSync);
        }

        /// <summary>
        /// Add item to table
        /// </summary>
        async void Add()
        {
            try
            {
                // Since Image controls can't disable in Xamarin.Forms we have to
                // simulate disablement by explicity checking to see if the operation
                // is currently allowed and let the binding system make the image 
                // button look disabled.
                if (!CanAdd())
                    return;

                IsAddActive = true;
                Locator.Instance.IsBusy = true;

                // Add item to database
                await Locator.Instance.TodoItemTable.InsertAsync(this.TodoItemViewModel.TodoItem);

                // Add item to bound ViewModel collection to keep it in sync and so ObservableCollection raises INPC event
                Locator.Instance.TodoItems.Add(new TodoItemViewModel() { TodoItem = this.TodoItemViewModel.TodoItem });

                // If sync enabled and connected then push changes to server but it won't pull any server updates
                if (Locator.Instance.IsSyncEnabled && await Locator.Instance.IsConnected())
                    await Locator.Instance.PushChanges();

                // Reset UI in preparation for subsequent entries
                Messenger.Default.Send<ResetUI>(new ResetUI());
            }
            catch (Exception e)
            {
                Messenger.Default.Send<PersistanceException>(new PersistanceException { Exception = e });
            }
            finally
            {
                Locator.Instance.IsBusy = false;
                IsAddActive = false;
            }
        }

        bool CanAdd()
        {
            return !IsAddActive;
        }

        async void Complete(TodoItemViewModel todoItemViewModel)
        {
            try
            {
                Locator.Instance.IsBusy = true;

                // Mark todo item as done which raises INPC event so UI can react appropriately
                todoItemViewModel.Done = true;

                // Update the database to reflect the completed state
                await Locator.Instance.TodoItemTable.UpdateAsync(todoItemViewModel.TodoItem);

                // If connected then push changes to server but it won't pull any server updates
                if (Locator.Instance.IsSyncEnabled && await Locator.Instance.IsConnected())
                    await Locator.Instance.PushChanges();

                // Pause so UI can briefly show the checkmark
                await Task.Delay(250);

                // Delete item from the bound ViewModel to keep it in sync and so ObservableCollection
                // raises INPC which updates ListView to reflect deleted item
                Locator.Instance.TodoItems.Remove(todoItemViewModel);

                // If we don't pause here the ResetUI will effectively be ignored because it runs
                // asychonously with the previous ListView.Remove() and will complete before it
                await Task.Delay(225);

                // Reset UI in preparation for subsequent entries
                Messenger.Default.Send<ResetUI>(new ResetUI());
            }
            catch (Exception e)
            {
                // Something went wrong so rollback changes to ViewModel
                todoItemViewModel.Done = false;

                Messenger.Default.Send<PersistanceException>(new PersistanceException { Exception = e });
            }
            finally
            {
                Locator.Instance.IsBusy = false;
            }
        }

        bool CanComplete(TodoItemViewModel todoItem)
        {
            return true;
        }

        async void Sync()
        {
            try
            {
                Locator.Instance.IsBusy = true;

                if (await Locator.Instance.IsMobileAppServiceReachable())
                    await Locator.Instance.SyncChanges();
                else if (ApplicationCapabilities.ModeOfOperation == ModeOfOperation.OnlineOnly)
                    Messenger.Default.Send(new ShowMessageDialog() { Title = "Connectivity Error", Message = "Could not connect to app service and ModeOfOperation is set to OnlineOnly so you will need to try again when network connectivity has been restored" });
                else if (ApplicationCapabilities.ModeOfOperation == ModeOfOperation.OnlineAndOffline)
                    Messenger.Default.Send(new ShowMessageDialog() { Title = "Be Aware", Message = "Could not connect to the app service so the application will run in offline mode." });
            }
            catch (Exception e)
            {
                Messenger.Default.Send<PersistanceException>(new PersistanceException { Exception = e });
            }
            finally
            {
                Locator.Instance.IsBusy = false;
            }
        }

        bool CanSync()
        {
            return true;
        }

    }
}
