using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Yeast.WebApi.Data
{
    public class ViewModelCollection<TEntity, TVM> : IList<TVM>, IList
        where TVM : BaseViewModel<TEntity>
        where TEntity : new()
    {
        protected IList<TEntity> _srcCollection;
        protected List<TVM> _vmCollection;
        protected readonly Func<TEntity, bool> _filter = null;
        protected readonly Action<TVM> _initializer = null;
        protected readonly Func<TEntity, TVM> _vmActivator;

        /// <summary>
        /// Initializes a new EntityCollectionViewModel on an existing collection
        /// </summary>
        /// <param name="srcCollection">Source entity collection.</param>
        /// <param name="vmActivator">Function to instanciate a new ViewModel from an Entity.</param>
        /// <param name="filter">Filter to apply to source collection (optional)
        /// Can be used to expose a partial list ViewModels</param>
        /// <param name="initializer">Action to execute on newly created ViewModels.</param>
        public ViewModelCollection(IList<TEntity> srcCollection, Func<TEntity, TVM> vmActivator, Func<TEntity, bool> filter = null, Action<TVM> initializer = null)
            : this(vmActivator, filter, initializer)
        {
            _srcCollection = srcCollection;
            _vmCollection = InitVmCollection();
        }

        /// <summary>
        /// Initializes a new EntityCollectionViewModel on an empty collection
        /// </summary>
        /// <param name="vmActivator">Function to instanciate a new ViewModel from an Entity.</param>
        /// <param name="filter">Filter to apply to source collection (optional)
        /// Can be used to expose a partial list ViewModels</param>
        /// <param name="initializer">Action to execute on newly created ViewModels.</param>
        public ViewModelCollection(Func<TEntity, TVM> vmActivator, Func<TEntity, bool> filter = null, Action<TVM> initializer = null)
        {
            _filter = filter;
            _initializer = initializer;
            _vmActivator = vmActivator;
            _srcCollection = new List<TEntity>();
            _vmCollection = new List<TVM>();
        }

        /// <summary>
        /// Initializes the ViewModel collection from source collection
        /// </summary>
        /// <returns>The exposed ViewModel collection</returns>
        protected virtual List<TVM> InitVmCollection()
        {
            if (_filter != null)
            {
                return new List<TVM>(_srcCollection.Where(_filter).Select(_vmActivator));
            }
            else
            {
                return new List<TVM>(_srcCollection.Select(_vmActivator));
            }
        }

        /// <summary>
        /// Lately rebinds EntityCollectionViewModel to an existing entity collection
        /// </summary>
        /// <param name="srcCollection"></param>
        public void BindToSource(IList<TEntity> srcCollection)
        {
            if (srcCollection == _srcCollection)
            {
                // Already bound
                return;
            }
            var existingVms = _vmCollection.ToList();
            _srcCollection = srcCollection;
            _vmCollection = InitVmCollection();
            // Append new entities
            foreach (var vm in existingVms.Where(vm => !_srcCollection.Contains(vm.Entity)))
            {
                Add(vm);
            }
        }

        #region IList<> implementation
        public virtual TVM this[int index] {
            get { return _vmCollection[index]; }
            set {
                _initializer?.Invoke(value);
                if (index < _vmCollection.Count && _vmCollection[index] != null)
                {
                    // Replace in source collection
                    RemoveAt(index);
                    Insert(index, value);
                }
                else
                {
                    // Append source collection
                    Add(value);
                }
            }
        }

        public int Count {
            get { return _vmCollection.Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        #endregion

        #region IList implementation
        bool IList.IsFixedSize {
            get { return false; }
        }

        bool IList.IsReadOnly {
            get { return IsReadOnly; }
        }

        int ICollection.Count {
            get { return Count; }
        }

        bool ICollection.IsSynchronized {
            get { return false; }
        }

        object ICollection.SyncRoot {
            get {
                return ((ICollection)_vmCollection).SyncRoot;
            }
        }

        object IList.this[int index] {
            get {
                return this[index];
            }

            set {
                if (value is TVM)
                {
                    this[index] = (TVM)value;
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        public virtual void Add(TVM item)
        {
            _vmCollection.Add(item);
            _initializer?.Invoke(item);
            _srcCollection.Add(item.Entity);
        }

        public void Clear()
        {
            _vmCollection.Clear();
            if (_filter != null)
            {
                var toRemove = _srcCollection.Where(_filter).ToArray();
                foreach (var removed in toRemove)
                {
                    _srcCollection.Remove(removed);
                }
            }
            else
            {
                _srcCollection.Clear();
            }
        }

        public bool Contains(TVM item)
        {
            return _vmCollection.Contains(item);
        }

        public void CopyTo(TVM[] array, int arrayIndex)
        {
            _vmCollection.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TVM> GetEnumerator()
        {
            return _vmCollection.GetEnumerator();
        }

        public int IndexOf(TVM item)
        {
            return _vmCollection.IndexOf(item);
        }

        public virtual void Insert(int index, TVM item)
        {
            _vmCollection.Insert(index, item);
            _initializer?.Invoke(item);
            _srcCollection.Add(item.Entity);
        }

        public virtual bool Remove(TVM item)
        {
            _srcCollection.Remove(item.Entity);
            return _vmCollection.Remove(item);
        }

        public virtual void RemoveAt(int index)
        {
            if (index < _vmCollection.Count)
            {
                var removed = _vmCollection[index];
                _srcCollection.Remove(removed.Entity);
            }
            _vmCollection.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _vmCollection.GetEnumerator();
        }

        int IList.Add(object value)
        {
            if (value is TVM)
            {
                Add((TVM)value);
                return Count - 1;
            }
            else
            {
                return -1;
            }
        }

        void IList.Clear()
        {
            Clear();
        }

        bool IList.Contains(object value)
        {
            if (value is TVM)
            {
                return Contains((TVM)value);
            }
            else
            {
                return false;
            }
        }

        int IList.IndexOf(object value)
        {
            if (value is TVM)
            {
                return IndexOf((TVM)value);
            }
            else
            {
                return -1;
            }
        }

        void IList.Insert(int index, object value)
        {
            if (value is TVM)
            {
                Insert(index, (TVM)value);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        void IList.Remove(object value)
        {
            if (value is TVM)
            {
                Remove((TVM)value);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        #endregion

        #region ICollection implementation
        void ICollection.CopyTo(Array array, int index)
        {
            var vmArray = _vmCollection.ToArray();
            Array.Copy(vmArray, 0, array, index, vmArray.Length);
        }

        #endregion
    }
}
