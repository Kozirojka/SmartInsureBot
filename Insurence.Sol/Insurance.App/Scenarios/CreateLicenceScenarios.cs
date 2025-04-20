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
            case UserState.Default:
                _userState.SetState(message.Chat, UserState.AwaitingPhotoPassport);
                
                //todo: тут потрібно буде зберігати до колекції у пам'яті
                return await _bot.SendMessage(message.Chat, "Please send the photo of passport");
            
            case UserState.AwaitingPhotoPassport:
                _userState.SetState(message.Chat, UserState.AwaitingPhotoDrivingLicense);
                // тут також збергіаєм у память і після цього відправляю до штучного інтеоекту
                
                //туту буде виконуватись штучний інтелект вирахоування певних даних
                return await _bot.SendMessage(message.Chat, "Passport is sent.");
            
            
            case UserState.AwaitingPhotoDrivingLicense:
                _userState.SetState(message.Chat, UserState.AllowForApprove);
                
                await _bot.SendMessage(message.Chat, "Driving license is sent please " +
                                                     "wait some time for proceed of data.");
                
                // todo: тут ми відправляємо наші дані до AI штучного
                // інтелекту та очікуємо на вибірку даних
                // що він відішле нам

                var dataFromThePhoto = "Some shit with this";
                
                
                // ось тут можна буде зробити для користувача дві кноки так або ні, типу як inline
                return await _bot.SendMessage(message.Chat, dataFromThePhoto);
            
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
}