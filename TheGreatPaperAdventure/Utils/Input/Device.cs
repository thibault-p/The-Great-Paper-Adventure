//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TGPA.Localization;
using Microsoft.Xna.Framework.GamerServices;

namespace TGPA.Utils.Input
{
    /// <summary>
    /// Kind of device that can be used to play TGPA
    /// </summary>
    public enum DeviceType
    {
        KeyboardMouse,
        Gamepad, //pad x360
        Joystick //PC Joystick/Joypad
    }

    /// <summary>
    /// A game device
    /// </summary>
    public class Device
    {
        /// <summary>
        /// For a given gamepad, this function determines if there is a profile logged on it
        /// </summary>
        /// <param name="gamepadIndex"></param>
        /// <returns></returns>
        public static bool DeviceHasProfile(Device device)
        {
#if XBOX
            return DeviceProfile(device) != null;
#else
            return false;
#endif
        }

        /// <summary>
        /// For a given gamepad, this function return the logged profile on it
        /// </summary>
        /// <param name="gamepadIndex"></param>
        /// <returns></returns>
        public static SignedInGamer DeviceProfile(Device device)
        {
#if XBOX
            if (device.Type == DeviceType.Gamepad)
            {
                foreach (SignedInGamer gamer in SignedInGamer.SignedInGamers)
                {
                    if ((int)gamer.PlayerIndex == device.Index)
                    {
                        return gamer;
                    }
                }
            }
#endif
            return null;
        }

        /// <summary>
        /// Kind of device
        /// </summary>
        public DeviceType Type { get; set; }

        /// <summary>
        /// Index of the device if required
        /// </summary>ice(DeviceTyp
        public int Index { get; set; }

        public Device()
        {
            Type = DeviceType.KeyboardMouse;
            Index = 0;
        }

        public Device(DeviceType type, int index)
        {
            Type = type;
            Index = index;
        }

        public override string ToString()
        {
            String s = LocalizedStrings.GetString(Type.ToString());

            if ((Type == DeviceType.Joystick) || (Type == DeviceType.Gamepad)) s += " " + Index;

            return s;
        }

        public override bool Equals(object obj)
        {
            if (obj is Device)
            {
                Device d2 = (Device)obj;

                return d2.Type == this.Type
                        && d2.Index == this.Index;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
