namespace Yeast.Multitenancy
{
    public interface ITenant
    {
        /// <summary>
        /// Unique string identifier of the tenant
        /// </summary>
        string Identifier { get; }
    }
}
