using CareerHub.Domain.Entities.User;
using CareerHub.Shared.Protos;
using Mapster;

namespace CareerHub.Application.Configurations
{
    public static class MappingConfigurations
    {
        public static void ConfigureMappings()
        {
            TypeAdapterConfig<UserModel, UserResponse>
                .NewConfig()
                .Map(dest => dest.UserId, src => src.Id.Value);
        }
    }
}
