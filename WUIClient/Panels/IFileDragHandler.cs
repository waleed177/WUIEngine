using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUIClient.Panels {
    public interface IFileDragHandler {

        void OnFileDrop(string filepath);

    }
}
