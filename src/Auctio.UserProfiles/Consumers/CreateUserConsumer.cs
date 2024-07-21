using MassTransit;
using Auctio.Shared.Masstransit;

namespace Auctio.UserProfiles.Consumers;

public class CreateUserConsumer : IConsumer<CreateUser>
{
    private readonly Repositories.UserProfileRepository _userProfileRepository;
    public CreateUserConsumer(Repositories.UserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    public async Task Consume(ConsumeContext<CreateUser> context)
    {
        var msg = context.Message;
        await _userProfileRepository.CreateUser(msg.UserId, msg.Username, msg.CreatedAt);
    }
}