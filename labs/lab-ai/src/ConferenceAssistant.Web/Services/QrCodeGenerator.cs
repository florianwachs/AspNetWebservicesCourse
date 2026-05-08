using QRCoder;

namespace ConferenceAssistant.Web.Services;

public static class QrCodeGenerator
{
    /// <summary>
    /// Generates a QR code as a base64-encoded PNG data URI.
    /// </summary>
    public static string GenerateDataUri(string content, int pixelsPerModule = 10)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M);
        using var pngQrCode = new PngByteQRCode(qrCodeData);
        var pngBytes = pngQrCode.GetGraphic(pixelsPerModule,
            darkColorRgba: [233, 69, 96, 255],   // #e94560 (app accent color)
            lightColorRgba: [0, 0, 0, 0]);        // transparent background
        return $"data:image/png;base64,{Convert.ToBase64String(pngBytes)}";
    }
}
