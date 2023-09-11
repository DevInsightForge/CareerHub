using CareerHub.Application.Features.Common.ViewModels;
using CareerHub.Domain.Entities.User;
using CareerHub.gRPC.Protos;
using Google.Protobuf.WellKnownTypes;
using Mapster;

namespace CareerHub.gRPC.Extensions
{
    public static class MappingServiceExtension
    {
        public static void ConfigureMappings()
        {
            TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);
            TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);

            // Global Type Mappings
            TypeAdapterConfig<DateTimeOffset, Timestamp>.NewConfig()
                .MapWith(src => Timestamp.FromDateTimeOffset(src));

            // Specific Mappings
            TypeAdapterConfig<UserModel, UserResponse>
                .NewConfig()
                .Map(dest => dest.UserId, src => src.Id.Value);

            TypeAdapterConfig<UserModel, TokenUserModel>
                .NewConfig()
                .Map(dest => dest.UserId, src => src.Id.Value);
        }
    }
}
