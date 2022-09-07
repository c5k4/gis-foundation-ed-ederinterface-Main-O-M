using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;

namespace Telvent.Delivery.Framework
{
    /// <summary>
    /// Helper class to Encrypt and Decrypt passwords using Miner.PasswordEncryption class
    /// </summary>
    public class EncryptionFacade
    {
        /// <summary>
        /// Encrypts and plain string using Miner.PasswordEncryption class. 
        /// </summary>
        /// <param name="plainPassword"></param>
        /// <returns></returns>
        public static string Encrypt(string plainPassword)
        {
            if (string.IsNullOrEmpty(plainPassword)) return string.Empty;
            PasswordEncryption passwordEncrypt = new PasswordEncryption();
            return passwordEncrypt.Encrypt(plainPassword);  
        }
        /// <summary>
        /// Decrypts the string Encrypted using Miner.PasswordEncryption to plain text. 
        /// </summary>
        /// <param name="encryptedPassword"></param>
        /// <returns></returns>
        public static string Decrypt(string encryptedPassword)
        {
            if (string.IsNullOrEmpty(encryptedPassword)) return string.Empty;
            PasswordEncryption passwordEncrypt = new PasswordEncryption();
            return passwordEncrypt.Decrypt(encryptedPassword); 
        }
    }


    public class PasswordEncryption
    {
        // Indicates that the password has been encrypted.
        private const string _Prefix = "*#Kk%(";

        // Provides the seed for the encryption/decrypton process.
        private const string _encryptionKey = "39($(#)Qkds0__d9i";

        // Provides the algorithm used to perform encryption/decryption.
        private SymmetricAlgorithm _cryptoService;

        public PasswordEncryption()
        {
            _cryptoService = new DESCryptoServiceProvider();

            byte[] bytesKey = ASCIIEncoding.ASCII.GetBytes(_encryptionKey.Substring(0, 8));
            _cryptoService.Key = bytesKey;
            _cryptoService.IV = bytesKey;
        }

        /// <summary>
        /// Provides encryption of the specified plain text.
        /// </summary>
        /// <param name="plainText">The plain text to be encrypted.</param>
        /// <returns>The encrypted text result.</returns>
        public string Encrypt(string plainText)
        {
            string encryptedText = plainText;

            // check to see if the string is already encrypted
            // and don't encrypt if it is.
            if (!plainText.StartsWith(_Prefix))
            {
                // trim out the funky ASCII characters
                System.Text.Encoding ascii = System.Text.Encoding.ASCII;
                Byte[] encodedBytes = ascii.GetBytes(plainText);
                for (int i = 0; i < encodedBytes.Length; i++)
                {
                    if (encodedBytes[i] < 32)
                    {
                        string removeString = new string((char)encodedBytes[i], 1);
                        plainText = plainText.Replace(removeString, "");
                    }
                }

                // create a space-less string
                plainText = plainText.Trim();

                if (plainText.Length > 0)
                {
                    // ignore alpha case
                    //plainText = plainText.ToLower(CultureInfo.CurrentCulture);
                    byte[] bytesIn = System.Text.ASCIIEncoding.ASCII.GetBytes(plainText);

                    // create an in-memory stream for the encrypted result
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();

                    // create an Encryptor from the Provider Service instance
                    ICryptoTransform encrypto = _cryptoService.CreateEncryptor();

                    // create Crypto Stream that transforms a stream using the encryption
                    CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);

                    // write out encrypted content into MemoryStream
                    cs.Write(bytesIn, 0, bytesIn.Length);
                    cs.FlushFinalBlock();

                    // get the output and trim the '\0' bytes
                    // where two successive '\0's are found 
                    byte[] bytesOut = ms.GetBuffer();

                    int idx;
                    for (idx = 0; idx < bytesOut.Length; idx++)
                    {
                        if (bytesOut[idx] == 0 && bytesOut[idx + 1] == 0)
                        {
                            // if the string can't be decrypted, keep
                            // adding bytes until it can be
                            string decryptResult = Decrypt(_Prefix + System.Convert.ToBase64String(bytesOut, 0, idx));
                            if (decryptResult != plainText)
                            {
                                while (decryptResult != plainText & idx < bytesOut.Length)
                                {
                                    idx++;
                                    decryptResult = Decrypt(System.Convert.ToBase64String(bytesOut, 0, idx));
                                }
                                break;
                            }
                            break;
                        }
                    }
                    cs = null;

                    // convert into Base64 so that the result can be used in text
                    encryptedText = _Prefix + System.Convert.ToBase64String(bytesOut, 0, idx);
                }
                else
                {
                    encryptedText = plainText;
                }
            }

            return encryptedText;
        }

        /// <summary>
        /// Provides decription of the specified encrypted text.
        /// </summary>
        /// <param name="encryptedText">The text to be decrypted.</param>
        /// <returns>The decrypted text result.</returns>
        public string Decrypt(string encryptedText)
        {
            string plainText = encryptedText;

            if (encryptedText.Trim().Length > 0 && encryptedText.StartsWith(_Prefix))
            {
                // trim the encrypted string flag from the ciphered text
                encryptedText = encryptedText.Remove(0, _Prefix.Length);

                // convert from Base64 to binary
                byte[] bytesIn = System.Convert.FromBase64String(encryptedText);

                // create a MemoryStream with the input
                System.IO.MemoryStream ms = new System.IO.MemoryStream(bytesIn, 0, bytesIn.Length);

                // create a Decryptor from the Provider Service instance
                ICryptoTransform encrypto = _cryptoService.CreateDecryptor();

                // create Crypto Stream that transforms a stream using the decryption
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);

                // read out the result from the Crypto Stream
                System.IO.StreamReader sr = new System.IO.StreamReader(cs);
                try
                {
                    plainText = sr.ReadToEnd();
                }
                catch (System.IO.IOException)
                {
                    plainText = _Prefix + encryptedText.Trim();
                }
                catch (System.OutOfMemoryException)
                {
                    plainText = _Prefix + encryptedText.Trim();
                }

            }

            return plainText;
        }
    }
}
