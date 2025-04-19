using Insurance.App.Scenario;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Insurance.App.Scenarios;

public class PurchaseInsuranceScenario : IScenario
{
    private readonly ITelegramBotClient _bot;
    private readonly IUserStateService _userState;

    public string ScenarioKey => "purchase_insurance";

    public PurchaseInsuranceScenario(ITelegramBotClient bot, IUserStateService userState)
    {
        _bot = bot;
        _userState = userState;
    }

    public async Task HandleAsync(Update update)
    {
        // var userId = update.Id();
        //
        // string? currentState = _userState.GetState(userId);
        //
        // if (update.Message?.Text == "/cancel")
        // {
        //     _userState.ClearState(userId);
        //     await _bot.SendTextMessageAsync(update.GetChatId(), "❌ Сценарій скасовано.");
        //     return;
        // }
        //
        // switch (currentState)
        // {
        //     case null:
        //         _userState.SetState(userId, "awaiting_name");
        //         await _bot.SendTextMessageAsync(update.GetChatId(), "🧾 Введіть ваше ПІБ:");
        //         break;
        //
        //     case "awaiting_name":
        //         var fullName = update.Message?.Text;
        //         _userState.SetState(userId, "awaiting_photo");
        //         await _bot.SendTextMessageAsync(update.GetChatId(), $"📸 Тепер надішліть фото документів, {fullName}");
        //         break;
        //
        //     case "awaiting_photo":
        //         if (update.Message?.Photo is not null)
        //         {
        //             _userState.ClearState(userId);
        //             await _bot.SendTextMessageAsync(update.GetChatId(), "✅ Документи отримано. Генерую поліс...");
        //             // тут можна викликати OpenAI / PDF генерацію
        //         }
        //         else
        //         {
        //             await _bot.SendTextMessageAsync(update.GetChatId(), "⚠️ Надішліть саме фото, будь ласка.");
        //         }
        //         break;
        //}
    }

    public async Task HandleAsync(Message message, CancellationToken cancellationToken)
    {
        var userId = message.Chat.Id;
        
        var currentState = _userState.GetUserState(userId);
        
        if (message?.Text == "/cancel")
        {
            _userState.ClearUserState(userId);
            await _bot.SendMessage(message.Chat, "❌ Your plot is canceled", ParseMode.Html, cancellationToken: cancellationToken);
        }

        switch (currentState)
        {
            case UserState.none:
                _userState.SetUserState(userId, UserState.awaiting_photo_passport);
                await _bot.SendMessage(message.Chat, "Please send a photo of your passport");
        }
    }

    public UserState State { get; }
}
