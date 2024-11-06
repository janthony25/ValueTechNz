using ValueTechNz.Models.Dto;

namespace ValueTechNz.Repository.IRepository
{
    public interface IAccountRepository
    {
        Task RegisterAsync(RegisterDto registerDto);
    }
}
