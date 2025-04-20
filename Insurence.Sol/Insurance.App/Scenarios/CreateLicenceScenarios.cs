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

public class CreateLicenceScenarios(ITelegramBotClient bot, IUserState userState, IPhotoService photoService) : IScenarios
{
    private readonly ITelegramBotClient _bot = bot;
    private readonly IUserState _userState = userState;

    MindeeClient mindeeClient = new ("08552e670efa3fe3a3b257bd3061d6b5");
    //in this plot user parametr exist for swting in switch case for anser
    public async Task<Message> HandleAsync(Message message, UserState userState)
    {
        switch (userState)
        {
            case UserState.Default:
                _userState.SetState(message.Chat, UserState.AwaitingPhotoPassport);
                
                //todo: тут потрібно буде зберігати до колекції у пам'яті
                return await _bot.SendMessage(message.Chat, "Please send the photo of passport");
            
            case UserState.AwaitingPhotoPassport:
                _userState.SetState(message.Chat, UserState.AwaitingPhotoDrivingLicense);
                
                
                if (message.Type == MessageType.Photo && message.Photo != null)
                {
                    var photo = message.Photo.Last();
                    
                    _userState.AddPhoto(message.Chat, photo);
                }
                
                //туту буде виконуватись штучний інтелект вирахоування певних даних
                return await _bot.SendMessage(message.Chat, "Passport is sent. Please send an driving license.");
            
            
            case UserState.AwaitingPhotoDrivingLicense:
            
                if (message.Type == MessageType.Photo && message.Photo != null)
                {
                    var photo = message.Photo.Last();
                    
                    _userState.AddPhoto(message.Chat, photo);
                }
                await _bot.SendMessage(
                    message.Chat,
                    "You sent all required data.<b>Processing...</b>" +
                    "Please wait some time for the data to be processed.",
                    ParseMode.Html
                );
            
                var savedPhotos = _userState.GetPhotos(message.Chat);
            
                var passportInput = await photoService.DownloadPhotoAsInputSource(_bot, savedPhotos[0].FileId, "passport.jpg");
            
                var passportResponse = await mindeeClient.EnqueueAndParseAsync<InternationalIdV2>(passportInput);
            
                var licenseInput = await photoService.DownloadPhotoAsInputSource(_bot, savedPhotos[1].FileId, "driving_license.jpg");
            
                var licenseResponse = await mindeeClient.EnqueueAndParseAsync<DriverLicenseV1>(licenseInput);
            
                Console.WriteLine("Driving License:");
                Console.WriteLine(licenseResponse.Document.ToString());
            
                await _bot.SendMessage(
                    message.Chat,
                    "<b>✅ Passport</b>:\n" + passportResponse.Document.ToString(),
                    ParseMode.Html
                );
                
                 await _bot.SendMessage(
                    message.Chat,
                     "\n\n<b>✅ License</b>:\n" + licenseResponse.Document.ToString(),
                    ParseMode.Html
                );

                var inlineMarkup = new InlineKeyboardMarkup()
                    .AddNewRow()
                    .AddButton("Yes", "Yes")
                    .AddButton("No", "No");
                
                _userState.SetState(message.Chat, UserState.AllowForApprove);

                return await _bot.SendMessage(
                    message.Chat,
                    "Does everything is good and you are happy with result",
                    ParseMode.Html,
                    replyMarkup: inlineMarkup
                );
                
            case UserState.AllowForApprove:
                
                if(message.Text == "No")
                {
                    _userState.SetState(message.Chat, UserState.AwaitingPhotoPassport);
                    return await _bot.SendMessage(message.Chat, "Спробуйте будь ласка" +
                                                                " перефотографувати під кращим освітленям");
                }

                if (message.Text == "Yes")
                {
                    _userState.SetState(message.Chat, UserState.OpenPurchesePipe);
                    return await _bot.SendMessage(message.Chat, "У нас стандартна ціна " +
                                                                "на підписку становить 100$, " +
                                                                "чи згідні ви оплатити таку ціну?");
                }
                break;
            case UserState.OpenPurchesePipe:
                
                if(message.Text == "No")
                {
                    _userState.SetState(message.Chat, UserState.AllowForApprove);
                    return await _bot.SendMessage(message.Chat, "У нас в системі мінімальна сума становить 100 доларів");
                }

                if (message.Text == "Yes")
                {
                    _userState.SetState(message.Chat, UserState.None);
                    
                    //todo: робимо тут запит до OpenAi щоб він нам нагенерував pdf файд

                    var result = "Some Pdf File Data";
                    return await _bot.SendMessage(message.Chat, result);
                }
                break;
        }
        
        //its mock data
        return await _bot.SendMessage(message.Chat, "Dring licence is sended.");
    }

    public async Task<Message> HandleCallbackAsync(CallbackQuery callback, UserState userState)
    {
        var chat = callback.Message?.Chat;
    
        switch (userState)
        {
            case UserState.AllowForApprove:
                if (callback.Data == "No")
                {
                    _userState.SetState(chat, UserState.AwaitingPhotoPassport);
                    return await _bot.SendMessage(chat, "Спробуйте будь ласка перефотографувати під кращим освітленням");
                }
    
                if (callback.Data == "Yes")
                {
                    _userState.SetState(chat, UserState.OpenPurchesePipe);
                    return await _bot.SendMessage(chat, "У нас стандартна ціна на підписку становить 100$, чи згідні ви оплатити таку ціну?");
                }
                break;
    
            case UserState.OpenPurchesePipe:
                if (callback.Data == "No")
                {
                    _userState.SetState(chat, UserState.AllowForApprove);
                    return await _bot.SendMessage(chat, "У нас в системі мінімальна сума становить 100 доларів");
                }
    
                if (callback.Data == "Yes")
                {
                    _userState.SetState(chat, UserState.None);
                    var result = "Some Pdf File Data"; 
                    return await _bot.SendMessage(chat, result);
                }
                break;
        }
    
        return await _bot.SendMessage(chat, "Callback received but not handled properly.");
    }

}