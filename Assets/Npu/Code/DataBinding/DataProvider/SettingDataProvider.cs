using System;

namespace Npu
{
    public class SettingDataProvider : StaticDataProvider
    {
        public override Type GetDataType()
        {
            return typeof(Npu.Settings);
        }
    }

}