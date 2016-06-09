using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneBox_WebServices.Utilities
{
    public class Utility
    {
        public const DeployState deployState = DeployState.local;

        public static bool IsLocal()
        {
            if (deployState == DeployState.local)
            {
                return true;
            }
            return false;
        }
    }

    public enum DeployState
    {
        local, azure
    }
}