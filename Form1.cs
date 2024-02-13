using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stealerium_Decryptor
{
    public partial class Form1 : Form
    {
        string version = "1.0";
        public Form1()
        {
            InitializeComponent();
        }
        private static byte[] CryptKey;
        private static byte[] SaltBytes;
        private void button1_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.StartsWith("ENCRYPTED:", StringComparison.OrdinalIgnoreCase))
            {
                richTextBox3.AppendText($"Webhook: Invalid Webhook" + Environment.NewLine);
                return;
            }
            try
            {
                // Get the input text from richTextBox1
                string inputText = richTextBox1.Text;

                // Split the text into an array of strings based on commas
                string[] values = inputText.Split(',');

                // Update CryptKey with the parsed byte values
                CryptKey = new byte[values.Length];

                for (int i = 0; i < values.Length; i++)
                {
                    if (byte.TryParse(values[i], out byte parsedValue))
                    {
                        CryptKey[i] = parsedValue;
                    }
                    else
                    {
                        richTextBox3.AppendText($"Crypt Key: Unable to parse value at index {i}: {values[i]}. Please enter valid byte values." + Environment.NewLine);
                        return; // Exit the method if parsing fails
                    }
                }
            }
            catch { 

            }
            try
            {
                // Get the input text from richTextBox1
                string inputText = richTextBox2.Text;

                // Split the text into an array of strings based on commas
                string[] values = inputText.Split(',');

                // Update CryptKey with the parsed byte values
                SaltBytes = new byte[values.Length];

                for (int i = 0; i < values.Length; i++)
                {
                    if (byte.TryParse(values[i], out byte parsedValue))
                    {
                        SaltBytes[i] = parsedValue;
                    }
                    else
                    {
                        richTextBox3.AppendText($"Salt Bytes: Unable to parse value at index {i}: {values[i]}. Please enter valid byte values." + Environment.NewLine);
                        return; // Exit the method if parsing fails
                    }
                }
            }
            catch { 
            }
            try
            {
                richTextBox3.Clear();
                richTextBox3.AppendText($"Decrypted Webhook: " + DecryptConfig(textBox1.Text)+ Environment.NewLine);
                MessageBox.Show("Successfully Decrypted!", "Stealerium Decryptor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                richTextBox3.AppendText($"Decrypt: Error!" + Environment.NewLine);
            }


        }
        public static string Decrypt(byte[] bytesToBeDecrypted)
        {
            byte[] bytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(CryptKey, SaltBytes, 1000);
                    aes.Key = rfc2898DeriveBytes.GetBytes(aes.KeySize / 8);
                    aes.IV = rfc2898DeriveBytes.GetBytes(aes.BlockSize / 8);
                    aes.Mode = CipherMode.CBC;
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cryptoStream.Close();
                    }
                    bytes = memoryStream.ToArray();
                }
            }
            return Encoding.UTF8.GetString(bytes);
        }
        public static string DecryptConfig(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }
            if (!value.StartsWith("ENCRYPTED:"))
            {
                return value;
            }
            return Decrypt(Convert.FromBase64String(value.Replace("ENCRYPTED:", "")));
        }
        static string GetRawContent(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    response.EnsureSuccessStatusCode(); // Ensure success status code (2xx)

                    string content = response.Content.ReadAsStringAsync().Result;
                    return content;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                    return string.Empty;
                }
            }
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://pastebin.com/raw/5MibSPkN";

            string content = GetRawContent(url);

            Console.WriteLine(content);
            if (content == version)
            {
                MessageBox.Show("You are using the lasted version! (" + content + ")", "Stealerium Decryptor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("You are using version " + version + ". There is a newer version available ("+content+")", "Stealerium Decryptor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                System.Diagnostics.Process.Start("https://github.com/Oz1p/Stealerium_Decryptor/tree/main");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string url = "https://pastebin.com/raw/5MibSPkN";

            string content = GetRawContent(url);

            Console.WriteLine(content);
            if (content == version)
            {
                
            }
            else
            {
                MessageBox.Show("You are using version " + version + ". There is a newer version available (" + content + ")", "Stealerium Decryptor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                System.Diagnostics.Process.Start("https://github.com/Oz1p/Stealerium_Decryptor/tree/main");
            }
        }
    }
    }


