using Insurance.App.Enums;
using Insurance.App.Interface;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Insurance.App.Scenarios;

public class CreateLicenceScenarios(ITelegramBotClient bot, IUserState userState) : IScenarios
{
    private readonly ITelegramBotClient _bot = bot;
    private readonly IUserState _userState = userState;

    //in this plot user parametr exist for swting in switch case for anser
    public async Task<Message> HandleAsync(Message message, UserState userState)
    {
        switch (userState)
        {
            case UserState.None:
                _userState.SetState(message.Chat, UserState.AwaitingPhotoPassport);
                return await _bot.SendMessage(message.Chat, "Messege passport is sended.");
            
            case UserState.AwaitingPhotoPassport:
                _userState.SetState(message.Chat, UserState.AwaitingPhotoDrivingLicense);
                return await _bot.SendMessage(message.Chat, "Dring licence is sended.");
        }
        
        //its mock data
        return await _bot.SendMessage(message.Chat, "Dring licence is sended.");
    }
}