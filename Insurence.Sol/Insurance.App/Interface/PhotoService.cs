using Mindee.Input;
using Telegram.Bot;

namespace Insurance.App.Interface;

public class PhotoService : IPhotoService
{
    public async Task<LocalInputSource> DownloadPhotoAsInputSource(ITelegramBotClient bot, string fileId, string fileName)
    {
        using var ms = new MemoryStream();

        var downloadedFile = await bot.GetInfoAndDownloadFile(fileId, ms);

        ms.Position = 0;

        var fileBytes = ms.ToArray();

        return new LocalInputSource(fileBytes, fileName);
    }

}

public interface IPhotoService
{
    Task<LocalInputSource> DownloadPhotoAsInputSource(ITelegramBotClient bot, string fileId, string fileName);
}