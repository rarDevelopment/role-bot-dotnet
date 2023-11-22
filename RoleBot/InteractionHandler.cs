using System.Reflection;

namespace RoleBot;

public class InteractionHandler(DiscordSocketClient client, InteractionService handler, IServiceProvider services, ILogger<InteractionHandler> logger)
{
    private readonly ILogger _logger = logger;

    public async Task InitializeAsync()
    {
        await handler.AddModulesAsync(Assembly.GetEntryAssembly(), services);

        client.InteractionCreated += HandleInteraction;
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(client, interaction);

            var result = await handler.ExecuteCommandAsync(context, services);

            if (!result.IsSuccess)
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        _logger.LogInformation($"Unmet precondition - {result.Error}");
                        break;
                }
        }
        catch
        {
            if (interaction.Type is InteractionType.ApplicationCommand)
                await interaction.GetOriginalResponseAsync().ContinueWith(async msg
                    => await msg.Result.DeleteAsync());
        }
    }
}