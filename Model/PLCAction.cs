using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirle.AK1.PLCScriptHandler.Model
{
    public class PLCAction
    {
        public string Name { get; set; } = string.Empty;
        public List<Func<bool>> WriteFuncs { get; private set; } = new List<Func<bool>>();

        public void Fire()
        {
            bool isSuccess = true;
            foreach (var func in WriteFuncs)
            {
                isSuccess &= func.Invoke();
            }
        }
    }
}
