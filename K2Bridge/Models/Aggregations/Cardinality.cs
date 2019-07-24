﻿namespace K2Bridge.Models.Aggregations
{
    using Newtonsoft.Json;

    [JsonConverter(typeof(JustFieldConverter))]
    internal class Cardinality : MetricAggregation
    {
        [JsonProperty("field")]
        public string FieldName { get; set; }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}