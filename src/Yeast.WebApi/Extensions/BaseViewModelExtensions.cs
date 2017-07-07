namespace Yeast.WebApi
{
    public static class BaseViewModelExtensions
    {
        /// <summary>
        /// Gets the underlying entity of a <see cref="BaseViewModel{T}"/>
        /// </summary>
        /// <typeparam name="T">Type of underlying entity</typeparam>
        /// <param name="viewModel">The View Model</param>
        /// <returns>The underlying entity.</returns>
        public static T GetUnderlyingEntity<T>(this BaseViewModel<T> viewModel)
            where T: new()
        {
            return viewModel.Entity;
        }
    }
}
