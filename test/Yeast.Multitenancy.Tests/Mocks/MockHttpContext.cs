using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features;
using System.Security.Claims;
using System.Threading;

namespace Yeast.Multitenancy.Tests.Mocks
{
    public class MockHttpContext : HttpContext
    {
        private readonly HttpRequest _request;

        public MockHttpContext(HttpRequest request) {
            _request = request;
        }

        public override AuthenticationManager Authentication {
            get {
                throw new NotImplementedException();
            }
        }

        public override ConnectionInfo Connection {
            get {
                throw new NotImplementedException();
            }
        }

        public override IFeatureCollection Features {
            get {
                throw new NotImplementedException();
            }
        }

        public override IDictionary<object, object> Items {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public override HttpRequest Request {
            get {
                return _request;
            }
        }

        public override CancellationToken RequestAborted {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public override IServiceProvider RequestServices { get; set; }

        public override HttpResponse Response {
            get {
                throw new NotImplementedException();
            }
        }

        public override ISession Session {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public override string TraceIdentifier {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public override ClaimsPrincipal User {
            get {
                throw new NotImplementedException();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public override WebSocketManager WebSockets {
            get {
                throw new NotImplementedException();
            }
        }

        public override void Abort()
        {
            throw new NotImplementedException();
        }
    }
}
