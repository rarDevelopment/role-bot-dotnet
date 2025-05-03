using RoleBot.BusinessLayer;
using RoleBot.Helpers;

namespace RoleBot.Commands;

public class ToggleEnableColorChoosingCommand(IConfigurationBusinessLayer configurationBusinessLayer, RoleHelper roleHelper)
    : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("set-color-choosing", "Set color choosing on or off.")]
    public async Task ToggleEnableColorChoosing(
        [Summary("is_enabled", "True for ON, False for OFF")] bool isEnabled)
    {
        var member = Context.Guild.Users.FirstOrDefault(u => u.Id == Context.User.Id);
        if (member == null)
        {
            await RespondAsync("Hmm, something is wrong, you aren't able to do that.");
            return;
        }

        if (!await roleHelper.CanAdministrate(Context.Guild, member))
        {
            await RespondAsync("You do not have permission to change this setting!");
        }
        else
        {
            var success =
                await configurationBusinessLayer.SetEnableColorChoosing(Context.Guild.Id.ToString(), isEnabled);
            if (success)
            {
                await RespondAsync($"Color Choosing is now {(isEnabled ? "ON" : "OFF")}.");
                return;
            }

            await RespondAsync("Sorry, I wasn't able to do that. There was an error.");
        }
    }
}