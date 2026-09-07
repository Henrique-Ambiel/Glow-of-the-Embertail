using System;
using System.Collections.Generic;

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

public class VolumeSettings
{
   public VolumeType soundType;
   public float volume;
}
