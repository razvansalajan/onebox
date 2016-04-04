using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBox_DataAccess.Utilities
{
    public class Utility
    {
        public const string AdminRoles = "Administratores";
        public static string AdminEmail = "admin@yahoo.com";
        public static string UsersRole = "Users";
        public static string AdminName = "admin";
        public static string AdminPassword = "admin1";
        public static string DatabseName = "oneboxdatabase";

        public const string LocalUsersRoleName = "localregisteredusers";

        public const string STORAGEACCOUNTNAME = "oneboxstorage";
        public const string STORAGEACCOUNTKEY = "gKXXEdoe/AYAE7vBR1KeYiVgd6eo6bhbMpijnak9lfAg/y2T9Yxdtqq6QwkbvEegrciaFwaubP+PLqlVZ0YNjQ==";

        public static string GetNextString(string absoluteUri, string filePath)
        {
            absoluteUri = Convention(absoluteUri);
            filePath = Convention(filePath);
            string nextString = "";
            string[] l1 = Split(absoluteUri, '/');
            string[] l2 = Split(filePath, '/');
            
            if (l1.Length <= l2.Length)
            {
                return nextString;
            }
            for (int i=0; i<l2.Length; ++i)
            {
                if (l1[i] != l2[i])
                {
                    return nextString;
                }
            }
            return l1[l2.Length];
        }

        public static string Convention(string filePath)
        {
            string newString = "";
            if (filePath[0] != '/')
            {
                newString = "/" + filePath;
            }
            else
            {
                newString = filePath;
            }
            if (newString[newString.Length - 1] != '/') {
                newString += "/"; 
            }
            return newString;
        }

        internal static bool IsFolder(string s)
        {
            for (int i = 0; i < s.Length; ++i)
            {
                if (s[i] == '.')
                {
                    return false;
                }

            }
            return true;
        }

        public static string[] Split(string s, char v)
        {
            s = Convention(s);
            int cnt = 0;
            for(int i=0; i<s.Length; ++i)
            {
                if (s[i] == v)
                {
                    ++cnt;
                }
            }
            string []words = new string[cnt-1];
            int idx = 0;
            string currentString = "";
            for(int i=1; i<s.Length; ++i) {
                if (s[i]== v)
                {
                    words[idx] = currentString;
                    currentString = "";
                    ++idx;
                }
                else
                {
                    currentString += s[i];
                }
            }

            return words;
        }
    }
}
