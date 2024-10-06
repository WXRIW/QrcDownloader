using Lyricify.Lyrics.Helpers.General;
using Microsoft.VisualBasic;

Console.WriteLine("QRC Downloader by WXRIW");
Console.WriteLine("Version 0.1-alpha\n");
Console.WriteLine("Powered by Lyricify Lyrics Helper (https://github.com/WXRIW/Lyricify-Lyrics-Helper)");
Console.WriteLine("Open source at https://github.com/WXRIW/QrcDownloader");
Console.WriteLine("Enter 'exit' to close this app.\n");
Console.WriteLine("-----------------------------------------------------\n");

while (true)
{
    try
    {
        Console.Write("Enter QQ Music song link, id or mid: ");
        var mid = Console.ReadLine();
        mid = mid?.Trim();
        if (string.IsNullOrEmpty(mid)) continue;
        if (mid.ToLower() == "exit") return;

        if (mid.Length > 14)
        {
            string s = mid;
            if (s.Contains("mid="))
            {
                s = s[(s.IndexOf("mid=") + 8)..];
                if (s.Contains('&')) s = s[..s.IndexOf('&')];
            }

            if (s.Contains("song/"))
            {
                s = s[(s.IndexOf("song/") + 5)..];
                if (s.Contains('.')) s = s[..s.IndexOf('.')];
            }
            else if (s.Contains("songDetail/"))
            {
                s = s[(s.IndexOf("songDetail/") + 11)..];
                if (s.Contains('.')) s = s[..s.IndexOf('.')];
            }

            if (s.Length == 14 || s.IsNumber())
            {
                mid = s;
            }
            else
            {
                if (mid.StartsWith("https://c") && mid.Contains(".y.qq.com/base"))
                {
                    string webStr = await new HttpClient().GetStringAsync(mid);
                    if (webStr.Contains("mid\":\""))
                    {
                        s = webStr;
                        s = s[(s.IndexOf("mid\":\"") + 6)..];
                        s = s[..s.IndexOf("\"")];
                        mid = s;
                    }
                    if (mid.Length != 14)
                    {
                        continue;
                    }
                }
                if (mid.Length != 14)
                {
                    continue;
                }
            }
        }

        // 获取歌词
        var song = await Lyricify.Lyrics.Helpers.ProviderHelper.QQMusicApi.GetSong(mid);
        if (song is null) continue;
        Console.WriteLine($"\nTrack: {song.Data[0].Name}");
        Console.WriteLine($"Artist: {string.Join(", ", song.Data[0].Singer.Select(t => t.Title))}");
        Console.WriteLine($"Id: {song.Data[0].Id}");
        Console.WriteLine($"Mid: {song.Data[0].Mid}\n");

        var lyrics = await Lyricify.Lyrics.Helpers.ProviderHelper.QQMusicApi.GetLyricsAsync(song.Data[0].Id);
        if (lyrics is null) continue;
        Console.WriteLine("Lyrics:");
        Console.WriteLine(lyrics.Lyrics?.Trim());
        if (!string.IsNullOrWhiteSpace(lyrics.Trans))
        {
            Console.WriteLine("Translation:");
            Console.WriteLine(lyrics.Trans);
        }
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}