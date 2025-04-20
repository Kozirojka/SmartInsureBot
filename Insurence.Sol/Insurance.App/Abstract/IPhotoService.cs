using Mindee.Input;
using Telegram.Bot;

namespace Insurance.App.Abstract;

public interface IPhotoService
{
    Task<LocalInputSource> DownloadPhotoAsInputSource(ITelegramBotClient bot, string fileId, string fileName);
}