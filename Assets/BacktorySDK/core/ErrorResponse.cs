using RestSharp.Deserializers;

namespace Assets.BacktorySDK.core
{
    internal class ErrorResponse
    {
        public string Error { get; set; }

        [DeserializeAs(Name ="error_description")]
        public string ErrorDescription { get; set; }
    }
}
