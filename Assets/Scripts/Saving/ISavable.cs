using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**Is a interface that every component should implment IF the component needs to save any data
 */

public interface ISavable
{
    object CaptureState();
    void RestoreState(object state);
}
