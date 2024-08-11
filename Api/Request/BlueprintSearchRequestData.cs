using System.Collections.Generic;

namespace BPX.Api.Request;

public class BlueprintSearchRequestData
{
    public string Creator { get; set; } = null!;
    public List<string> Tags { get; set; } = null!;
    public List<string> Terms { get; set; } = null!;
}
