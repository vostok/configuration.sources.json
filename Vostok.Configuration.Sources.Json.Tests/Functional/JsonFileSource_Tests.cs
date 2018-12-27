using System;
using FluentAssertions.Extensions;
using NUnit.Framework;
using Vostok.Commons.Testing;
using Vostok.Commons.Testing.Observable;

namespace Vostok.Configuration.Sources.Json.Tests.Functional
{
    [TestFixture]
    internal class JsonFileSource_Tests
    {
        [Test]
        public void Should_work_correctly()
        {
            using (var temporaryFile = new TemporaryFile(TestCase.Json))
            {
                var source = new JsonFileSource(temporaryFile.FileName);
                source.Observe()
                    .ShouldStartWithIn(1.Seconds(),(TestCase.SettingsNode, null as Exception));
            }
        }
    }
}