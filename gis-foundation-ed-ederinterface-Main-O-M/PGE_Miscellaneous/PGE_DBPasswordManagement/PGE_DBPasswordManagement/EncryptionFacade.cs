// ========================================================================
// Copyright © 2021 PGE.
// <history>
// PGE DB Password Management (Encryption/Decryption Classes and functions)
// TCS S2NN (EDGISREARC-638) 03/09/2021 (DD/MM/YYYY)                Updated
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;

namespace PGE_DBPasswordManagement
{
    /// <summary>
    /// Helper class to Encrypt and Decrypt passwords using Miner.PasswordEncryption class
    /// </summary>
    internal class EncryptionFacade
    {
        /// <summary>
        /// Encrypts and plain string using Miner.PasswordEncryption class. 
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        internal static string Encrypt(string plainText, Configuration pConfiguration)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            PasswordEncryption passwordEncrypt = new PasswordEncryption(pConfiguration);
            return passwordEncrypt.Encrypt(plainText);
        }
        /// <summary>
        /// Decrypts the string Encrypted using Miner.PasswordEncryption to plain text. 
        /// </summary>
        /// <param name="encryptedPassword"></param>
        /// <returns></returns>
        internal static string Decrypt(string encryptedPassword, Configuration pConfiguration)
        {
            if (string.IsNullOrEmpty(encryptedPassword)) return string.Empty;
            PasswordEncryption passwordEncrypt = new PasswordEncryption(pConfiguration);
            return passwordEncrypt.Decrypt(encryptedPassword);
        }
    }


    internal class PasswordEncryption
    {
        //V3SF Config Change
        private Configuration pConfiguration = default(Configuration);
        // Indicates that the password has been encrypted.

        private static string _PrefixPR = "*#Sn$(",
                              _PrefixNP = "*#Rl%(";

        //V3SF Config Change
        private string _Prefix = default;
        //private string _Prefix = (ConfigurationManager.AppSettings["PROD"] == "Y") ? _PrefixPR : _PrefixNP;

        // Provides the seed for the encryption/decrypton process.
        string keyString = "B374A26A71490437AA024E4FADD5B497FDFF1A8EA6FF12F6FB65AF2720B59CCF";
        string ivString = "7E892875A52C59A3B588306B13C31FBD";

        // Provides the algorithm used to perform encryption/decryption.
        private SymmetricAlgorithm _cryptoService;

        /// <summary>
        /// Construct for initiating Crypto provider
        /// </summary>
        internal PasswordEncryption(Configuration pConfiguration)
        {
            this.pConfiguration = pConfiguration;
            _Prefix = (pConfiguration.AppSettings.Settings["PROD"].Value == "Y") ? _PrefixPR : _PrefixNP;
            _cryptoService = new AesCryptoServiceProvider() { KeySize = 256, Key = StringToByteArray(keyString), IV = StringToByteArray(ivString) };
            //(V3SF) 27 Dec 2021 Added Mode
            _cryptoService.Mode = CipherMode.ECB;
        }

        /// <summary>
        /// Convert Hex string to byte
        /// </summary>
        /// <param name="hex">Hexadecimal string</param>
        /// <returns>Byte array</returns>
        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// Provides encryption of the specified plain text.
        /// </summary>
        /// <param name="plainText">The plain text to be encrypted.</param>
        /// <returns>The encrypted text result.</returns>
        internal string Encrypt(string plainText)
        {
            string encryptedText = plainText;

            try
            { 
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
            }
            catch(CryptographicException cx)
            {
                Console.WriteLine("Provided text is invallid for the current Encrypter. \nPlease change the text : {0} ", plainText);
                encryptedText = string.Empty;
            }
            return encryptedText;
        }

        /// <summary>
        /// Provides decription of the specified encrypted text.
        /// </summary>
        /// <param name="encryptedText">The text to be decrypted.</param>
        /// <returns>The decrypted text result.</returns>
        internal string Decrypt(string encryptedText)
        {
            //V3SF Config Change
            //if (ConfigurationManager.AppSettings["PROD"] == "Y" && encryptedText.StartsWith(_PrefixNP)) return "INVALID ARGUMENT";
            if (pConfiguration.AppSettings.Settings["PROD"].Value == "Y" && encryptedText.StartsWith(_PrefixNP)) return "INVALID ARGUMENT";
            
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
