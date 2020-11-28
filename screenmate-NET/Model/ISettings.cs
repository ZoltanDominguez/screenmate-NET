using System.Collections.Generic;

namespace ScreenMateNET
{
  interface ISettings
  {
    Dictionary<ScreenMateStateID, StateSetting> StateSettings { get; set; }
  }
}