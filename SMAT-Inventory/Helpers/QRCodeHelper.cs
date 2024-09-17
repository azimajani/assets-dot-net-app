using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SMAT_Inventory.Helpers
{
    public class QRCodeHelper
    {
        public static string GenerateQrCode(string url, int pixelsPerModule = 10)  // Default size set to 10
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap qrCodeImage = qrCode.GetGraphic(pixelsPerModule))  // Adjust the pixel size
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            qrCodeImage.Save(ms, ImageFormat.Png);
                            byte[] byteImage = ms.ToArray();
                            return Convert.ToBase64String(byteImage);
                        }
                    }
                }
            }
        }
    }
}