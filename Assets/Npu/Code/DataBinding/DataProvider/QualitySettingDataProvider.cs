using System;
using UnityEngine;

namespace Npu
{
    public class QualitySettingDataProvider : StaticDataProvider
    {
        public override Type GetDataType()
        {
            return typeof(QualitySettings);
        }
    }

}