﻿using System;

namespace ExpressionTrees.Task2.ExpressionMapping.Tests.Models
{
    public class Foo
    {
        public string Name { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string Suite { get; set; }

        public Guid Id { get; set; }

        [Map("Foreign", typeof(string))]
        public Guid ForeignId { get; set; }

        [Map("TRA", typeof(byte))]
        public int Tratarara { get; set; }

        public int Count { get; set; }

        public string BreakProperty { get; set; }
    }
}
