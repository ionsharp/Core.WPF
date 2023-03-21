using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Imagin.Core.Markup;

public class Parser
{
    static void AddParagraph(FlowDocument document, string input)
    {
        if (input?.Length > 0)
        {
            var block = new TextBlock();
            var result = "";

            Dictionary<string, bool> tags = new()
            {
                { "b",      false },
                { "code",   false },
                { "i",      false },
            };

            void AddRun(string text)
            {
                var run = new Run(text);
                if (tags["b"])
                    run.FontWeight = FontWeights.Bold;

                if (tags["code"])
                    run.Background = System.Windows.Media.Brushes.LightGray;

                if (tags["i"])
                    run.FontStyle = FontStyles.Italic;

                block.Inlines.Add(run);
            }

            for (var i = 0; i < input.Length; i++)
            {
                bool handle = false;
                foreach (var j in tags)
                {
                    //< * >
                    if (i < input.Length - (j.Key.Length + 2) && input.Substring(i, j.Key.Length + 2).ToLower() == "<" + j.Key + ">")
                    {
                        if (result?.Length > 0)
                            AddRun(result);

                        tags[j.Key] = true;
                        result = "";

                        i += j.Key.Length + 1;
                        handle = true;
                        break;
                    }
                    //</ * >
                    else if (i < input.Length - (j.Key.Length + 3) && input.Substring(i, j.Key.Length + 3).ToLower() == "</" + j.Key + ">")
                    {
                        AddRun(result);

                        tags[j.Key] = false;
                        result = "";

                        i += j.Key.Length + 2;
                        handle = true;
                        break;
                    }
                }

                if (!handle)
                    result += input[i];

                /*
                if (i < input.Length - 3 && input.Substring(i, 3) == "<b>")
                {
                    if (result?.Length > 0)
                    {
                        var run = new Run(result);
                        if (bold)
                            run.FontWeight = FontWeights.Bold;

                        if (code)
                            run.Background = System.Windows.Media.Brushes.LightGray;

                        if (italic)
                            run.FontStyle = FontStyles.Italic;

                        block.Inlines.Add(run);
                    }

                    bold = true;
                    result = "";
                    i += 2;
                }
                else if (i < input.Length - 6 && input.Substring(i, 6) == "<code>")
                {
                    if (result?.Length > 0)
                    {
                        var run = new Run(result);
                        if (bold)
                            run.FontWeight = FontWeights.Bold;

                        if (code)
                            run.Background = System.Windows.Media.Brushes.LightGray;

                        if (italic)
                            run.FontStyle = FontStyles.Italic;

                        block.Inlines.Add(run);
                    }

                    code = true;
                    result = "";
                    i += 5;
                }
                else if (i < input.Length - 3 && input.Substring(i, 3) == "<i>")
                {
                    if (result?.Length > 0)
                    {
                        var run = new Run(result);
                        if (bold)
                            run.FontWeight = FontWeights.Bold;

                        if (code)
                            run.Background = System.Windows.Media.Brushes.LightGray;

                        if (italic)
                            run.FontStyle = FontStyles.Italic;

                        block.Inlines.Add(run);
                    }

                    italic = true;
                    result = "";
                    i += 2;
                }

                else if (i < input.Length - 4 && input.Substring(i, 4) == "</b>")
                {
                    bold = false;

                    var run = new Run(result) { FontWeight = FontWeights.Bold };
                    if (code)
                        run.Background = System.Windows.Media.Brushes.LightGray;

                    if (italic)
                        run.FontStyle = FontStyles.Italic;

                    block.Inlines.Add(run);
                    result = "";

                    i += 3;
                }
                else if (i < input.Length - 7 && input.Substring(i, 7) == "</code>")
                {
                    code = false;

                    var run = new Run(result) { Background = System.Windows.Media.Brushes.LightGray };
                    if (bold)
                        run.FontWeight = FontWeights.Bold;

                    if (italic)
                        run.FontStyle = FontStyles.Italic;

                    block.Inlines.Add(run);
                    result = "";

                    i += 6;
                }
                else if (i < input.Length - 4 && input.Substring(i, 4) == "</i>")
                {
                    italic = false;

                    var run = new Run(result) { FontStyle = FontStyles.Italic };
                    if (bold)
                        run.FontWeight = FontWeights.Bold;

                    if (code)
                        run.Background = System.Windows.Media.Brushes.LightGray;

                    block.Inlines.Add(run);
                    result = "";

                    i += 3;
                }
                else
                {
                    result += input[i];
                }
                */
            }

            if (result.Length > 0)
                block.Inlines.Add(new Run(result));

            var paragraph = new Paragraph();
            paragraph.Inlines.Add(block);
            document.Blocks.Add(paragraph);
        }
    }

    public static void Parse(FlowDocument document, string text)
    {
        var result = "";

        string x = "<p>", y = "</p>";
        for (var i = 0; i < text.Length; i++)
        {
            if (i < text.Length - x.Length && text.Substring(i, x.Length) == x)
            {
                AddParagraph(document, result);
                result = "";

                i += x.Length - 1;
            }
            else if (i < text.Length - y.Length && text.Substring(i, y.Length) == y)
            {
                AddParagraph(document, result);
                result = "";

                i += y.Length - 1;
            }
            else result += text[i];
        }

        AddParagraph(document, result);
    }
}