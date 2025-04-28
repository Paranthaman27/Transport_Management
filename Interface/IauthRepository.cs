using Transport_Management.Models.DTO;
using Transport_Management.Models.Entity;

namespace Transport_Management.Interface
{
    public interface IauthRepository
    {
        ApiResponseDTO registerNewUser(mstUser user);
        ApiResponseDTO loginUser(userDTO user);
        ApiResponseDTO checkUserExistByEmail(string useremail);

    }
}
