using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Net;

namespace KP.Api.AspNetCore.Tests
{
    public class CustomApiJsonController : ApiJsonController
    {
        private readonly HttpStatusCode _statusCodeToReturn;

        public CustomApiJsonController(HttpStatusCode statusCodeToReturn) : base(new Mock<ILogger<ApiJsonController>>().Object)
        {
            _statusCodeToReturn = statusCodeToReturn;
        }

        public IActionResult Test() => ExecuteAction(() => { throw new TestException(); });

        protected override IActionResult Wrap(Func<IActionResult> getActionResult)
        {
            try
            {
                return getActionResult();
            }
            catch (TestException ex)
            {
                return StatusCode((int)_statusCodeToReturn);
            }
        }
    }
}
