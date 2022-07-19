using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLibrary.AutoFiller
{
    public class GoogleFiller : IAutoFiller
    {
        private static string BaseLink = "https://api.dictionaryapi.dev/api/v2/entries/en/";

        private Root WordDefinition;

        private string WordText;

        public GoogleFiller(string spelling)
        {
            WordText = spelling;
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                try
                {
                    string json = client.DownloadString(BaseLink + spelling);
                    List<Root> list = JsonConvert.DeserializeObject<List<Root>>(json);
                    WordDefinition = list[0];
                }
                catch (System.Net.WebException)
                {
                    WordDefinition = new Root();
                    WordDefinition.meanings = new List<Meaning>();
                    WordDefinition.sourceUrls = new List<string>();
                    WordDefinition.sourceUrls.Add(BaseLink + spelling);
                }
            }
        }

        public string ContextText()
        {
            string fullInfo = "";

            foreach (var meaning in WordDefinition.meanings)
            {
                fullInfo += meaning.partOfSpeech + Environment.NewLine;
                int index = 1;

                foreach (var definition in meaning.definitions)
                {
                    if (!String.IsNullOrEmpty(definition.example))
                    {
                        fullInfo += index + " ";
                        fullInfo += definition.example + Environment.NewLine;
                    }
                    index++;
                }

                fullInfo += Environment.NewLine;
            }

            return fullInfo;
        }

        public string DefinitionText()
        {
            string fullInfo = "";

            foreach (var meaning in WordDefinition.meanings)
            {
                fullInfo += meaning.partOfSpeech + Environment.NewLine;
                int index = 1;

                foreach (var definition in meaning.definitions)
                {
                    fullInfo += index + " ";
                    fullInfo += definition.definition + Environment.NewLine;
                    index++;
                }

                fullInfo += Environment.NewLine;
            }

            return fullInfo;
        }

        public string SourceText()
        {
            return String.Join(", ", WordDefinition.sourceUrls);
        }

        public string Spelling()
        {
            return WordText;
        }

        private class Definition
        {
            public string definition { get; set; }
            public List<object> synonyms { get; set; }
            public List<object> antonyms { get; set; }
            public string example { get; set; }
        }

        private class Meaning
        {

            public string partOfSpeech { get; set; }
            public List<Definition> definitions { get; set; }
            public List<string> synonyms { get; set; }
            public List<object> antonyms { get; set; }
        }

        private class Phonetic
        {
            public string audio { get; set; }
            public string sourceUrl { get; set; }
            public string text { get; set; }
        }

        private class Root
        {
            public string word { get; set; }
            public List<Phonetic> phonetics { get; set; }
            public List<Meaning> meanings { get; set; }
            public List<string> sourceUrls { get; set; }
        }
    }
}
