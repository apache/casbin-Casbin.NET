// Licensed to the Apache Software Foundation (ASF) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The ASF licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
//
//   https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
#if !NET452
using Microsoft.Extensions.Configuration;
#endif

namespace Casbin.Config
{
    public class DefaultConfig : IConfig
    {
        private static readonly string _defaultSection = "default";
        private static readonly string _defaultComment = "#";
        private static readonly string _defaultFeed = "\\";

#if NET452
        private static readonly string _defaultCommentSem = ";";
#endif

        // Section:key=value
        private readonly IDictionary<string, IDictionary<string, string>> _data;

        private DefaultConfig()
        {
            _data = new Dictionary<string, IDictionary<string, string>>();
        }

        /// <summary>
        /// Creates an empty default configuration representation.
        /// </summary>
        /// <returns>The constructor of Config.</returns>
        public static IConfig Create()
        {
            return new DefaultConfig();
        }

        /// <summary>
        /// Creates an empty default configuration representation from file.
        /// </summary>
        /// <param name="configFilePath">The path of the model file.</param>
        /// <returns>The constructor of Config.</returns>
        public static IConfig CreateFromFile(string configFilePath)
        {
            var config = new DefaultConfig();
            config.Parse(configFilePath);
            return config;
        }

        /// <summary>
        /// Creates an empty default configuration representation from text.
        /// </summary>
        /// <param name="text">The model text.</param>
        /// <returns>The constructor of Config.</returns>
        public static IConfig CreateFromText(string text)
        {
            var config = new DefaultConfig();
            using MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            config.AddStream(config.RemoveComment(memoryStream));
            return config;
        }

        public string Get(string key)
        {
            string section;
            string option;

            var keys = key.ToLower().Split(new string[] { "::" }, StringSplitOptions.None);
            if (keys.Length >= 2)
            {
                section = keys[0];
                option = keys[1];
            }
            else
            {
                section = _defaultSection;
                option = keys[0];
            }

            bool ok = _data.ContainsKey(section) && _data[section].ContainsKey(option);
            if (ok)
            {
                return _data[section][option];
            }
            else
            {
                return string.Empty;
            }
        }

        public bool GetBool(string key)
        {
            return bool.Parse(Get(key));
        }

        public int GetInt(string key)
        {
            return int.Parse(Get(key));
        }

        public float GetFloat(string key)
        {
            return float.Parse(Get(key));
        }

        public string GetString(string key)
        {
            return Get(key);
        }

        public string[] GetStrings(string key)
        {
            string v = Get(key);
            if (string.IsNullOrEmpty(v))
            {
                return null;
            }
            return v.Split(',');
        }

        public void Set(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("key is empty");
            }

            string section = string.Empty;
            string option;

            var keys = key.ToLower().Split(new string[] { "::" }, StringSplitOptions.None);
            if (keys.Length >= 2)
            {
                section = keys[0];
                option = keys[1];
            }
            else
            {
                option = keys[0];
            }
            AddConfig(section, option, value);
        }

        /// <summary>
        /// Adds a new section->key:value to the configuration.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="option"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool AddConfig(string section, string option, string value)
        {
            if (string.IsNullOrEmpty(section))
            {
                section = _defaultSection;
            }

            if (!_data.ContainsKey(section))
            {
                _data.Add(section, new Dictionary<string, string>());
            }

            bool ok = _data[section].ContainsKey(option);
            _data[section].Add(option, value);
            return !ok;
        }

        private void Parse(string configFilePath)
        {
            using FileStream fileStream = File.OpenRead(configFilePath);
            AddStream(RemoveComment(fileStream));
        }

        private Stream RemoveComment(Stream stream)
        {
            TextWriter textWriter = new StringWriter();
            string line;
            string processedValue = string.Empty;
            using var streamReader = new StreamReader(stream);
            while ((line = streamReader.ReadLine()) != null)
            {
                line = line.Split(_defaultComment[0], ';').First().Trim();
                if (line.EndsWith(_defaultFeed))
                {
                    processedValue += line.Split(_defaultFeed[0]).First();
                }
                else
                {
                    processedValue += line;
                    textWriter.WriteLine(processedValue);
                    processedValue = string.Empty;
                }
            }
            if (processedValue != string.Empty)
            {
                textWriter.WriteLine(processedValue);
            }
            return new MemoryStream(Encoding.UTF8.GetBytes(textWriter.ToString()));
        }

        private void AddStream(Stream stream)
        {
#if NET452
            string section = string.Empty;
            int lineNum = 0;
            string line;
            bool inSuccessiveLine = false;
            string option = string.Empty;
            string processedValue = string.Empty;
            using var streamReader = new StreamReader(stream);
            while (true)
            {
                lineNum++;
                try
                {
                    if ((line = streamReader.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                catch (IOException e)
                {
                    throw new IOException("IO error occurred", e);
                }

                line = line.Trim();

                if (line.StartsWith(_defaultComment))
                {
                    continue;
                }
                else if (line.StartsWith(_defaultCommentSem))
                {
                    continue;
                }
                else if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    section = line.Substring(1, line.Length - 2);
                }
                else
                {
                    if (inSuccessiveLine == false)
                    {
                        var optionVal = line.Split("=".ToCharArray(), 2);
                        if (optionVal.Length != 2)
                        {
                            throw new Exception(
                                    string.Format("parse the content error : line {0} , {1} = ? ", lineNum, optionVal[0]));
                        }
                        option = optionVal[0].Trim();
                        string value = optionVal[1].Trim();
                        int commentStartIdx = value.IndexOf(PermConstants.PolicyCommentChar);
                        string lineProcessedValue = (commentStartIdx == -1 ? value : value.Remove(commentStartIdx)).Trim();
                        if (lineProcessedValue.EndsWith(_defaultFeed))
                        {
                            inSuccessiveLine = true;
                            processedValue = lineProcessedValue.Substring(0, lineProcessedValue.Length - 1);
                        }
                        else
                        {
                            inSuccessiveLine = false;
                            processedValue = lineProcessedValue;
                        }
                    }
                    else
                    {
                        string value = line.Trim();
                        int commentStartIdx = value.IndexOf(PermConstants.PolicyCommentChar);
                        string lineProcessedValue = (commentStartIdx == -1 ? value : value.Remove(commentStartIdx)).Trim();
                        if (lineProcessedValue.EndsWith(_defaultFeed))
                        {
                            inSuccessiveLine = true;
                            processedValue += lineProcessedValue.Substring(0, lineProcessedValue.Length - 1);
                        }
                        else
                        {
                            inSuccessiveLine = false;
                            processedValue += lineProcessedValue;
                        }
                    }

                    if (inSuccessiveLine == false)
                    {
                        AddConfig(section, option, processedValue);
                    }
                }
            }
#else
            IConfigurationBuilder builder = new ConfigurationBuilder().AddIniStream(stream);
            IConfigurationRoot configuration = builder.Build();
            var sections = configuration.GetChildren().ToList();

            foreach (var section in sections)
            {
                foreach (var kvPair in section.AsEnumerable())
                {
                    if (kvPair.Value is not null)
                    {
                        AddConfig(section.Path, kvPair.Key.Split(':').Last().Trim(), kvPair.Value.Trim());
                    }
                }
            }
#endif
        }
    }
}
