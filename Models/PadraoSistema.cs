using System;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class PadraoSistema
    {
        [JsonIgnore]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        [JsonIgnore]
        public int? CreatedBy { get; set; }
        [JsonIgnore]
        public DateTime? UpdatedAt { get; set; }
        [JsonIgnore]
        public int? UpdatedBy { get; set; }
        [JsonIgnore]
        public DateTime? DeletedAt { get; set; }
        [JsonIgnore]
        public int? DeletedBy { get; set; }
    }
}