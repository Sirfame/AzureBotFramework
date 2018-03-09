using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using System.Collections.Generic;

namespace HelloWorldBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            String sentimentAnswer = SentimentAnalysis(activity.Text);

            // return our reply to the user
            await context.PostAsync($"You sent {activity.Text} which was {length} characters. Sentiment Analysis: {sentimentAnswer}");
            //await context.PostAsync($"You sent {activity.Text} which was {length} characters.");
            context.Wait(MessageReceivedAsync);
        }

        private String SentimentAnalysis(String text)
        {
            // Create a client.
            ITextAnalyticsAPI client = new TextAnalyticsAPI();
            client.AzureRegion = AzureRegions.Westus;
            client.SubscriptionKey = "0e9cc6838a504e2194eb09d155678423";

            Console.OutputEncoding = System.Text.Encoding.UTF8;


            String result = "";
            SentimentBatchResult result3 = client.Sentiment(
                    new MultiLanguageBatchInput(
                        new List<MultiLanguageInput>()
                        {
                          new MultiLanguageInput("en", "1", text)
                          //new MultiLanguageInput("en", "1", "This was a waste of my time. The speaker put me to sleep."),
                          //new MultiLanguageInput("es", "2", "No tengo dinero ni nada que dar..."),
                          //new MultiLanguageInput("it", "3", "L'hotel veneziano era meraviglioso. È un bellissimo pezzo di architettura."),
                        }));

            //String answer = "";
            //// Printing sentiment results
            foreach (var document in result3.Documents)
            {
                Console.WriteLine("Document ID: {0} , Sentiment Score: {1:0.00}", document.Id, document.Score);
                result += String.Format("Document ID: {0} , Sentiment Score: {1:0.00}", document.Id, document.Score);
            }
            return result;
        }
    }
}