using System;
using FluentAssertions.Extensions;
using NUnit.Framework;
using Vostok.Commons.Testing.Observable;

namespace Vostok.Configuration.Sources.Json.Tests.Functional
{
    internal class JsonStringSource_Tests
    {
        [Test]
        public void Should_work_correctly()
        {
            var source = new JsonStringSource(TestCase.Json);
            
            source.Observe()
                .ShouldStartWithIn(1.Seconds(),(TestCase.SettingsNode, null as Exception));
        }
    }
}