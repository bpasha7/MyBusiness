using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { set; get; }
        public string Phone { get; set; }
        public string Link { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public DateTime Birthday { get; set; }
        public void ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            if (image == null)
                return;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();
                Picture = Convert.ToBase64String(imageBytes);
            }
        }

        public Image Base64ToImage()
        {
            if (Picture == null)
                return null;
            var bytes = Convert.FromBase64String(Picture);

            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }

            return image;
        }
    }
}
