using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace project_fall_app.Models
{
    class User : INotifyPropertyChanged
    {
        public enum UserTypes
        {
            citizen,
            contact,
            citizenAdmin
        }
        #region fields
        private string id;
        private string name;
        private UserTypes type;
        #endregion

        #region gettersetter
        public string ID
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        public UserTypes Type
        {
            get { return type; }
            set { SetProperty(ref type, value); }
        }

        public string Credentials
        {
            get { return String.Format("{0}\n{1}\n{2}", ID, Name, Type.ToString()); }
        }

        #endregion

        #region INotifyPropertyChanged
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;
            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
