using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace KineSis.ContentManagement.Model {

    [Serializable]
    public class Shape {

        private Boolean loaded = false;
        public String bitmapUrl;
        public String thumbUrl;

        [NonSerialized]
        Bitmap bitmap;

        public void setThumbnail(String thumbUrl) {
            this.thumbUrl = thumbUrl;
        }

        public Bitmap Thumbnail {
            get {
                Bitmap bmp = null;
                if (thumbUrl != null && thumbUrl.Length > 0) {
                    try {
                        bmp = new Bitmap(thumbUrl);
                    } catch (Exception) {
                    }
                }
                return bmp;
            }
        }

        public void setBitmap(String pictureUrl) {
            bitmapUrl = pictureUrl;
        }

        public Bitmap Bitmap {
            get {
                return bitmap;
            }
        }

        public Boolean IsLoaded {
            get {
                return loaded;
            }
        }

        public void LoadBitmap() {
            if (bitmapUrl != null && bitmapUrl.Length > 0) {
                try {
                    bitmap = new Bitmap(bitmapUrl);
                } catch (Exception) {
                }
                loaded = true;
            }
        }

        public void UnloadBitmap() {
            bitmap.Dispose();
            loaded = false;
        }
    }
}
