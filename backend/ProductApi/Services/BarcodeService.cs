using System.Drawing;
using System.Drawing.Imaging;

namespace ProductApi.Services;

public interface IBarcodeService
{
    string GenerateCode39Base64(string text);
}

public class BarcodeService : IBarcodeService
{
    // bar code 39
    private static readonly Dictionary<char, string> Code39Patterns = new()
    {
        { '0', "nnnwwnwnn" }, { '1', "wnnwnnnnw" }, { '2', "nnwwnnnnw" },
        { '3', "wnwwnnnnn" }, { '4', "nnnwwnnnw" }, { '5', "wnnwwnnnn" },
        { '6', "nnwwwnnnn" }, { '7', "nnnwnnwnw" }, { '8', "wnnwnnwnn" },
        { '9', "nnwwnnwnn" }, { 'A', "wnnnnwnnw" }, { 'B', "nnwnnwnnw" },
        { 'C', "wnwnnwnnn" }, { 'D', "nnnnwwnnw" }, { 'E', "wnnnwwnnn" },
        { 'F', "nnwnwwnnn" }, { 'G', "nnnnnwwnw" }, { 'H', "wnnnnwwnn" },
        { 'I', "nnwnnwwnn" }, { 'J', "nnnnwwwnn" }, { 'K', "wnnnnnnww" },
        { 'L', "nnwnnnnww" }, { 'M', "wnwnnnnwn" }, { 'N', "nnnnwnnww" },
        { 'O', "wnnnwnnwn" }, { 'P', "nnwnwnnwn" }, { 'Q', "nnnnnnwww" },
        { 'R', "wnnnnnwwn" }, { 'S', "nnwnnnwwn" }, { 'T', "nnnnwnwwn" },
        { 'U', "wwnnnnnnw" }, { 'V', "nwwnnnnnw" }, { 'W', "wwwnnnnnn" },
        { 'X', "nwnnwnnnw" }, { 'Y', "wwnnwnnnn" }, { 'Z', "nwwnwnnnn" },
        { '-', "nwnnnnwnw" }, { '.', "wwnnnnwnn" }, { ' ', "nwwnnnwnn" },
        { '*', "nwnnwnwnn" }
    };

    public string GenerateCode39Base64(string text)
    {
        text = text.ToUpperInvariant().Replace("-", "");
        var encoded = "*" + text + "*";

        int barWidth = 2;
        int wideWidth = barWidth * 3;
        int height = 80;
        int padding = 10;

        // Calculate width
        int totalWidth = padding * 2;
        foreach (var ch in encoded)
        {
            if (Code39Patterns.TryGetValue(ch, out var pattern))
            {
                foreach (var p in pattern)
                    totalWidth += (p == 'w' ? wideWidth : barWidth) + 1;
                totalWidth += barWidth; // inter-char gap
            }
        }

        using var bmp = new Bitmap(totalWidth, height + 20);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.White);

        int x = padding;
        bool isBar = true;

        foreach (var ch in encoded)
        {
            if (!Code39Patterns.TryGetValue(ch, out var pattern)) continue;

            for (int i = 0; i < pattern.Length; i++)
            {
                int w = pattern[i] == 'w' ? wideWidth : barWidth;
                if (isBar)
                    g.FillRectangle(Brushes.Black, x, 0, w, height);
                x += w + 1;
                isBar = !isBar;
            }
            // inter-char gap
            x += barWidth;
            isBar = true;
        }

        // Draw text
        using var font = new Font("Arial", 8);
        g.DrawString(text, font, Brushes.Black, padding, height + 2);

        using var ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Png);
        return Convert.ToBase64String(ms.ToArray());
    }
}
