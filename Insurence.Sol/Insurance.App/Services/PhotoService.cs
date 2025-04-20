using Insurance.App.Abstract;
using Mindee.Input;
using Telegram.Bot;

namespace Insurance.App.Interface;

/// <summary>
/// We need this class for parsing photo
/// I use such approach, becaues there can be defferent kind of downloading files
/// </summary>
public class PhotoService : IPhotoService
{
    public async Task<LocalInputSource> DownloadPhotoAsInputSource(ITelegramBotClient bot, string fileId, string fileName)
    {
        using var ms = new MemoryStream();

        await bot.GetInfoAndDownloadFile(fileId, ms);

        ms.Position = 0;

        var fileBytes = ms.ToArray();

        return new LocalInputSource(fileBytes, fileName);
    }

}