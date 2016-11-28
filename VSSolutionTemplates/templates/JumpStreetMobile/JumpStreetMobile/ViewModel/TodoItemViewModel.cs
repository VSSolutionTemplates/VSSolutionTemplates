using GalaSoft.MvvmLight;
using JumpStreetMobile.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace JumpStreetMobile.ViewModel
{
    public class TodoItemViewModel : ViewModelBase
    {
        #region public TodoItem TodoItem
        /// <summary>
        /// The <see cref="TodoItem" /> property's name.
        /// </summary>
        public const string TodoItemPropertyName = "TodoItem";

        private TodoItem _TodoItem = new TodoItem();

        /// <summary>
        /// Sets and gets the TodoItem property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public TodoItem TodoItem
        {
            get
            {
                return _TodoItem;
            }
            set
            {
                Set(TodoItemPropertyName, ref _TodoItem, value);
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
                return _TodoItem.Name;
            }
            set
            {
                // Sync backing property in Model
                _TodoItem.Name = value;

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
                return _TodoItem.Done;
            }
            set
            {
                _TodoItem.Done = value;

                Set(DonePropertyName, ref _Done, value);
            }
        }
        #endregion
    }
}
