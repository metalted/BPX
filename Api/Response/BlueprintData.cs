using System;
using System.Collections.Generic;

namespace BPX.Api.Response;

public class BlueprintData
{
    public int Id { get; set; }

    public int IdUser { get; set; }

    public string Name { get; set; } = null!;

    public List<string> Tags { get; set; } = null!;

    public string FileId { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }
    
    public UserData User { get; set; } = null!;
}
