using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Customs
{
    public class MyCustoms
    {
        List<string> ContentList;
        Dictionary<string, List<string>> MyDictionary;
        Random random = new Random();
        public (bool success, List<string> results, List<int> index) Search(string data, string searchingText)
        {
            var ss = new SortAlgorithms().SortAscending(searchingText);
            var jh = 0;
            bool success = false;
            List<int> index = new List<int>();
            List<string> results = new List<string>();
            foreach (var s in ss)
            {
                int[] value = SearchString(data, s);
                foreach (var i in value)
                {
                    jh++;
                    if (!index.Contains(i)) { index.Add(i); }
                    if (!results.Contains(s)) { results.Add(s); }
                }
            }
            if(jh > 0) { success = true; }
            return (success, results, index);
        }
        private int[] SearchString(string A, string B)
        {
            List<int> retVal = new List<int>();
            ulong siga = 0;
            ulong sigb = 0;
            ulong Q = 100007;
            ulong D = 256;

            for (int i = 0; i < B.Length; ++i)
            {
                siga = (siga * D + (ulong)A[i]) % Q;
                sigb = (sigb * D + (ulong)B[i]) % Q;
            }

            if (siga == sigb)
                retVal.Add(0);

            ulong pow = 1;

            for (int k = 1; k <= B.Length - 1; ++k)
                pow = (pow * D) % Q;

            for (int j = 1; j <= A.Length - B.Length; ++j)
            {
                siga = (siga + Q - pow * (ulong)A[j - 1] % Q) % Q;
                siga = (siga * D + (ulong)A[j + B.Length - 1]) % Q;

                if (siga == sigb)
                    if (A.Substring(j, B.Length) == B)
                        retVal.Add(j);
            }

            return retVal.ToArray();
        }
        public string GetKeyFromDictionary(Dictionary<string, List<string>> dictionary, string text)
        {
            var textArray = text.Split();
            int high = 0;
            string key = "";
            foreach (var value in dictionary)
            {
                var counter = 0;
                foreach (var item in dictionary[value.Key])
                {
                    for (int i = 0; i < textArray.Length; i++)
                    {
                        if (item.Contains(textArray[i])) { counter++; };
                    }
                    if (high < counter) { high = counter; key = value.Key; }
                }
            }
            return key;
        }
        public bool SearchFunction(object whatToSearchFor, Dictionary<object, List<object>> dictionary)
        {
            var items = new SortAlgorithms().SortAscending(whatToSearchFor);
            var size = dictionary.Count();
            bool flag = false;
            foreach (var key in dictionary)
            {
                var List = dictionary[key];
                for (int i = 0; i < items.Length; i++)
                {
                    if (List.Contains(items[i]))
                    {
                        flag = true;
                    }
                }
            }
            return flag;
        }
        public List<string> GetDictionaryValues(string type, Dictionary<string, List<string>> dictionary)
        {
            var MyList = new List<string>();
            var List = dictionary[type];
            foreach (var value in List)
            {
                MyList.Add(value);
            }
            return MyList;
        }        
        public Dictionary<string, List<string>> CreateDictionary(string type, string content)
        {
            MyDictionary = new Dictionary<string, List<string>>();
            ContentList = new List<string>();
            var ContentArray = content.Split();
            foreach (var value in ContentArray)
            {
                ContentList.Add(value);
            }
            MyDictionary.Add(type, ContentList);
            return MyDictionary;
        }
        private string RandomString(int length)
        {
            const string pool = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ0123456789@#$%^&*()_/?'\"\\";
            var builder = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var c = pool[index: random.Next(0, pool.Length)];
                builder.Append(c);
            }
            return builder.ToString();
        }
        /// <summary>
        /// Gets a list of tokens
        /// </summary>
        /// <param name="numberOfToken">declare the number of tokens</param>
        /// <param name="tokenLength">declare the length for all tokens</param>
        /// <returns></returns>
        public List<string> PrintTokens(int numberOfToken, int tokenLength)
        {
            var list = new List<string>();
            for (int i = 0; i < numberOfToken; i++)
            {
                var token = RandomString(tokenLength);
                if (list.Contains(token))
                {
                    i--;
                    continue;
                }
                list.Add(token);
            }
            return list;
        }
        /// <summary>
        /// Gets one token
        /// </summary>
        /// <param name="minTokenLength">minimun size of token</param>
        /// <param name="maxTokenLength">maximun size of token</param>
        /// <returns></returns>
        public string GetToken(int minTokenLength, int maxTokenLength)
        {
            return RandomString(new Random().Next(minTokenLength, maxTokenLength));
        }
    }
    public class SortAlgorithms
    {
        public string[] SortAscending(object contentToSort)
        {
            string[] sArray;
            if (contentToSort.ToString().Contains(',')) { sArray = contentToSort.ToString().Replace(',', ' ').Split(); }
            else { sArray = contentToSort.ToString().Split(); }
            var size = sArray.Count();
            for (int i = 0; i < size; i++)
            {
                for (int j = 1; j < size; j++)
                {
                    if (sArray[j - 1].CompareTo(sArray[j]) == 1)
                    {
                        var temp = sArray[j - 1];
                        sArray[j - 1] = sArray[j];
                        sArray[j] = temp;
                    }
                }
            }
            return sArray;
        }
        public string[] SortDescending(object contentToSort)
        {
            string[] sArray;
            if (contentToSort.ToString().Contains(',')) { sArray = contentToSort.ToString().Replace(',', ' ').Split(); }
            else { sArray = contentToSort.ToString().Split(); }
            var size = sArray.Count();
            for (int i = 0; i < size; i++)
            {
                for (int j = 1; j < size; j++)
                {
                    if (sArray[j - 1].CompareTo(sArray[j]) == -1)
                    {
                        var temp = sArray[j];
                        sArray[j] = sArray[j - 1];
                        sArray[j - 1] = temp;
                    }
                }
            }
            return sArray;
        }
    }
    public class Encryption
    {
        public string Key { get; set; }
        public string EncryptDataAES(string textData, string Encryptionkey)
        {
            RijndaelManaged objrij = new RijndaelManaged();
            //set the mode for operation of the algorithm   
            objrij.Mode = CipherMode.CBC;
            //set the padding mode used in the algorithm.   
            objrij.Padding = PaddingMode.PKCS7;
            //set the size, in bits, for the secret key.   
            objrij.KeySize = 0x80;
            //set the block size in bits for the cryptographic operation.    
            objrij.BlockSize = 0x80;
            //set the symmetric key that is used for encryption & decryption.    
            byte[] passBytes = Encoding.UTF8.GetBytes(Encryptionkey);
            //set the initialization vector (IV) for the symmetric algorithm    
            byte[] EncryptionkeyBytes = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            int len = passBytes.Length;
            if (len > EncryptionkeyBytes.Length)
            {
                len = EncryptionkeyBytes.Length;
            }
            Array.Copy(passBytes, EncryptionkeyBytes, len);

            objrij.Key = EncryptionkeyBytes;
            objrij.IV = EncryptionkeyBytes;

            //Creates symmetric AES object with the current key and initialization vector IV.    
            ICryptoTransform objtransform = objrij.CreateEncryptor();
            byte[] textDataByte = Encoding.UTF8.GetBytes(textData);
            //Final transform the test string.  
            return Convert.ToBase64String(objtransform.TransformFinalBlock(textDataByte, 0, textDataByte.Length));
        }
        public string DecryptDataAES(string EncryptedText, string Encryptionkey)
        {
            RijndaelManaged objrij = new RijndaelManaged();
            objrij.Mode = CipherMode.CBC;
            objrij.Padding = PaddingMode.PKCS7;

            objrij.KeySize = 0x80;
            objrij.BlockSize = 0x80;
            byte[] encryptedTextByte = Convert.FromBase64String(EncryptedText);
            byte[] passBytes = Encoding.UTF8.GetBytes(Encryptionkey);
            byte[] EncryptionkeyBytes = new byte[0x10];
            int len = passBytes.Length;
            if (len > EncryptionkeyBytes.Length)
            {
                len = EncryptionkeyBytes.Length;
            }
            Array.Copy(passBytes, EncryptionkeyBytes, len);
            objrij.Key = EncryptionkeyBytes;
            objrij.IV = EncryptionkeyBytes;
            byte[] TextByte = objrij.CreateDecryptor().TransformFinalBlock(encryptedTextByte, 0, encryptedTextByte.Length);
            return Encoding.UTF8.GetString(TextByte);  //it will return readable string  
        }
        public string EncryptDataDES(string strData, string strKey)
        {
            byte[] key = { }; //Encryption Key   
            byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
            byte[] inputByteArray;

            try
            {
                key = Encoding.UTF8.GetBytes(strKey);
                // DESCryptoServiceProvider is a cryptography class defind in c#.  
                DESCryptoServiceProvider ObjDES = new DESCryptoServiceProvider();
                inputByteArray = Encoding.UTF8.GetBytes(strData);
                MemoryStream Objmst = new MemoryStream();
                CryptoStream Objcs = new CryptoStream(Objmst, ObjDES.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                Objcs.Write(inputByteArray, 0, inputByteArray.Length);
                Objcs.FlushFinalBlock();

                return Convert.ToBase64String(Objmst.ToArray());//encrypted string  
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public string DecryptDataDES(string strData, string strKey)
        {
            byte[] key = { };// Key   
            byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
            byte[] inputByteArray = new byte[strData.Length];

            try
            {
                key = Encoding.UTF8.GetBytes(strKey);
                DESCryptoServiceProvider ObjDES = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(strData);

                MemoryStream Objmst = new MemoryStream();
                CryptoStream Objcs = new CryptoStream(Objmst, ObjDES.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                Objcs.Write(inputByteArray, 0, inputByteArray.Length);
                Objcs.FlushFinalBlock();

                Encoding encoding = Encoding.UTF8;
                return encoding.GetString(Objmst.ToArray());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public string DecryptString(string inputString, string encryptKey)
        {
            inputString = inputString.Replace("~", "=").Replace("$", "+").Replace("€", "/");
            MemoryStream memStream = null;
            try
            {
                byte[] key = { };
                byte[] IV = { 12, 21, 43, 17, 57, 35, 67, 27 };
                key = Encoding.UTF8.GetBytes(encryptKey);
                byte[] byteInput = new byte[inputString.Length];
                byteInput = Convert.FromBase64String(inputString);
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                memStream = new MemoryStream();
                ICryptoTransform transform = provider.CreateDecryptor(key, IV);
                CryptoStream cryptoStream = new CryptoStream(memStream, transform, CryptoStreamMode.Write);
                cryptoStream.Write(byteInput, 0, byteInput.Length);
                cryptoStream.FlushFinalBlock();
            }
            catch (Exception)
            {
                return "";
            }

            Encoding encoding1 = Encoding.UTF8;
            return encoding1.GetString(memStream.ToArray());
        }
        public string EncryptString(string inputString, string encryptKey)
        {
            MemoryStream memStream = null;
            try
            {
                byte[] key = { };
                byte[] IV = { 12, 21, 43, 17, 57, 35, 67, 27 };
                key = Encoding.UTF8.GetBytes(encryptKey);
                byte[] byteInput = Encoding.UTF8.GetBytes(inputString);
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                memStream = new MemoryStream();
                ICryptoTransform transform = provider.CreateEncryptor(key, IV);
                CryptoStream cryptoStream = new CryptoStream(memStream, transform, CryptoStreamMode.Write);
                cryptoStream.Write(byteInput, 0, byteInput.Length);
                cryptoStream.FlushFinalBlock();

            }
            catch (Exception)
            {
                return "";
            }
            return Convert.ToBase64String(memStream.ToArray()).Replace("=", "~").Replace("+", "$").Replace("/", "€");
        }
        public string StrongEncrypt(string text)
        {
            if (string.IsNullOrEmpty(Key)) { return "Key is required!"; }
            if (Key.Length < 8) { return "An 8 character key is required!"; }
            return EncryptString(EncryptDataAES(EncryptDataDES(text, Key), Key), Key);
        }
        public string StrongDecrypt(string text)
        {
            if (string.IsNullOrEmpty(Key)) { return "Key is required!"; }
            if (Key.Length < 8) { return "An 8 character key is required!"; }
            return DecryptDataDES(DecryptDataAES(DecryptString(text, Key), Key), Key);
        }
        public string GetMD5(string text)
        {
            string getMd5(MD5 mD5Hash, string input)
            {
                byte[] buffer = mD5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < buffer.Length; i++)
                {
                    stringBuilder.Append(buffer[i].ToString("x2"));
                }
                return stringBuilder.ToString();
            }
            var md5_String = string.Empty;
            using (MD5 md5Hash = MD5.Create())
            {
                md5_String = getMd5(md5Hash, text);
            }
            return md5_String;
        }
    }
    public class RegExpression
    {
        private Regex reg;
        Encryption enc = new Encryption() { Key = "1@RoGsT_" };
        public (bool check, string result) IsPassword(string password)
        {
            bool check = false;
            var result = string.Empty;
            reg = new Regex(@"^((?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,30})$");
            MatchCollection match = reg.Matches(password);
            if (match.Count > 0) { check = true; result = enc.StrongEncrypt(password); }
            else { result = "Password must contain an upper-case letter, a lower case letter, a number and a minimum length of 6 characters"; }
            return (check, result);
        }
        public bool IsCorrect(string text, string regExpression)
        {
            bool check = false;
            reg = new Regex(regExpression);
            MatchCollection match = reg.Matches(text);
            if (match.Count > 0) { check = true; }
            return check;
        }
    }
}
