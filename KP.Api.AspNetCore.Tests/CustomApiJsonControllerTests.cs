using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KP.Api.AspNetCore.Tests
{
    public class CustomApiJsonControllerTests
    {
        [Fact]
        public void Кастомный_контроллер_может_переописать_поимку_ошибок()
        {
            var statusCode = HttpStatusCode.EarlyHints;
            var sut = new CustomApiJsonController(statusCode);

            var result = sut.Test();

            result.As<StatusCodeResult>().StatusCode.Should().Be((int)statusCode);
        }
    }
}
