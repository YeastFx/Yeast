using System;

namespace Yeast.WebApi
{
    /// <summary>
    /// Base class to provide view models
    /// </summary>
    /// <typeparam name="T">Type of underlying entity</typeparam>
    public abstract class BaseViewModel<T>
        where T : new()
    {
        /// <summary>
        /// The underlying entity
        /// </summary>
        protected readonly T entity;

        internal T Entity {
            get => entity;
        }

        /// <summary>
        /// Creates a view model based on a new entity
        /// </summary>
        public BaseViewModel() {
            entity = Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Creates view model for an existing entity
        /// </summary>
        /// <param name="entity"></param>
        public BaseViewModel(T entity)
        {
            this.entity = entity;
        }
    }
}
