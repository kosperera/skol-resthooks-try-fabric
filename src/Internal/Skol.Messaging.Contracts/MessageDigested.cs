using System;
using System.Collections.Generic;

namespace Skol.Messaging.Contracts
{
    public sealed class MessageDigested
    {
        public IDictionary<string, string> Headers { get; set; }
        public string EventKind { get; set; }
        public string Environment { get; set; }
        public DateTimeOffset OccurredAsOf { get; set; }

        public string NotificationUrl { get; set; }
        public object Content { get; set; }
        public string ContentSignature { get; set; }
    }
}
