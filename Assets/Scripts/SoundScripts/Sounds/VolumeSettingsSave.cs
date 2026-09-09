using System;
using System.Collections.Generic;

[Serializable]
public enum VolumeType
{
   Master,
   Music,
   Sfx
}

[Serializable]
public class VolumeSettingsSave
{
   public List<VolumeSettings> volumeList = new List<VolumeSettings>();
}

[Serializable]
public class VolumeSettings
{
   public VolumeType soundType;
   public float volume;
}
