using LiteDB;

namespace Lyra.Features.Config
{
    public class PresenterConfig
    {
        [BsonIgnore]
        public const string ConfigId = "PresenterConfig";

        public string Id { get; set; } = ConfigId;

        public string SelectedScreen { get; set; }
    }
}