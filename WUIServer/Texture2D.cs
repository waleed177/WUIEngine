using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUIServer {
    public class Texture2D {
        public byte[] bytes;

        public Texture2D(byte[] texture) {
            this.bytes = texture;
        }
    }
}
