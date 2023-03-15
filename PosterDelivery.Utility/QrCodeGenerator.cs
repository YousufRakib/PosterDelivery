using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace PosterDelivery.Utility;

public class QrCodeGenerator {
    public static string Generate(string textToEncode) {
        var qrGenerator = new QRCodeGenerator();
        if (textToEncode != null) {
            QRCodeData qRCodeData = qrGenerator.CreateQrCode(textToEncode, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new(qRCodeData);
            Bitmap image = qrCode.GetGraphic(60);

            byte[] bitmapArray = BitmapToByteArray(image);
            string qrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(bitmapArray));

            return qrUri;
        }
        return "";
    }

    private static byte[] BitmapToByteArray(Bitmap bitmap) {
        using (MemoryStream ms = new MemoryStream()) {
            bitmap.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
    }
}
