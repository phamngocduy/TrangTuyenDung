using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TrangTuyenDung.Tests.Support {
    public class MockHelper {
        public Mock<HttpContextBase> Context { get; set; }
        public Mock<HttpSessionStateBase> Session { get; set; }
        public Mock<HttpRequestBase> Request { get; set; }
        //public Mock<ControllerContext> controllerContext { get; set; }
        public MockHelper() {
            Context = new Mock<HttpContextBase>();
            Session = new Mock<HttpSessionStateBase>();
            //controllerContext = new Mock<ControllerContext>();

        }
        public Mock<HttpContextBase> MakeFakeContext() {
            Context.SetupGet(c => c.Session).Returns(Session.Object);

            return Context;
        }
    }
}
