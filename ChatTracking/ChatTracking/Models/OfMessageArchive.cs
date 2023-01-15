using System;
using System.Collections.Generic;

namespace ChatTracking.Models;

public partial class OfMessageArchive
{
    public long ConversationId { get; set; }

    public string FromJid { get; set; } = null!;

    public string ToJid { get; set; } = null!;

    public long SentDate { get; set; }

    public string? Body { get; set; }
}
