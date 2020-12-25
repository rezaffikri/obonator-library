using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Obonator.Library
{
    public class ObonCryptography
    {
        public class TripleDes
        {
            //Regex isHex
            static readonly Regex r = new Regex(@"^[0-9A-F\r\n]+$");
            public static string Encrypt(string key, string input, bool outputHex)
            {
                string result = "";
                try
                {
                    byte[] keyArray;
                    byte[] hexData = null;
                    byte[] toEncryptArray;
                    string inputHex = input.Trim().Replace("-", "");
                    string keyHex = key.Trim().Replace("-", "");

                    if (r.Match(inputHex).Success)
                    {
                        hexData = HexStringToByteArray(inputHex);
                        toEncryptArray = hexData;
                    }
                    else
                    {
                        toEncryptArray = Encoding.UTF8.GetBytes(input);
                    }

                    TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                    if (r.Match(keyHex.Trim()).Success)
                    {
                        hexData = HexStringToByteArray(keyHex);
                        keyArray = hexData;
                        tdes.Padding = PaddingMode.None;
                    }
                    else
                    {
                        MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                        keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));

                        hashmd5.Clear();
                        tdes.Padding = PaddingMode.PKCS7;
                    }

                    tdes.Key = keyArray;

                    tdes.Mode = CipherMode.ECB;

                    ICryptoTransform cTransform = tdes.CreateEncryptor();

                    byte[] resultArray =
                      cTransform.TransformFinalBlock(toEncryptArray, 0,
                      toEncryptArray.Length);

                    tdes.Clear();

                    if (outputHex)
                    {
                        result = BitConverter.ToString(resultArray, 0, resultArray.Length);
                    }
                    else
                    {
                        result = Convert.ToBase64String(resultArray, 0, resultArray.Length);
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    string stack = ex.StackTrace;
                }
                return result;
            }

            public static string EncryptOct(string input)
            {
                string result = "";
                try
                {
                    string key = "96CCE365CD20047CA2E90FA065AF7021A1F423FBC2332D5B";
                    byte[] message = Encoding.Default.GetBytes(input);
                    string str1 = BitConverter.ToString(message, 0, message.Length).Replace("-", "");
                    string str2 = "";
                    int num1 = str1.Length % 16;
                    if (num1 > 0)
                        str1 += "".PadLeft(16 - num1, 'F');
                    int num2 = str1.Length / 16;
                    for (int index = 0; index < num2; ++index)
                    {
                        string plainText = str1.Substring(index * 16, 16);
                        string str3 = Encrypt(key, plainText, true).Replace("-", "");
                        str2 += str3;
                    }

                    result = (num1 <= 0 ? "00" + str2 : Convert.ToString((16 - num1) / 2, 16).PadLeft(2, '0') + str2);
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    string stack = ex.StackTrace;
                }
                return result;
            }

            public static byte[] HexStringToByteArray(string hex)
            {
                byte[] resultArray = null;
                try
                {
                    int NumberChars = hex.Length;
                    byte[] bytes = new byte[NumberChars / 2];
                    for (int i = 0; i < NumberChars; i += 2)
                        bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
                    resultArray = bytes;
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    string stack = ex.StackTrace;
                }
                return resultArray;
            }

            public static string Decrypt(string key, string input, bool outputHex)
            {
                string result = "";
                try
                {
                    byte[] keyArray;
                    byte[] hexData = null;
                    byte[] toEncryptArray;                  
                    string inputHex = input.Trim().Replace("-", "");
                    string keyHex = key.Trim().Replace("-", "");

                    if (r.Match(inputHex.Trim()).Success)
                    {
                        hexData = HexStringToByteArray(inputHex);
                        toEncryptArray = hexData;
                    }
                    else
                    {
                        toEncryptArray = Convert.FromBase64String(input);
                    }

                    TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                    if (r.Match(keyHex.Trim()).Success)
                    {
                        hexData = HexStringToByteArray(keyHex);
                        keyArray = hexData;
                        tdes.Padding = PaddingMode.None;
                    }
                    else
                    {
                        MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                        keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                        hashmd5.Clear();
                        tdes.Padding = PaddingMode.PKCS7;
                    }

                    tdes.Key = keyArray;
                    tdes.Mode = CipherMode.ECB;

                    ICryptoTransform cTransform = tdes.CreateDecryptor();
                    byte[] resultArray = cTransform.TransformFinalBlock(
                                         toEncryptArray, 0, toEncryptArray.Length);

                    tdes.Clear();
                    if (outputHex)
                    {
                        result = BitConverter.ToString(resultArray);
                    }
                    else
                    {
                        result = Encoding.UTF8.GetString(resultArray).Replace(@"/[\W_] +/ g", "");
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    string stack = ex.StackTrace;
                }
                return result;
            }

            public static string DecryptOct(string input)
            {
                string result = "";
                try
                {
                    string key = "96CCE365CD20047CA2E90FA065AF7021A1F423FBC2332D5B";
                    string res = Decrypt(key, input.Substring(2), false);
                    result = res.Replace("�", "");
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    string stack = ex.StackTrace;
                }
                return result;
            }
        }

        public class AES
        {
            public static string Decrypt(string key, string input)
            {
                string res = "";
                try
                {
                    using (AesManaged managed = new AesManaged())
                    {
                        byte[] salt = new byte[8];
                        using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(key, salt))
                        {
                            try
                            {
                                managed.BlockSize = 0x80;
                                managed.KeySize = 0x100;
                                managed.Key = bytes.GetBytes(managed.KeySize / 8);
                                managed.IV = bytes.GetBytes(managed.BlockSize / 8);
                                byte[] buffer2 = Convert.FromBase64String(input);
                                res = DecryptStringFromBytes_Aes(buffer2, managed.Key, managed.IV);
                                buffer2 = null;
                            }
                            catch (Exception ex)
                            {
                                string msg = ex.Message;
                                string stack = ex.StackTrace;
                            }
                        }
                        salt = null;
                    }
                }
                catch (Exception)
                {
                    return "";
                }
                return res;
            }

            private static string DecryptStringFromBytes_Aes(byte[] p_bytContent, byte[] p_bytKey, byte[] p_ivIV)
            {
                string str = null;
                if ((p_bytContent == null) || (p_bytContent.Length == 0))
                {
                    return str;
                }
                if ((p_bytKey == null) || (p_bytKey.Length == 0))
                {
                    return str;
                }
                if ((p_ivIV == null) || (p_ivIV.Length == 0))
                {
                    return str;
                }

                using (AesManaged managed = new AesManaged())
                {
                    managed.Key = p_bytKey;
                    managed.IV = p_ivIV;
                    using (ICryptoTransform transform = managed.CreateDecryptor(managed.Key, managed.IV))
                    {
                        using (MemoryStream stream = new MemoryStream(p_bytContent))
                        {
                            using (CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read))
                            {
                                using (StreamReader reader = new StreamReader(stream2))
                                {
                                    try
                                    {
                                        str = reader.ReadToEnd();
                                    }
                                    catch (Exception ex)
                                    {
                                        string msg = ex.Message;
                                        string stack = ex.StackTrace;
                                    }
                                }
                            }
                        }
                    }
                }
                return str;
            }

            public static string Encrypt(string key, string input)
            {
                string res = "";
                try
                {
                    using (AesManaged managed = new AesManaged())
                    {
                        byte[] salt = new byte[8];
                        using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(key, salt))
                        {
                            try
                            {
                                managed.BlockSize = 0x80;
                                managed.KeySize = 0x100;
                                managed.Key = bytes.GetBytes(managed.KeySize / 8);
                                managed.IV = bytes.GetBytes(managed.BlockSize / 8);
                                byte[] inArray = EncryptStringToBytes_Aes(input, managed.Key, managed.IV);
                                res = Convert.ToBase64String(inArray);
                                inArray = null;
                            }
                            catch (Exception ex)
                            {
                                string msg = ex.Message;
                                string stack = ex.StackTrace;
                            }
                            salt = null;
                        }
                    }
                }
                catch (Exception)
                {
                    return "";
                }
                return res;
            }

            private static byte[] EncryptStringToBytes_Aes(string inputData, byte[] p_bytKey, byte[] p_ivIV)
            {
                byte[] buffer = null;
                if ((inputData == null) || (inputData.Length <= 0))
                {
                    throw new ArgumentNullException("inputData");
                }
                if ((p_bytKey == null) || (p_bytKey.Length == 0))
                {
                    throw new ArgumentNullException("p_bytKey");
                }
                if ((p_ivIV == null) || (p_ivIV.Length == 0))
                {
                    throw new ArgumentNullException("p_bytKey");
                }
                using (AesManaged managed = new AesManaged())
                {
                    managed.Key = p_bytKey;
                    managed.IV = p_ivIV;
                    using (ICryptoTransform transform = managed.CreateEncryptor(managed.Key, managed.IV))
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            using (CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write))
                            {
                                using (StreamWriter writer = new StreamWriter(stream2))
                                {
                                    try
                                    {
                                        writer.Write(inputData);
                                    }
                                    catch (Exception ex)
                                    {
                                        string msg = ex.Message;
                                        string stack = ex.StackTrace;
                                    }
                                }
                                try
                                {
                                    buffer = stream.ToArray();
                                }
                                catch (Exception ex)
                                {
                                    string msg = ex.Message;
                                    string stack = ex.StackTrace;
                                }
                            }
                        }
                    }
                }
                return buffer;
            }
        }

        public class MD5Hash
        {
            public static string Hash(string input)
            {
                string result = "";
                try
                {
                    // Convert the input string to a byte array and compute the hash.
                    byte[] data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));

                    // Create a new Stringbuilder to collect the bytes
                    // and create a string.
                    StringBuilder sBuilder = new StringBuilder();

                    // Loop through each byte of the hashed data
                    // and format each one as a hexadecimal string.
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }

                    // Return the hexadecimal string.
                    result = sBuilder.ToString();
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    string stack = ex.StackTrace;
                }
                return result;
            }
        }
    }
}
