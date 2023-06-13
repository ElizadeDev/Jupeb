namespace JupebPortal.Models
{
    public class RemitaStatusResponseRoot
    {
        public string Status { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public string IResponseCode { get; set; }
        public string IResponseMessage { get; set; }
        public string AppVersionCode { get; set; }
        public List<RemitaStatusResponse> ResponseData { get; set; }
        public object Data { get; set; }
    }

}
