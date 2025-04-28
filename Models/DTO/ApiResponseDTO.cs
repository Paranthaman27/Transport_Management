namespace Transport_Management.Models.DTO
{
    public class ApiResponseDTO
    {
        public bool success {  get; set; }
        public string message {  get; set; }
        public int statusCode {  get; set; }
        public dynamic data {  get; set; }
    }
}
