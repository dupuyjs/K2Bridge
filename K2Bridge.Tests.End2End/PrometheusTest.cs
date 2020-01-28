// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace K2Bridge.Tests.End2End
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using static System.StringComparison;

    public class PrometheusTest : KustoTestBase
    {
        [Test]
        [Description("Expose exception counters")]
        public async Task When_ExceptionLogged_Then_IncrementExceptionCounter()
        {
            using var request1 = new HttpRequestMessage(HttpMethod.Post, "_msearch");
            var payload = new StringBuilder();
            payload.AppendLine($"{{\"index\":\"dummy\"}}");
            payload.AppendLine("{\"badly_formatted_json\":}");
            request1.Content = new StringContent(payload.ToString());
            request1.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-ndjson");
            await K2Client().Client().SendAsync(request1);

            using var request = new HttpRequestMessage(HttpMethod.Get, "/metrics");
            var response = await K2Client().Client().SendAsync(request);
            var responseData = await response.Content.ReadAsStringAsync();
            Assert.True(
                responseData.Contains(
                "# HELP exceptions_by_type Exceptions, by type\n"
                + "# TYPE exceptions_by_type counter\n"
                + @"exceptions_by_type{ExceptionType=""JsonReaderException"",SourceContext=""K2Bridge.ElasticQueryTranslator"",ActionName=""K2Bridge.Controllers.QueryController.Search (K2Bridge)""} ",
                Ordinal), responseData);
            Assert.True(
                responseData.Contains(
                "# HELP exceptions Exceptions logged\n"
                + "# TYPE exceptions counter\n"
                + "exceptions ",
                Ordinal));
        }
    }
}
