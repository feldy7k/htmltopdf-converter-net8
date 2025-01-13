using System.Text.Json.Serialization;

namespace HtmlToPdfFile.Request
{
    // author: feldy judah k
    // .NET 8

    public class PdfParameter
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("mobile_phone")]
        public string MobilePhone { get; set; }

        [JsonPropertyName("date_of_birth")]
        public string DateOfBirth { get; set; }

        [JsonPropertyName("occupation")]
        public string Occupation { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

    }
}
