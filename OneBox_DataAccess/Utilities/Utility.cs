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
        public const string STORAGEACCOUNTKEY = "srhqodT4jo0wvZbzcZa3XWVM5SLI8Z4lRPGW2EOfUj2O1pREAc1fD+9y2YEVmWkue5NbgUMgw0dD7rInuqS6KQ==";

        public static string GetNextString(string absoluteUri, string filePath)
        {
            absoluteUri = Convention(absoluteUri);
            filePath = Convention(filePath);
            string nextString = "";
            List<string> l1 = Split(absoluteUri, '/');
            List<string> l2 = Split(filePath, '/');
            
            if (l1.Count <= l2.Count)
            {
                return nextString;
            }
            for (int i=0; i<l2.Count; ++i)
            {
                if (l1[i] != l2[i])
                {
                    return nextString;
                }
            }
            return l1[l2.Count];
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

        public static string IdToDns(int emailToContainerId)
        {
            // it should have at least 3 characters

            if (emailToContainerId < 100)
            {                
                return emailToContainerId.ToString("000");
            }
            else
            {
                return emailToContainerId.ToString();
            }
            
        }

        public static List<string> Split(string s, char v)
        {
            s = Convention(s);
            List<string> words = new List<string>();
            //string []words = new string[cnt-1];
            string currentString = "";
            for(int i=1; i<s.Length; ++i) {
                if (s[i]== v)
                {
                    //words[idx] = currentString;
                    words.Add(currentString);
                    currentString = "";
                }
                else
                {
                    currentString += s[i];
                }
            }

            return words;
        }

        public static string GetFileName(string s)
        {
            char v = '/';
            List<string> words = Split(s, v);
            return words[words.Count - 1];
        }

        public static string GetBlockId(long blockNumber)
        {
            string blockId =Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("BlockId{0}",blockNumber.ToString("0000000"))));
            return blockId;
        }


        /// <summary>
        /// Get blob name from a file system onebox's path. Which is without the container name.
        /// and without '/' from the begining/ending of the name.
        /// </summary>
        /// <param name="path">the file system onebox's path.</param>
        /// <param name="containerName">container's name.</param>
        /// <returns>the internal blob's azure name.</returns>
        public static string GetBlobName(string path, string containerName)
        {
            List<string> folders = Split(path, '/');
            string newBlob = string.Empty;
            int deUnde = 0;
            if (folders[0].Equals(containerName))
            {
                deUnde = 1;
            }
            for (int i = deUnde; i < folders.Count; ++i)
            {
                string sep = "/";
                if (i == deUnde)
                {
                    sep = "";
                }
                newBlob = newBlob + sep + folders[i];
            }
            return newBlob;
        }
    }
}
