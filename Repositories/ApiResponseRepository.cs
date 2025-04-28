using Transport_Management.Interface;
using Transport_Management.Models.DTO;
using System.Net;

namespace Transport_Management.Repositories
{
    public class ApiResponseRepository : IApiResponseRepository
    {
       
        public ApiResponseDTO SuccessResponse(ApiResponseDTO responseDTO)
        {
            ApiResponseDTO responseDTOv2 = new ApiResponseDTO();
            responseDTOv2.statusCode = (int)HttpStatusCode.OK;
            responseDTOv2.success = true;
            responseDTOv2.message = responseDTO.message;
            responseDTOv2.data = responseDTO.data;
            return responseDTOv2;
        }
        public ApiResponseDTO FailureResponse(ApiResponseDTO responseDTO)
        {
            ApiResponseDTO responseDTOv2 = new ApiResponseDTO();
            responseDTOv2.statusCode = (int)HttpStatusCode.BadRequest;
            responseDTOv2.success = false;
            responseDTOv2.message = responseDTO.message;
            responseDTOv2.data = responseDTO.data;
            return responseDTOv2;
        }
    }
}
