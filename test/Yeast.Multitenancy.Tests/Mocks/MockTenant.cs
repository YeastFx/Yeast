namespace Yeast.Multitenancy.Tests.Mocks
{
    public class MockTenant :ITenant
    {
        private readonly string _identifier;

        public MockTenant(string identifier)
        {
            _identifier = identifier;
        }

        public string Identifier {
            get {
                return _identifier;
            }
        }
    }
}
