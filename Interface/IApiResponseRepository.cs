using Transport_Management.Models.DTO;

namespace Transport_Management.Interface
{
    public interface IApiResponseRepository 
    {
        ApiResponseDTO SuccessResponse(ApiResponseDTO responseInfo);
        ApiResponseDTO FailureResponse(ApiResponseDTO responseInfo);
    }
}
