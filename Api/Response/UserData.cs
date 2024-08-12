using System;

namespace BPX.Api.Response
{
    public class UserData
    {
        public int Id { get; set; }

        public decimal SteamId { get; set; }

        public string SteamName { get; set; } = null!;

        public bool Banned { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }
    }
}
