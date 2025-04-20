using Insurance.App.Abstract;
using Insurance.App.Enums;
using Insurance.App.Interface;
using Mindee;
using Mindee.Input;
using Mindee.Product.DriverLicense;
using Mindee.Product.InternationalId;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Insurance.App.Scenarios;


/// <summary>
/// Its scenario for buying licence in out bot
/// </summary>
public class CreateLicenceScenarios : IScenarios
{
    private readonly ITelegramBotClient _bot;
    private readonly IUserState _userState;
    private readonly IPhotoService _photoService;
    private readonly MindeeClient _mindeeClient;

    public CreateLicenceScenarios(
        ITelegramBotClient bot,
        IUserState userState,
        IPhotoService photoService
    )
    {
        _bot = bot;
        _userState = userState;
        _photoService = photoService;
        
        //todo: поміняти потрібно ключ
        var mindeeApiKey = "08552e670efa3fe3a3b257bd3061d6b5";
        _mindeeClient = new MindeeClient(mindeeApiKey);
    }

    public async Task<Message> HandleAsync(Message message, UserState userState)
    {
        return userState switch
        {
            UserState.Default => await StartLicenseFlow(message),
            UserState.AwaitingPhotoPassport => await HandlePassportPhoto(message),
            UserState.AwaitingPhotoDrivingLicense => await HandleDrivingLicensePhoto(message),
            UserState.AllowForApprove => await HandleApproval(message),
            UserState.OpenPurchesePipe => await HandlePurchase(message),
            _ => await _bot.SendMessage(
                chatId: message.Chat.Id,
                text: "Unexpected state. Please restart with /start")
        };
    }

    public async Task<Message> HandleCallbackAsync(CallbackQuery callback, UserState userState)
    {
        var chat = callback.Message?.Chat;
        if (chat == null)
            return await _bot.SendMessage(
                callback.From.Id,
                "Error processing callback: chat not found");

        var simulatedMessage = new Message
        {
            Chat = chat,
            Text = callback.Data
        };

        return await HandleAsync(simulatedMessage, userState);
    }

    private async Task<Message> StartLicenseFlow(Message message)
    {
        _userState.SetState(message.Chat, UserState.AwaitingPhotoPassport);
        return await _bot.SendMessage(
            message.Chat.Id,
            "Please send the photo of passport");
    }

    private async Task<Message> HandlePassportPhoto(Message message)
    {
        if (message.Type == MessageType.Photo && message.Photo != null)
        {
            var photo = message.Photo.Last();
            _userState.AddPhoto(message.Chat, photo);
        }

        _userState.SetState(message.Chat, UserState.AwaitingPhotoDrivingLicense);
        return await _bot.SendMessage(
            message.Chat.Id,
            "Passport is sent. Please send a driving license.");
    }

    private async Task<Message> HandleDrivingLicensePhoto(Message message)
    {
        if (message.Type == MessageType.Photo && message.Photo != null)
        {
            var photo = message.Photo.Last();
            _userState.AddPhoto(message.Chat, photo);
        }

        //тут можна добавити таку штуку, як oadim
        await _bot.SendMessage(
            message.Chat.Id,
            "You sent all required data. <b>Processing...</b> Please wait some time for the data to be processed.",
            parseMode: ParseMode.Html
        );

        try
        {
            var result = await ProcessDocuments(message.Chat);

            var inlineMarkup = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Yes", "Yes"),
                    InlineKeyboardButton.WithCallbackData("No", "No")
                }
            });

            _userState.SetState(message.Chat, UserState.AllowForApprove);

            return await _bot.SendMessage(
                message.Chat.Id,
                "Does everything look good and are you happy with the result?",
                ParseMode.Html,
                replyMarkup: inlineMarkup
            );
        }
        catch (Exception ex)
        {
            _userState.SetState(message.Chat, UserState.AwaitingPhotoPassport);
            return await _bot.SendMessage(
                message.Chat.Id,
                $"Error processing documents: {ex.Message}. Please try again."
            );
        }
    }

    private async Task<string> ProcessDocuments(Chat chat)
    {
        var savedPhotos = _userState.GetPhotos(chat);

        var passportInput = await _photoService.DownloadPhotoAsInputSource(
            _bot, savedPhotos[0].FileId, "passport.jpg");
        var passportResponse = await _mindeeClient.EnqueueAndParseAsync<InternationalIdV2>(passportInput);

        var licenseInput = await _photoService.DownloadPhotoAsInputSource(
            _bot, savedPhotos[1].FileId, "driving_license.jpg");
        var licenseResponse = await _mindeeClient.EnqueueAndParseAsync<DriverLicenseV1>(licenseInput);

        await _bot.SendMessage(
            chat,
            "<b>✅ Passport</b>:\n" + passportResponse.Document,
            parseMode: ParseMode.Html
        );

        await _bot.SendMessage(
            chat,
            "\n\n<b>✅ License</b>:\n" + licenseResponse.Document,
            parseMode: ParseMode.Html
        );

        return passportResponse.Document + "\n" + licenseResponse.Document;
    }

    private async Task<Message> HandleApproval(Message message)
    {
        if (message.Text == "No" || message.Text == "/No")
        {
            _userState.SetState(message.Chat, UserState.AwaitingPhotoPassport);
            return await _bot.SendMessage(
                message.Chat.Id,
                "Please try to take photos again with better lighting."
            );
        }

        if (message.Text == "Yes" || message.Text == "/Yes")
        {
            _userState.SetState(message.Chat, UserState.OpenPurchesePipe);

            var inlineMarkup = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Yes", "Yes"),
                    InlineKeyboardButton.WithCallbackData("No", "No")
                }
            });

            return await _bot.SendMessage(
                message.Chat.Id,
                "Our standard subscription price is $100. Are you willing to pay this price?",
                replyMarkup: inlineMarkup
            );
        }

        return await _bot.SendMessage(
            message.Chat.Id,
            "Please select Yes or No");
    }

    private async Task<Message> HandlePurchase(Message message)
    {
        if (message.Text == "No" || message.Text == "/No")
        {
            _userState.SetState(message.Chat, UserState.AllowForApprove);
            return await _bot.SendMessage(
                message.Chat.Id,
                "Our system has a minimum price of $100."
            );
        }

        if (message.Text == "Yes" || message.Text == "/Yes")
        {
            _userState.SetState(message.Chat, UserState.None);

            // Generate PDF document using OpenAI or other service
            var pdfContent = await GeneratePdfDocument(message.Chat);

            return await _bot.SendMessage(
                message.Chat.Id,
                pdfContent);
        }

        return await _bot.SendMessage(
            message.Chat.Id,
            "Please select Yes or No");
    }

    private async Task<string> GeneratePdfDocument(ChatId chatId)
    {
        // TODO: Ось тут потрібно добавити певні штуки, для того, що б чат дпт генерував повідомлення 

        return "Your insurance document has been generated and will be sent shortly.";
    }
}