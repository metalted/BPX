using System.Collections.Generic;

namespace BPX.Api.Request
{

    public class BlueprintSubmitRequestData
    {
        public string Name { get; set; } = null!;
        public List<string> Tags { get; set; } = null!;
        public string BlueprintBase64 { get; set; } = null!;
        public string ImageBase64 { get; set; } = null!;
    }
}
